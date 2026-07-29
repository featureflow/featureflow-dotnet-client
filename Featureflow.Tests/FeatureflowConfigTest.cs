using System;
using Featureflow.Client;
using Xunit;

namespace Featureflow.Tests
{
    // Regression cover for a directly-constructed FeatureflowConfig (the form the README documents).
    // BaseUri used to default to the SSE stream host, so GetFeaturesMethod.Polling fetched features
    // from a host that does not serve the polling endpoint.
    public class FeatureflowConfigTest
    {
        private static readonly Uri ExpectedPollingUri = new Uri("https://app.featureflow.io/api/sdk/v1/features");
        private static readonly Uri ExpectedStreamUri = new Uri("https://rtm.featureflow.io/api/sdk/v1/features");

        [Fact]
        public void DefaultConfigPollsTheRestHost()
        {
            var config = new FeatureflowConfig { GetFeaturesMethod = GetFeaturesMethod.Polling };

            Assert.Equal(ExpectedPollingUri, new Uri(config.BaseUri, FeatureflowConfig.FeaturesRestPath));
        }

        [Fact]
        public void DefaultConfigStreamsFromTheStreamHost()
        {
            var config = new FeatureflowConfig();

            Assert.Equal(GetFeaturesMethod.Sse, config.GetFeaturesMethod);
            Assert.Equal(ExpectedStreamUri, new Uri(config.StreamBaseUri, FeatureflowConfig.StreamFeaturesRestPath));
        }

        [Fact]
        public void DirectlyConstructedConfigMatchesBuilderDefaults()
        {
            var direct = new FeatureflowConfig();
            var built = new FeatureflowConfigBuilder().Build();

            Assert.Equal(built.BaseUri, direct.BaseUri);
            Assert.Equal(built.StreamBaseUri, direct.StreamBaseUri);
            Assert.Equal(built.EventsBaseUri, direct.EventsBaseUri);
            Assert.Equal(built.ConnectionTimeout, direct.ConnectionTimeout);
            Assert.Equal(built.GetFeaturesMethod, direct.GetFeaturesMethod);
            Assert.Equal(built.Offline, direct.Offline);
        }

        [Fact]
        public void BuiltPollingConfigPollsTheRestHost()
        {
            var config = new FeatureflowConfigBuilder()
                .WithGetFeaturesMethod(GetFeaturesMethod.Polling)
                .Build();

            Assert.Equal(ExpectedPollingUri, new Uri(config.BaseUri, FeatureflowConfig.FeaturesRestPath));
        }

        [Fact]
        public void ExplicitBaseUriOverridesTheDefault()
        {
            var config = new FeatureflowConfigBuilder()
                .WithBaseUri("https://featureflow.internal.example.com")
                .WithGetFeaturesMethod(GetFeaturesMethod.Polling)
                .Build();

            Assert.Equal(
                new Uri("https://featureflow.internal.example.com/api/sdk/v1/features"),
                new Uri(config.BaseUri, FeatureflowConfig.FeaturesRestPath));
        }
    }
}
