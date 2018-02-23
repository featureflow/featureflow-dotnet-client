using System;

namespace Featureflow.Client
{
    public class FeatureflowConfig
    {
        public Uri BaseUri { get; internal set; } = new Uri("https://app.featureflow.io");
        public Uri StreamBaseUri { get; internal set; } = new Uri("https://rtm.featureflow.io");
        public bool Offline { get; internal set; } 
        
    }
}