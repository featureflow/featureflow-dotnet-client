using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Featureflow.Client
{
    internal class RestClient
    {
        // private static readonly ILogger Logger = ApplicationLogging.CreateLogger<RestClient>();
        private readonly string _apiKey;
        private readonly HttpClientHandler httpClientHandler = new HttpClientHandler();
        private readonly FeatureflowConfig _config;
        private readonly RestConfig _restConfig;
        private HttpClient _httpClient;
        private EntityTagHeaderValue _etag;

        internal RestClient(string apiKey, FeatureflowConfig config, RestConfig restConfig)
        {
            _apiKey = apiKey;
            _config = config;
            _restConfig = restConfig;
            _httpClient = CreateHttpClient();
        }

        internal HttpClient CreateHttpClient()
        {
            // var httpClient = new HttpClient(handler: httpClientHandler, disposeHandler: false);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("DotNetClient/" + _restConfig.SdkVersion);
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _apiKey);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            return httpClient;
        }

        internal async Task<IDictionary<string, FeatureControl>> GetAllFeatureControls()
        {
            var cts = new CancellationTokenSource(_config.ConnectionTimeout);
            try
            {
                return await GetFeatureControls(cts);
            }
            catch (Exception /*e*/)
            {
                // Using a new client after errors because: https://github.com/dotnet/corefx/issues/11224
                _httpClient?.Dispose();
                _httpClient = CreateHttpClient();

                // Logger<>.LogDebug("Error getting feature flags: " + Util.ExceptionMessage(e) +" waiting 1 second before retrying.");
                // Thread.Sleep(TimeSpan.FromSeconds(1));
                cts = new CancellationTokenSource(_config.ConnectionTimeout);
                try
                {
                    return await GetFeatureControls(cts);
                }
                catch (TaskCanceledException tce)
                {
                    _httpClient?.Dispose();
                    _httpClient = CreateHttpClient();
                    if (tce.CancellationToken == cts.Token)
                    {
                        throw tce;
                    }

                    throw new Exception("Get Features timed out");
                }
                catch (Exception ex)
                {
                    _httpClient?.Dispose();
                    _httpClient = CreateHttpClient();
                    throw ex;
                }
            }
        }

        private async Task<IDictionary<string, FeatureControl>> GetFeatureControls(CancellationTokenSource cts)
        {
            // Logger.LogDebug("Getting all flags with uri: " + _uri.AbsoluteUri);
            var requestUri = new Uri(_config.BaseUri, "api/sdk/v1/features");
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            if (_etag != null)
            {
                request.Headers.IfNoneMatch.Add(_etag);
            }

            using (var response = await _httpClient.SendAsync(request, cts.Token).ConfigureAwait(false))
            {
                if (response.StatusCode == HttpStatusCode.NotModified)
                {
                    // Logger.LogDebug("Get all flags returned 304: not modified");
                    return null;
                }

                _etag = response.Headers.ETag;
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var flags = JsonConvert.DeserializeObject<IDictionary<string, FeatureControl>>(result);
                //// Logger.LogDebug("Get all flags returned " + flags.Keys.Count + " feature flags");
                return flags;
            }
        }
    }
}
