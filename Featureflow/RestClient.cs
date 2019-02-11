using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Featureflow.Client
{
    internal class RestClient
    {
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
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        internal HttpClient CreateHttpClient()
        {
            // var httpClient = new HttpClient(handler: httpClientHandler, disposeHandler: false);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("DotNetClient/" + _restConfig.SdkVersion);
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _apiKey);
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
                _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                cts = new CancellationTokenSource(_config.ConnectionTimeout);
                try
                {
                    return await GetFeatureControls(cts);
                }
                catch (TaskCanceledException tce)
                {
                    _httpClient?.Dispose();
                    _httpClient = CreateHttpClient();
                    _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
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
                    _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                    throw ex;
                }
            }
        }

        internal async Task<bool> SendEventsAsync(IEnumerable<Event> events, CancellationToken cancellationToken)
        {
            var uri = new Uri(_config.BaseUri, FeatureflowConfig.EventsRestPath);
            var content = new StringContent(JsonConvert.SerializeObject(events), Encoding.UTF8, "application/json");

            var timeoutCts = new CancellationTokenSource(_config.ConnectionTimeout);
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            using (var client = CreateHttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "*/*");
                client.DefaultRequestHeaders.Add("Accept-Enconding", "gzip,deflate");
                client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.8");

                using (var response = await client.PostAsync(uri, content, linkedCts.Token))
                {
                    return response.IsSuccessStatusCode;
                }
            }
        }

        private async Task<IDictionary<string, FeatureControl>> GetFeatureControls(CancellationTokenSource cts)
        {
            var requestUri = new Uri(_config.BaseUri, FeatureflowConfig.FeaturesRestPath);
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            if (_etag != null)
            {
                request.Headers.IfNoneMatch.Add(_etag);
            }

            using (var response = await _httpClient.SendAsync(request, cts.Token).ConfigureAwait(false))
            {
                if (response.StatusCode == HttpStatusCode.NotModified)
                {
                    return null;
                }

                _etag = response.Headers.ETag;
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var flags = JsonConvert.DeserializeObject<IDictionary<string, FeatureControl>>(result);
                return flags;
            }
        }
    }
}
