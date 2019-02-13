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
        private readonly FeatureflowConfig _config;
        private readonly RestConfig _restConfig;
        private EntityTagHeaderValue _etag;

        internal RestClient(string apiKey, FeatureflowConfig config, RestConfig restConfig)
        {
            _apiKey = apiKey;
            _config = config;
            _restConfig = restConfig;
        }

        internal HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("DotNetClient/" + _restConfig.SdkVersion);
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _apiKey);
            return httpClient;
        }

        internal async Task<bool> SendEventsAsync(IEnumerable<Event> events, CancellationToken cancellationToken)
        {
            var uri = new Uri(_config.EventsBaseUri, FeatureflowConfig.EventsRestPath);
            var content = new StringContent(JsonConvert.SerializeObject(events), Encoding.UTF8, "application/json");

            var timeoutCts = new CancellationTokenSource(_config.ConnectionTimeout);
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            using (var client = CreateHttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "*/*");
                client.DefaultRequestHeaders.Add("Accept-Enconding", "gzip,deflate");
                client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.8");

                using (var response = await client.PostAsync(uri, content, linkedCts.Token).ConfigureAwait(false))
                {
                    return response.IsSuccessStatusCode;
                }
            }
        }

        internal async Task<IDictionary<string, FeatureControl>> GetFeatureControlsAsync(CancellationToken cancellationToken)
        {
            var requestUri = new Uri(_config.BaseUri, FeatureflowConfig.FeaturesRestPath);
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            if (_etag != null)
            {
                request.Headers.IfNoneMatch.Add(_etag);
            }

            var timeoutCts = new CancellationTokenSource(_config.ConnectionTimeout);
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            using (var httpClient = CreateHttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                using (var response = await httpClient.SendAsync(request, linkedCts.Token).ConfigureAwait(false))
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
}
