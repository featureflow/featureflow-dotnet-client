using System;
using System.Threading;
using System.Threading.Tasks;
//using Microsoft.Extensions.Logging;

namespace Featureflow.Client
{
    public class PollingClient : IFeatureControlClient
    {
        //private static readonly ILogger Logger = ApplicationLogging.CreateLogger<PollingClient>();
        private IFeatureControlCache _featureControlCache;
        private volatile bool _isFeatureControlCacheInitialized; // TODO remove
        private FeatureflowConfig _config;
        private int _initialized = 0;
        private readonly TaskCompletionSource<bool> _initTask;
        private bool _disposed;
        private RestClient _restClient;

        internal PollingClient(FeatureflowConfig config, IFeatureControlCache featureControlCache, RestClient restClient)
        {
            _config = config;
            _featureControlCache = featureControlCache;            
            _restClient = restClient;
            _initTask = new TaskCompletionSource<bool>();
        }

        public TaskCompletionSource<bool> Init()
        {
            
            //Logger.LogInformation("Starting Featureflow Client");
            Task.Run(GetFeaturesPollAsync);
            return _initTask;
            
            
        }

        private async Task GetFeaturesPollAsync()
        {
            while (!_disposed)
            {
                await UpdateFeaturesAsync();
                await Task.Delay(30000);
            }
        }
        private async Task UpdateFeaturesAsync()
        {

            var featureControls = await _restClient.GetAllFeatureControls();
            if (featureControls != null)
            {
                _featureControlCache.Update(featureControls);
                if (!_isFeatureControlCacheInitialized)
                {

                    //Logger.LogInformation("Initialized Featureflow FeatureControl Client.");
                    _isFeatureControlCacheInitialized = true;
                    _initTask.SetResult(true);
                }   
            }          
        }
        
        void IDisposable.Dispose()
        {
            //Logger.LogInformation("Stopping FeatureControl Client");
            _disposed = true;
        }
    }
}