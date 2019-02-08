using System;

namespace Featureflow.Client
{
    public class FeatureflowConfig
    {
        public Uri BaseUri { get; internal set; } = DefaultStreamBaseUri;
        public Uri StreamBaseUri { get; internal set; } = DefaultStreamBaseUri;
        public TimeSpan ConnectionTimeout { get; internal set; } = DefaultConnectionTimeout;
        public bool Offline { get; internal set; }

        internal static readonly TimeSpan DefaultConnectionTimeout = TimeSpan.FromSeconds(30);
        internal static readonly Uri DefaultBaseUri = new Uri("https://app.featureflow.io");
        internal static readonly Uri DefaultStreamBaseUri = new Uri("https://rtm.featureflow.io");
    }
}