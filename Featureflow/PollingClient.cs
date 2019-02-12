using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Featureflow.Client
{
    internal class PollingClient
    {
        private static readonly TimeSpan PollPeriod = TimeSpan.FromSeconds(30);

        private readonly IFeatureControlCache _featureControlCache;
        private readonly FeatureflowConfig _config;
        private readonly RestClient _restClient;
        private readonly Timer _timer;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private bool disposedValue = false; // To detect redundant calls

        internal PollingClient(FeatureflowConfig config, IFeatureControlCache featureControlCache, RestClient restClient)
        {
            _config = config;
            _featureControlCache = featureControlCache;
            _restClient = restClient;
            _timer = new Timer(new TimerCallback(OnTimer), null, TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
        }

        internal event EventHandler<FeatureUpdatedEventArgs> FeatureUpdated;

        internal event EventHandler<FeatureDeletedEventArgs> FeatureDeleted;

        internal async Task InitAsync(CancellationToken cancellationToken)
        {
            var features = await _restClient.GetFeatureControlsAsync(cancellationToken).ConfigureAwait(false);
            _featureControlCache.Update(features);
            InitializeTimer();
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _timer.Dispose();
                    _cts.Cancel();
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
                if (_cts.IsCancellationRequested)
                {
                    return;
                }

                var newFeatures = await _restClient.GetFeatureControlsAsync(_cts.Token).ConfigureAwait(false);

                if (_cts.IsCancellationRequested)
                {
                    return;
                }

                var oldKeys = new HashSet<string>(_featureControlCache.GetAll().Keys);

                _featureControlCache.Update(newFeatures);


                foreach (var newFeature in newFeatures)
                {
                    FeatureUpdated?.Invoke(this, new FeatureUpdatedEventArgs(newFeature.Key));
                }

                oldKeys.ExceptWith(newFeatures.Keys);
                foreach (var missingKey in oldKeys)
                {
                    FeatureDeleted?.Invoke(this, new FeatureDeletedEventArgs(missingKey));
                }
            }
            finally
            {
                if (!_cts.IsCancellationRequested)
                {
                    InitializeTimer();
                }
            }
        }
    }
}