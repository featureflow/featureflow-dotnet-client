using System;
using System.Text.RegularExpressions;

namespace Featureflow.Client
{
    public class FeatureflowConfig
    {
        internal static readonly TimeSpan DefaultConnectionTimeout = TimeSpan.FromSeconds(30);
        internal static readonly Uri DefaultBaseUri = new Uri("https://app.featureflow.io");
        internal static readonly Uri DefaultStreamBaseUri = new Uri("https://rtm.featureflow.io");
        internal static readonly Uri DefaultEventsBaseUri = new Uri("https://events.featureflow.io");

        // Contract slug (featureflow-client-sdk-testbed/CONTRACT.md): lowercase [a-z0-9._-],
        // max 64 chars. The server sanitises defensively; the SDK validates strictly so a typo
        // is a visible warning here rather than a silently mangled tag there.
        internal static readonly Regex ApplicationPattern = new Regex("^[a-z0-9._-]{1,64}$");

        internal static readonly string FeaturesRestPath = "/api/sdk/v1/features";
        internal static readonly string EventsRestPath = "/api/sdk/v1/events";
        internal static readonly string StreamFeaturesRestPath = "/api/sdk/v1/features";

        // BaseUri is the REST host the features endpoint is polled from; StreamBaseUri is the
        // (currently unserved) SSE host. Defaulting this to the stream host made a directly-constructed
        // config - the form the README documents - point GetFeaturesMethod.Polling at a host that does
        // not serve the polling endpoint. The default now matches FeatureflowConfigBuilder, so a config
        // built either way polls the right host.
        public Uri BaseUri { get; internal set; } = DefaultBaseUri;

        public Uri StreamBaseUri { get; internal set; } = DefaultStreamBaseUri;

        public Uri EventsBaseUri { get; internal set; } = DefaultEventsBaseUri;

        public TimeSpan ConnectionTimeout { get; internal set; } = DefaultConnectionTimeout;

        public bool Offline { get; internal set; }

        // Polling, as in every other Featureflow SDK. The Featureflow service does not currently
        // serve the SSE stream; GetFeaturesMethod.Sse remains for API compatibility only.
        public GetFeaturesMethod GetFeaturesMethod { get; internal set; } = GetFeaturesMethod.Polling;

        /// <summary>
        /// Sanitised application tag sent as X-Featureflow-Application on every request so the
        /// Featureflow dashboard can attribute SDK usage to this workload; null = send no tag.
        /// Defaults to the FEATUREFLOW_APPLICATION environment variable; set it explicitly with
        /// <see cref="FeatureflowConfigBuilder.WithApplication(string)"/>.
        /// </summary>
        public string Application { get; internal set; } = SanitiseApplication(Environment.GetEnvironmentVariable("FEATUREFLOW_APPLICATION"));

        /// <summary>
        /// Validates an application tag. Case is forgiven (lowercased); anything else invalid is
        /// dropped with a warning and no X-Featureflow-Application header is sent.
        /// </summary>
        internal static string SanitiseApplication(string raw)
        {
            if (string.IsNullOrEmpty(raw))
            {
                return null;
            }

            var value = raw.Trim().ToLowerInvariant();
            if (!ApplicationPattern.IsMatch(value))
            {
                Console.Error.WriteLine($"Featureflow: ignoring application \"{raw}\" - must be lowercase a-z, 0-9, dot, underscore or hyphen, max 64 chars");
                return null;
            }

            return value;
        }
    }
}
