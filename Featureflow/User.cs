
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Featureflow.Client
{
    
    public class User
    {
        public User(string id)
        {
            Id = id;
        }

        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("attributes")]
        public Dictionary<string, List<object>> Attributes { get; set; } = new Dictionary<string, List<object>>();
        [JsonProperty("sessionAttributes")]
        public Dictionary<string, List<object>> SessionAttributes { get; set; } = new Dictionary<string, List<object>>();
        [JsonProperty("bucketKey")]
        public string BucketKey { get; set; }
        
        public static User Anonymous()
        {
            return new User("ANONYMOUS");            
        }

        public void WithAttribute(string key, object value)
        {
            Attributes.Add(key, new List<object> {value});
        }
        public void WithAttribute(string key, List<object> values)
        {
            Attributes.Add(key, values);
        }
        public void WithSessionAttribute(string key, object value)
        {
            SessionAttributes.Add(key, new List<object> {value});
        }
        public void WithSessionAttribute(string key, List<object> values)
        {
            SessionAttributes.Add(key, values);
        }
        
    }
}