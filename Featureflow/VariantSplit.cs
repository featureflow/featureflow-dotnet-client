using Newtonsoft.Json;

namespace Featureflow.Client
{
    public class VariantSplit
    {        
        [JsonProperty("variantKey")]
        public string VariantKey { get; set; }
        [JsonProperty("split")]
        public int Split { get; set; }                
    }
}