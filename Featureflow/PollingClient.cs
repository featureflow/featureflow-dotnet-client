using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Featureflow.Client
{
    internal class PollingClient : IFeatureControlClient
    {
        private static readonly TimeSpan PollPeriod = TimeSpan.FromSeconds(30);

        private readonly IFeatureControlCache _featureControlCache;
        private readonly FeatureflowConfig _config;
        private readonly RestClient _restClient;
        private readonly Timer _timer;
        private readonly ManualResetEventSlim _waiter = new ManualResetEventSlim(true);
        private bool disposedValue = false; // To detect redundant calls

        internal PollingClient(FeatureflowConfig config, IFeatureControlCache featureControlCache, RestClient restClient)
        {
            _config = config;
            _featureControlCache = featureControlCache;
            _restClient = restClient;
            _timer = new Timer(new TimerCallback(OnTimer), null, TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
        }

        internal Task InitAsync()
        {
            return Task.Run(LoadFeaturesAsync)
                .ContinueWith(_ =>
                {
                    if (_.IsCompleted && _.Result != null)
                    {
                        _featureControlCache.Update(_.Result);
                        InitializeTimer();
                    }
                });
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _waiter.Wait();
                    _timer.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        private void InitializeTimer()
        {
            _timer.Change(PollPeriod, TimeSpan.FromMilliseconds(-1));
        }

        private async void OnTimer(object state)
        {
            try
            {
                _waiter.Reset();
                var newFeatures = await LoadFeaturesAsync();
                _featureControlCache.Update(newFeatures);
            }
            finally
            {
                InitializeTimer();
                _waiter.Set();
            }
        }

        private async Task<IDictionary<string, FeatureControl>> LoadFeaturesAsync()
        {
            var controls = await _restClient.GetAllFeatureControls().ConfigureAwait(false);
            return controls;
        }
    }
}