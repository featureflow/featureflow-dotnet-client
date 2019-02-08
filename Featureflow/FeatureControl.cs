using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Featureflow.Client
{
    public class FeatureControl
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("offVariantKey")]
        public string OffVariantKey { get; set; } = "off";

        [JsonProperty("rules")]
        public List<Rule> Rules { get; set; } = new List<Rule>();
    }
}