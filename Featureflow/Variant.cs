using System;

namespace Featureflow.Client
{
    public class Variant
    {
        public static readonly string off = "off";
        public static readonly string on = "on";
        public string key;
        public string name;
        
        public Variant(string key, string name)
        {
            this.key = key;
            this.name = name;
        }
    }
}