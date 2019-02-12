using System;

namespace Featureflow.Client
{
    public enum GetFeaturesMethod
    {
        /// <summary>
        /// Features updates with periodic polling from server.
        /// </summary>
        Polling,

        /// <summary>
        /// Features updates using server-side events.
        /// It is default behavior
        /// </summary>
        Sse,
    }
}
