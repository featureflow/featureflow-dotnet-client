using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Featureflow.Client
{
    public class FeatureControl
    {
        [JsonProperty ("enabled")] public bool Enabled { get; set; } 
        [JsonProperty ("key")]     public string Key{ get; set; }
        [JsonProperty("offVariantKey")] public string OffVariantKey { get; set; } = "off";
        [JsonProperty ("rules")]   public List<Rule> Rules { get; set; } = new List<Rule>();

        /*
      
        public string evaluate(FeatureflowUser user) {
            //if off then offVariant
            if(!enabled) {
                return offVariantKey;
            }
            //if we have rules (we should always have at least one - the default rule
            foreach (FeatureControl faeture in fe rule in rules)            
                if(rule.matches(user)){
                    //if the rule matches then pass back the variant based on the split evaluation
                    //return //getVariantByKey(rule.getVariantSplitKey(context.key, variationsSeed)).key;
                    return rule.getVariantSplitKey(user.Key, this.key, salt);
                }
            }
            return null; //at least the default rule above should have matched, if not, return null to invoke using the failover rule
        }*/
    }
}