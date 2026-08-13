using System;
using System.Net.Http;

namespace Featureflow.Client
{
    internal class RestConfig
    {
        internal string SdkVersion { get; set; }

        // Test seam: when set, HttpClients are created over this handler (not disposed with the
        // client) instead of the default network stack. Null in production.
        internal HttpMessageHandler HttpMessageHandler { get; set; }
    }
}