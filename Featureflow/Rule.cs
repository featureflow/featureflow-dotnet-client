using System.Collections.Generic;
using Newtonsoft.Json;

namespace Featureflow.Client
{
    public class Rule
    {
        [JsonProperty("audience")]
        public Audience Audience { get; set; }

        [JsonProperty("variantSplits")]
        public List<VariantSplit> VariantSplits { get; set; }
    }
}