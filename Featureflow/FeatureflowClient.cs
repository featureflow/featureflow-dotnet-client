using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Featureflow.Client
{
    public class FeatureflowClient : IFeatureflowClient
    {
        private readonly FeatureflowConfig _config;
        private readonly IFeatureControlCache _featureControlCache; // local cache
        private readonly PollingClient _pollingClient; // retrieves features
        private readonly FeatureflowEventsClient _eventsClient;
        private readonly FeatureflowStreamClient _streamClient;
        private readonly RestClient _restClient;
        private readonly Dictionary<string, Feature> _featureDefaults = new Dictionary<string, Feature>();
        private bool disposedValue = false; // To detect redundant calls

        public FeatureflowClient(string apiKey)
            : this(apiKey, null, new FeatureflowConfig())
        {
        }

        public FeatureflowClient(string apiKey, IEnumerable<Feature> defaultFeatures)
            : this(apiKey, defaultFeatures, new FeatureflowConfig())
        {
        }

        public FeatureflowClient(string apiKey, IEnumerable<Feature> defaultFeatures, FeatureflowConfig config)
        {
            InitializeDefaultFeatures(defaultFeatures);

            _featureControlCache = new SimpleMemoryFeatureCache();
            _config = config;

            if (_config.Offline)
            {
                return;
            }

            var restConfig = new RestConfig
            {
                SdkVersion = ((AssemblyInformationalVersionAttribute)typeof(FeatureflowClient)
                    .GetTypeInfo().Assembly.GetCustomAttribute(typeof(AssemblyInformationalVersionAttribute))).InformationalVersion
            };

            _restClient = new RestClient(apiKey, config, restConfig);

            // start the featureControl Client
            var pollingClient = new PollingClient(config, _featureControlCache, _restClient);
            var initTask = pollingClient.InitAsync(); // initialise
            var waitResult = initTask.Wait(_config.ConnectionTimeout);
            if (!waitResult)
            {
                throw new TimeoutException("initialization failed");
            }

            _pollingClient = pollingClient;
            _pollingClient.FeatureUpdated += OnFeatureUpdated;
            _pollingClient.FeatureDeleted += OnFeatureDeleted;

            _eventsClient = new FeatureflowEventsClient(_restClient, config);

            _streamClient = new FeatureflowStreamClient(_featureControlCache, _restClient, _config);
            _streamClient.FeatureUpdated += OnFeatureUpdated;
            _streamClient.FeatureDeleted += OnFeatureDeleted;
            _streamClient.Start();
        }

        public event FeatureUpdatedEventHandler FeatureUpdated;

        public event FeatureDeletedEventHandler FeatureDeleted;

        public Evaluate Evaluate(string featureKey, User user)
        {
            return EvaluateInternal(_featureControlCache.Get(featureKey), user);
        }

        public Evaluate Evaluate(string featureKey)
        {
            return Evaluate(featureKey, User.Anonymous());
        }

        public Dictionary<string, Evaluate> EvaluateAll(User user)
        {
            return _featureControlCache
                .GetAll()
                .ToDictionary(_ => _.Key, _ => EvaluateInternal(_.Value, user));
        }

        public Dictionary<string, Evaluate> EvaluateAll()
        {
            return EvaluateAll(User.Anonymous());
        }

        private Evaluate EvaluateInternal(FeatureControl featureControl, User user)
        {
            var failoverVariant = "off";

            if (featureControl != null &&
                _featureDefaults.TryGetValue(featureControl.Key, out var failoverFeature))
            {
                failoverVariant = failoverFeature.FailoverVariant;
            }

            return new Evaluate(featureControl, user, failoverVariant, _eventsClient);
        }

        private void InitializeDefaultFeatures(IEnumerable<Feature> defaultFeatures)
        {
            if (defaultFeatures != null)
            {
                foreach (Feature feature in defaultFeatures)
                {
                    if (feature == null)
                    {
                        throw new ArgumentNullException("Default feature is null");
                    }

                    if (feature.Key == null)
                    {
                        throw new ArgumentNullException("Default feature have NULL key");
                    }

                    _featureDefaults[feature.Key] = feature;
                }
            }
        }

        private void OnFeatureUpdated(object sender, FeatureUpdatedEventArgs e)
        {
            FeatureUpdated?.Invoke(this, e);
        }

        private void OnFeatureDeleted(object sender, FeatureDeletedEventArgs e)
        {
            FeatureDeleted?.Invoke(this, e);
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _pollingClient.FeatureUpdated -= OnFeatureUpdated;
                    _pollingClient.FeatureDeleted -= OnFeatureDeleted;
                    _pollingClient.Dispose();

                    _streamClient.FeatureUpdated -= OnFeatureUpdated;
                    _streamClient.FeatureDeleted -= OnFeatureDeleted;
                    _streamClient.Dispose();
                }

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~FeatureflowClient() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
