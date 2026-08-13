using System;

namespace Featureflow.Client
{
    public class FeatureflowConfigBuilder
    {
        private Uri _baseUri = FeatureflowConfig.DefaultBaseUri;
        private Uri _streamBaseUri = FeatureflowConfig.DefaultStreamBaseUri;
        private TimeSpan _connectionTimeout = FeatureflowConfig.DefaultConnectionTimeout;
        private bool _offline = false;
        private GetFeaturesMethod _method = GetFeaturesMethod.Sse;
        private string _application;

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

        public FeatureflowConfigBuilder WithGetFeaturesMethod(GetFeaturesMethod method)
        {
            _method = method;
            return this;
        }

        /// <summary>
        /// Names this workload (e.g. "checkout-api") so the Featureflow dashboard can attribute
        /// SDK usage and flag evaluations to it. Sent as the X-Featureflow-Application header on
        /// every request. A slug: lowercase [a-z0-9._-], max 64 chars - invalid values are dropped
        /// with a warning. Falls back to the FEATUREFLOW_APPLICATION environment variable when not
        /// set here.
        /// </summary>
        public FeatureflowConfigBuilder WithApplication(string application)
        {
            _application = application;
            return this;
        }

        public FeatureflowConfig Build()
        {
            var config = new FeatureflowConfig
            {
                BaseUri = _baseUri,
                StreamBaseUri = _streamBaseUri,
                ConnectionTimeout = _connectionTimeout,
                Offline = _offline,
                GetFeaturesMethod = _method,
            };

            if (_application != null)
            {
                // Config wins over the FEATUREFLOW_APPLICATION environment variable, which
                // FeatureflowConfig itself falls back to when nothing is set here.
                config.Application = FeatureflowConfig.SanitiseApplication(_application);
            }

            return config;
        }
    }
}
