using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Featureflow.Client
{
    internal class SseClient
        : IDisposable
    {
        private static readonly TimeSpan DefaultReconnectionTime = TimeSpan.FromSeconds(3);

        private readonly RestClient _restClient;
        private readonly FeatureflowConfig _config;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private bool disposedValue = false; // To detect redundant calls
        private bool _started;
        private string _lastEventId;
        private TimeSpan _reconnectionTime = DefaultReconnectionTime;

        internal SseClient(RestClient restClient, FeatureflowConfig config)
        {
            _restClient = restClient;
            _config = config;
        }

        internal event EventHandler<Message> MessageReceived;

        internal event EventHandler<Disconnect> Disconnected;

        internal void Start()
        {
            if (!_started)
            {
                _started = true;
                Task.Run(() => ReadStream(), _cts.Token);
            }
        }

        private async void ReadStream()
        {
            var uri = new Uri(_config.StreamBaseUri, FeatureflowConfig.StreamFeaturesRestPath);

            try
            {
                using (var client = _restClient.CreateHttpClient())
                using (var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, _cts.Token))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Client returned non-success code");
                    }

                    if (response.Headers.TryGetValues("content-type", out IEnumerable<string> contentTypes) && contentTypes != null)
                    {
                        if (!contentTypes.Contains("text/event-stream"))
                        {
                            throw new Exception("Server does not support SSE");
                        }
                    }

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(stream))
                    {
                        string eventType = null;
                        string eventId = null;
                        StringBuilder sb = new StringBuilder();

                        while (true)
                        {
                            if (_cts.IsCancellationRequested)
                            {
                                break;
                            }

                            var s = await reader.ReadLineAsync();

                            if (s == null)
                            {
                                continue;
                            }

                            /*
                             * https://www.w3.org/TR/eventsource/#event-stream-interpretation
                             * Lines must be processed, in the order they are received, as follows:
                             *   If the line is empty (a blank line)
                             *     Dispatch the event, as defined below.
                             *   If the line starts with a U+003A COLON character (:)
                             *     Ignore the line.
                             *   If the line contains a U+003A COLON character (:)
                             *     Collect the characters on the line before the first U+003A COLON character (:), and let field be that string.
                             *     Collect the characters on the line after the first U+003A COLON character (:), and let value be that string. If value starts with a U+0020 SPACE character, remove it from value.
                             *     Process the field using the steps described below, using field as the field name and value as the field value.
                             *   Otherwise, the string is not empty but does not contain a U+003A COLON character (:)
                             *     Process the field using the steps described below, using the whole line as the field name, and the empty string as the field value.
                             */

                            if (s == string.Empty)
                            {
                                if (!_cts.IsCancellationRequested)
                                {
                                    MessageReceived?.Invoke(this, new Message(eventType ?? "message", sb.ToString()));
                                }

                                eventType = null;
                                eventId = null;
                                sb.Clear();
                            }
                            else if (s.StartsWith(":"))
                            {
                                // Ignore the line.
                            }
                            else
                            {
                                var sepPos = s.IndexOf(':');
                                string field = null;
                                string value = null;
                                if (sepPos != -1)
                                {
                                    field = s.Substring(0, sepPos);
                                    value = s.Substring(sepPos);
                                    if (value.StartsWith(" "))
                                    {
                                        value = value.Substring(1);
                                    }
                                }
                                else
                                {
                                    field = s;
                                    value = string.Empty;
                                }

                                switch (field)
                                {
                                    case "event":
                                        eventType = value;
                                        break;

                                    case "data":
                                        sb.Append(value);
                                        sb.Append('\u000A');
                                        break;

                                    case "id":
                                        eventId = _lastEventId = value;
                                        break;

                                    case "retry":
                                        if (int.TryParse(value, out int ms))
                                        {
                                            _reconnectionTime = TimeSpan.FromMilliseconds(ms);
                                        }

                                        break;
                                }
                            }
                        }

                        if (!_cts.IsCancellationRequested)
                        {
                            Disconnected?.Invoke(this, new Disconnect(DefaultReconnectionTime, null));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Disconnected?.Invoke(this, new Disconnect(DefaultReconnectionTime, ex));
            }

            _started = false;
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _cts.Cancel();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

        internal class Message
        {
            internal Message(string eventType, string value)
            {
                EventType = eventType;
                Value = value;
            }

            internal string EventType { get; }

            internal string Value { get; }
        }

        internal class Disconnect
        {
            internal Disconnect(TimeSpan waitTime, Exception exception)
            {
                WaitTime = waitTime;
                Exception = exception;
            }

            internal TimeSpan WaitTime { get; }

            internal Exception Exception { get; }
        }
    }
}
