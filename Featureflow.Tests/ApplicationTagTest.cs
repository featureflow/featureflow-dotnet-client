using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Featureflow.Client;
using Xunit;

namespace Featureflow.Tests
{
    // Mirrors the shared application-tag scenarios in the client SDK testbed
    // (featureflow-client-sdk-testbed, gherkin/application_tag.feature): a configured
    // application is sent as X-Featureflow-Application on every request this SDK makes
    // (features polls and event posts); case is forgiven, anything else invalid is dropped
    // with a warning and no header at all.
    public class ApplicationTagTest
    {
        private const string HeaderName = "X-Featureflow-Application";
        private const string EnvironmentVariableName = "FEATUREFLOW_APPLICATION";

        [Fact]
        public async Task ConfiguredApplicationIsSentOnFeaturesRequest()
        {
            var config = new FeatureflowConfigBuilder().WithApplication("checkout-api").Build();

            var request = await SendFeaturesRequestAsync(config);

            Assert.Equal("checkout-api", ApplicationHeader(request));
        }

        [Fact]
        public async Task ConfiguredApplicationIsSentOnEventsRequest()
        {
            var config = new FeatureflowConfigBuilder().WithApplication("checkout-api").Build();

            var request = await SendEventsRequestAsync(config);

            Assert.Equal("checkout-api", ApplicationHeader(request));
        }

        [Fact]
        public async Task MixedCaseApplicationIsLowercased()
        {
            var config = new FeatureflowConfigBuilder().WithApplication("Checkout-API").Build();

            Assert.Equal("checkout-api", config.Application);
            Assert.Equal("checkout-api", ApplicationHeader(await SendFeaturesRequestAsync(config)));
        }

        [Fact]
        public async Task InvalidApplicationIsDroppedWithWarning()
        {
            var originalError = Console.Error;
            var stderr = new StringWriter();
            Console.SetError(stderr);
            FeatureflowConfig config;
            try
            {
                config = new FeatureflowConfigBuilder().WithApplication("checkout api!").Build();
            }
            finally
            {
                Console.SetError(originalError);
            }

            Assert.Null(config.Application);
            Assert.Contains("checkout api!", stderr.ToString());
            Assert.Null(ApplicationHeader(await SendFeaturesRequestAsync(config)));
        }

        [Fact]
        public async Task NoApplicationConfiguredSendsNoHeader()
        {
            var config = WithEnvironmentVariable(null, () => new FeatureflowConfigBuilder().Build());

            Assert.Null(config.Application);
            Assert.Null(ApplicationHeader(await SendFeaturesRequestAsync(config)));
            Assert.Null(ApplicationHeader(await SendEventsRequestAsync(config)));
        }

        [Fact]
        public void EnvironmentVariableIsUsedWhenNotConfigured()
        {
            var config = WithEnvironmentVariable("billing-api", () => new FeatureflowConfigBuilder().Build());

            Assert.Equal("billing-api", config.Application);
        }

        [Fact]
        public void ConfiguredApplicationWinsOverEnvironmentVariable()
        {
            var config = WithEnvironmentVariable(
                "billing-api",
                () => new FeatureflowConfigBuilder().WithApplication("checkout-api").Build());

            Assert.Equal("checkout-api", config.Application);
        }

        private static FeatureflowConfig WithEnvironmentVariable(string value, Func<FeatureflowConfig> build)
        {
            var original = Environment.GetEnvironmentVariable(EnvironmentVariableName);
            Environment.SetEnvironmentVariable(EnvironmentVariableName, value);
            try
            {
                return build();
            }
            finally
            {
                Environment.SetEnvironmentVariable(EnvironmentVariableName, original);
            }
        }

        private static async Task<HttpRequestMessage> SendFeaturesRequestAsync(FeatureflowConfig config)
        {
            var handler = new CapturingHandler();
            await NewRestClient(config, handler).GetFeatureControlsAsync(CancellationToken.None);
            return Assert.Single(handler.Requests);
        }

        private static async Task<HttpRequestMessage> SendEventsRequestAsync(FeatureflowConfig config)
        {
            var handler = new CapturingHandler();
            await NewRestClient(config, handler).SendEventsAsync(new List<Event>(), CancellationToken.None);
            return Assert.Single(handler.Requests);
        }

        private static RestClient NewRestClient(FeatureflowConfig config, CapturingHandler handler)
        {
            var restConfig = new RestConfig
            {
                SdkVersion = "0.0.0-test",
                HttpMessageHandler = handler,
            };
            return new RestClient("sdk-srv-env-test", config, restConfig);
        }

        private static string ApplicationHeader(HttpRequestMessage request)
        {
            return request.Headers.TryGetValues(HeaderName, out var values) ? string.Join(",", values) : null;
        }

        private class CapturingHandler : HttpMessageHandler
        {
            internal List<HttpRequestMessage> Requests { get; } = new List<HttpRequestMessage>();

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Requests.Add(request);
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{}", Encoding.UTF8, "application/json"),
                };
                return Task.FromResult(response);
            }
        }
    }
}
