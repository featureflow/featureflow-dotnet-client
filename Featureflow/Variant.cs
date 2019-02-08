using System;

namespace Featureflow.Client
{
    internal class Variant
    {
        public static readonly string Off = "off";
        public static readonly string On = "on";

        private readonly string key;
        private readonly string name;
        
        public Variant(string key, string name)
        {
            this.key = key;
            this.name = name;
        }
    }
}