using System;
using System.Collections.Generic;
using System.Threading;

namespace Featureflow.Client
{
    internal class FeatureflowEventsClient
        : IDisposable
    {
        private static readonly int MaximumEventsInQueue = 10000;
        private static readonly TimeSpan EventsSendPeriod = TimeSpan.FromSeconds(30);

        private readonly RestClient _restClient;
        private readonly FeatureflowConfig _config;
        private readonly Timer _timer;
        private readonly List<Event> _events;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private bool disposedValue = false; // To detect redundant calls

        internal FeatureflowEventsClient(RestClient restClient, FeatureflowConfig config)
        {
            _restClient = restClient;
            _config = config;

            if (!_config.Offline)
            {
                _events = new List<Event>(MaximumEventsInQueue);
                _timer = new Timer(OnTimer, null, EventsSendPeriod, EventsSendPeriod);
            }
        }

        internal void QueueEvent(Event evt)
        {
            if (!_config.Offline)
            {
                lock (_events)
                {
                    if (_events.Count < MaximumEventsInQueue)
                    {
                        _events.Add(evt);
                    }
                }
            }
        }

        private async void OnTimer(object state)
        {
            List<Event> events = null;

            lock (_events)
            {
                if (_events.Count > 0)
                {
                    events = new List<Event>(_events);
                    _events.Clear();
                }
            }

            if (events != null)
            {
                try
                {
                    await _restClient.SendEventsAsync(events, _cts.Token).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    // TODO handle exception
                }
            }
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _cts.Cancel();
                    _timer?.Dispose();
                }

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~FeatureflowEventsClient() {
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
