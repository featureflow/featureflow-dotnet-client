using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Featureflow.Client
{
    
    public class User
    {
        public User()
        {
        }

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
            return new User {Id = "ANONYMOUS"};            
        }
        
    }
}