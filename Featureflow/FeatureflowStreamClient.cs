using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;

namespace Featureflow.Client
{
    internal class FeatureflowStreamClient
        : IDisposable
    {
        private const string FeaturesUpdatedEventType = "features.updated";
        private const string FeaturesDeletedEventType = "features.deleted";

        private readonly IFeatureControlCache _controlCache;
        private readonly SseClient _sseClient;
        private readonly Timer _reconnectionTimer;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private bool _canReinitializeControlCache;
        private bool disposedValue = false; // To detect redundant calls

        internal FeatureflowStreamClient(IFeatureControlCache controlCache, RestClient restClient, FeatureflowConfig config)
        {
            _controlCache = controlCache;
            _reconnectionTimer = new Timer(OnTimer, null, TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
            _sseClient = new SseClient(restClient, config);
            _sseClient.MessageReceived += OnSseClient_MessageReceived;
            _sseClient.Disconnected += OnSseClient_Disconnected;
        }

        internal void Start()
        {
            _canReinitializeControlCache = true;
            _sseClient.Start(_cts.Token);
        }

        private void OnTimer(object state)
        {
            _sseClient.Start(_cts.Token);
        }

        private void OnSseClient_MessageReceived(object sender, SseClient.Message e)
        {
            switch (e.EventType)
            {
                case FeaturesUpdatedEventType:
                    try
                    {
                        Dictionary<string, FeatureControl> entries = JsonConvert.DeserializeObject<Dictionary<string, FeatureControl>>(e.Value);
                        if (_canReinitializeControlCache)
                        {
                            _canReinitializeControlCache = false;
                            _controlCache.Update(entries);
                        }
                        else
                        {
                            foreach (var entry in entries)
                            {
                                _controlCache.Set(entry.Value);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        _canReinitializeControlCache = false;
                    }

                    break;

                case FeaturesDeletedEventType:
                    try
                    {
                        List<string> entries = JsonConvert.DeserializeObject<List<string>>(e.Value);
                        foreach (var entry in entries)
                        {
                            _controlCache.Delete(entry);
                        }
                    }
                    catch (Exception)
                    {
                    }

                    break;
            }
        }

        private void OnSseClient_Disconnected(object sender, SseClient.Disconnect e)
        {
            _reconnectionTimer.Change(e.WaitTime, TimeSpan.FromMilliseconds(-1));
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _cts.Cancel();

                    _reconnectionTimer.Dispose();

                    _sseClient.Disconnected -= OnSseClient_Disconnected;
                    _sseClient.MessageReceived -= OnSseClient_MessageReceived;
                    _sseClient.Dispose();
                }

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~FeatureflowStreamClient() {
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
