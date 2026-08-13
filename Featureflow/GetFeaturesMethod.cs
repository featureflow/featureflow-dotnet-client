using System;

namespace Featureflow.Client
{
    public enum GetFeaturesMethod
    {
        /// <summary>
        /// Feature updates via periodic polling of the server. This is the default.
        /// </summary>
        Polling,

        /// <summary>
        /// Feature updates via server-sent events. Not currently served by the Featureflow
        /// service — retained for API compatibility; use Polling.
        /// </summary>
        Sse,
    }
}
