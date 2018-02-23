using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Featureflow.Client
{
    public class SimpleMemoryFeatureCache : IFeatureControlCache
    
    {
        
        
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<SimpleMemoryFeatureCache>();                        
        private ConcurrentDictionary<string, FeatureControl> controls;              
        private bool initialized;

        public void Init(IDictionary<string, FeatureControl> controls)
        {
            this.controls = new ConcurrentDictionary<string, FeatureControl>(controls);
            initialized = true;
        }

        public FeatureControl Get(string key)
        {
            controls.TryGetValue(key, out var control);            
            return control;
        }        

        public void Set(FeatureControl featureControl)
        {
            controls.TryAdd(featureControl.Key, featureControl);
        }

        public void Delete(string key)
        {
            if (controls.TryGetValue(key, out var control))
            {
                controls.TryRemove(key, out control);    
            }            
        }

        public bool Initialised()
        {
            return initialized;
        }
    }
}