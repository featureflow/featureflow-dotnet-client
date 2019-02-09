using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Featureflow.Client
{
    public class FeatureflowClient : IFeatureflowClient
    {
        // private static readonly ILogger Logger = ApplicationLogging.CreateLogger<FeatureflowClient>();
        private readonly FeatureflowConfig _config;
        private readonly IFeatureControlCache _featureControlCache; // local cache
        private readonly IFeatureControlClient _featureControlClient; // retrieves features
        private readonly RestClient _restClient;
        private readonly Dictionary<string, Feature> _featureDefaults = new Dictionary<string, Feature>();

        public FeatureflowClient(string apiKey)
            : this(apiKey, new List<Feature>(), new FeatureflowConfig())
        {
        }

        public FeatureflowClient(string apiKey, List<Feature> features)
            : this(apiKey, features, new FeatureflowConfig())
        {
        }

        public FeatureflowClient(string apiKey, List<Feature> features, FeatureflowConfig config)
        {
            // Logger.LogInformation("Initialising Featureflow...");
            features?.ForEach(feature => _featureDefaults[feature.Key] = feature);
            _featureControlCache = new SimpleMemoryFeatureCache();
            _config = config;

            if (_config.Offline)
            {
                // Logger.LogWarning("Featureflow is in Offline mode. Registered defaults will be used.");
                return;
            }

            var restConfig = new RestConfig
            {
                SdkVersion = ((AssemblyInformationalVersionAttribute)typeof(FeatureflowClient)
                    .GetTypeInfo().Assembly.GetCustomAttribute(typeof(AssemblyInformationalVersionAttribute))).InformationalVersion
            };

            _restClient = new RestClient(apiKey, config, restConfig);

            // start the featureControl Client
            var featureControlClient = new PollingClient(config, _featureControlCache, _restClient);
            var initTask = featureControlClient.InitAsync(); // initialise
            var waitResult = initTask.Wait(_config.ConnectionTimeout);
            if (!waitResult)
            {
                throw new TimeoutException("initialization failed");
            }

            _featureControlClient = featureControlClient;
        }

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

            return new Evaluate(featureControl, user, failoverVariant);
        }
    }
}
