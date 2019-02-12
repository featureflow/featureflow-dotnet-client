using System;

namespace Featureflow.Client
{
    public class FeatureflowConfig
    {
        internal static readonly TimeSpan DefaultConnectionTimeout = TimeSpan.FromSeconds(30);
        internal static readonly Uri DefaultBaseUri = new Uri("https://app.featureflow.io");
        internal static readonly Uri DefaultStreamBaseUri = new Uri("https://rtm.featureflow.io");
        internal static readonly Uri DefaultEventsBaseUri = new Uri("https://events.featureflow.io");

        internal static readonly string FeaturesRestPath = "/api/sdk/v1/features";
        internal static readonly string EventsRestPath = "/api/sdk/v1/events";
        internal static readonly string StreamFeaturesRestPath = "/api/sdk/v1/features";

        public Uri BaseUri { get; internal set; } = DefaultStreamBaseUri;

        public Uri StreamBaseUri { get; internal set; } = DefaultStreamBaseUri;

        public Uri EventsBaseUri { get; internal set; } = DefaultEventsBaseUri;

        public TimeSpan ConnectionTimeout { get; internal set; } = DefaultConnectionTimeout;

        public bool Offline { get; internal set; }

        public GetFeaturesMethod GetFeaturesMethod { get; internal set; } = GetFeaturesMethod.Sse;
    }
}
