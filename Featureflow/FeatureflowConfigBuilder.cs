using System;

namespace Featureflow.Client
{
    public class FeatureflowConfigBuilder
    {
        private Uri _baseUri = FeatureflowConfig.DefaultBaseUri;
        private Uri _streamBaseUri = FeatureflowConfig.DefaultStreamBaseUri;
        private TimeSpan _connectionTimeout = FeatureflowConfig.DefaultConnectionTimeout;
        private bool _offline = false;

        public FeatureflowConfigBuilder WithConnectionTimeout(TimeSpan connectionTimeout)
        {
            _connectionTimeout = connectionTimeout;
            return this;
        }

        public FeatureflowConfigBuilder WithBaseUri(Uri baseUri)
        {
            _baseUri = baseUri;
            return this;
        }

        public FeatureflowConfigBuilder WithBaseUri(string baseUri)
        {
            _baseUri = new Uri(baseUri);
            return this;
        }

        public FeatureflowConfigBuilder WithStreamBaseUri(Uri streamBaseUri)
        {
            _streamBaseUri = streamBaseUri;
            return this;
        }

        public FeatureflowConfigBuilder WithStreamBaseUri(string streamBaseUri)
        {
            _streamBaseUri = new Uri(streamBaseUri);
            return this;
        }

        public FeatureflowConfigBuilder WithOffline(bool offline)
        {
            _offline = offline;
            return this;
        }

        public FeatureflowConfig Build()
        {
            return new FeatureflowConfig
            {
                BaseUri = _baseUri,
                StreamBaseUri = _streamBaseUri,
                ConnectionTimeout = _connectionTimeout,
                Offline = _offline
            };
        }
    }
}
