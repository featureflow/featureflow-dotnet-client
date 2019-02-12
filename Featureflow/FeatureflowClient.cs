using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Featureflow.Client
{
    internal class FeatureflowClient : IFeatureflowClient
    {
        private readonly FeatureflowConfig _config;
        private readonly IFeatureControlCache _featureControlCache; // local cache
        private readonly PollingClient _pollingClient; // retrieves features
        private readonly FeatureflowEventsClient _eventsClient;
        private readonly FeatureflowStreamClient _streamClient;
        private readonly RestClient _restClient;
        private readonly Dictionary<string, Feature> _featureDefaults = new Dictionary<string, Feature>();
        private bool disposedValue = false; // To detect redundant calls

        private FeatureflowClient(string apiKey)
            : this(apiKey, null, new FeatureflowConfig())
        {
        }

        private FeatureflowClient(string apiKey, IEnumerable<Feature> defaultFeatures)
            : this(apiKey, defaultFeatures, new FeatureflowConfig())
        {
        }

        private FeatureflowClient(string apiKey, IEnumerable<Feature> defaultFeatures, FeatureflowConfig config)
        {
            _config = config;
            _featureControlCache = new SimpleMemoryFeatureCache();

            InitializeDefaultFeatures(defaultFeatures);

            if (!_config.Offline)
            {
                var restConfig = new RestConfig
                {
                    SdkVersion = ((AssemblyInformationalVersionAttribute)typeof(FeatureflowClient)
                        .GetTypeInfo().Assembly.GetCustomAttribute(typeof(AssemblyInformationalVersionAttribute))).InformationalVersion
                };

                _restClient = new RestClient(apiKey, config, restConfig);
                _eventsClient = new FeatureflowEventsClient(_restClient, config);

                switch (_config.GetFeaturesMethod)
                {
                    case GetFeaturesMethod.Polling:
                        _pollingClient = new PollingClient(config, _featureControlCache, _restClient);
                        _pollingClient.FeatureUpdated += OnFeatureUpdated;
                        _pollingClient.FeatureDeleted += OnFeatureDeleted;
                        break;

                    case GetFeaturesMethod.Sse:
                        _streamClient = new FeatureflowStreamClient(_config, _featureControlCache, _restClient);
                        _streamClient.FeatureUpdated += OnFeatureUpdated;
                        _streamClient.FeatureDeleted += OnFeatureDeleted;
                        break;
                }
            }
        }

        public event FeatureUpdatedEventHandler FeatureUpdated;

        public event FeatureDeletedEventHandler FeatureDeleted;

        public Evaluate Evaluate(string featureKey, User user)
        {
            EnsureNotDisposed();
            return EvaluateInternal(_featureControlCache.Get(featureKey), user);
        }

        public Evaluate Evaluate(string featureKey)
        {
            return Evaluate(featureKey, User.Anonymous());
        }

        public Dictionary<string, Evaluate> EvaluateAll(User user)
        {
            EnsureNotDisposed();
            return _featureControlCache
                .GetAll()
                .ToDictionary(_ => _.Key, _ => EvaluateInternal(_.Value, user));
        }

        public Dictionary<string, Evaluate> EvaluateAll()
        {
            return EvaluateAll(User.Anonymous());
        }

        internal static IFeatureflowClient Create(
            string apiKey,
            IEnumerable<Feature> defaultFeatures,
            FeatureflowConfig featureflowConfig)
        {
            var client = new FeatureflowClient(apiKey, defaultFeatures, featureflowConfig);
            using (var evt = new ManualResetEventSlim(false))
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await client.InitializeAsync(CancellationToken.None).ConfigureAwait(false);
                    }
                    finally
                    {
                        evt.Set();
                    }
                });

                evt.Wait();
            }

            return client;
        }

        internal static async Task<IFeatureflowClient> CreateAsync(
            string apiKey,
            IEnumerable<Feature> defaultFeatures,
            FeatureflowConfig featureflowConfig,
            CancellationToken cancellationToken)
        {
            var client = new FeatureflowClient(apiKey, defaultFeatures, featureflowConfig);
            await client.InitializeAsync(cancellationToken).ConfigureAwait(false);
            return client;
        }

        private Task InitializeAsync(CancellationToken cancellationToken)
        {
            if (!_config.Offline)
            {
                switch (_config.GetFeaturesMethod)
                {
                    case GetFeaturesMethod.Polling:
                        return _pollingClient.InitAsync(cancellationToken);

                    case GetFeaturesMethod.Sse:
                        _streamClient.Init(cancellationToken);
                        break;
                }
            }

            return Task.FromResult(true);
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

        private void EnsureNotDisposed()
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(nameof(FeatureflowClient));
            }
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_pollingClient != null)
                    {
                        _pollingClient.FeatureUpdated -= OnFeatureUpdated;
                        _pollingClient.FeatureDeleted -= OnFeatureDeleted;
                        _pollingClient.Dispose();
                    }

                    if (_streamClient != null)
                    {
                        _streamClient.FeatureUpdated -= OnFeatureUpdated;
                        _streamClient.FeatureDeleted -= OnFeatureDeleted;
                        _streamClient.Dispose();
                    }
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
