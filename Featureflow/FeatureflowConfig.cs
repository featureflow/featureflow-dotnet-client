using System;

namespace Featureflow.Client
{
    public class FeatureflowConfig
    {
        internal static readonly TimeSpan DefaultConnectionTimeout = TimeSpan.FromSeconds(30);
        internal static readonly Uri DefaultBaseUri = new Uri("https://app.featureflow.io");
        internal static readonly Uri DefaultStreamBaseUri = new Uri("https://rtm.featureflow.io");

        internal static readonly string FeaturesRestPath = "/api/sdk/v1/features";
        internal static readonly string EventsRestPath = "/api/sdk/v1/events";

        public Uri BaseUri { get; internal set; } = DefaultStreamBaseUri;

        public Uri StreamBaseUri { get; internal set; } = DefaultStreamBaseUri;

        public TimeSpan ConnectionTimeout { get; internal set; } = DefaultConnectionTimeout;

        public bool Offline { get; internal set; }
    }
}