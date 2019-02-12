using System;
using Newtonsoft.Json;

namespace Featureflow.Client
{
    public class Event
    {
        [JsonIgnore]
        public string EventType { get; set; }

        [JsonProperty("featureKey")]
        public string FeatureKey { get; set; }

        [JsonProperty("evaluatedVariant")]
        public string EvaluatedVariant { get; set; }

        [JsonProperty("expectedVariant")]
        public string ExpectedVariant { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }
    }
}
