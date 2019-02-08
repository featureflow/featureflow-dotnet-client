using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Featureflow.Client
{
    public class Audience
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("conditions")]
        public List<Condition> Conditions { get; set; } = new List<Condition>();
    }
}