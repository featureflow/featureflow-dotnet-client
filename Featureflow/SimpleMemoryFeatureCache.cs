using System.Collections.Concurrent;
using System.Collections.Generic;
//using Microsoft.Extensions.Logging;

namespace Featureflow.Client
{
    public class SimpleMemoryFeatureCache : IFeatureControlCache
    
    {
        
        
        //private static readonly ILogger Logger = ApplicationLogging.CreateLogger<SimpleMemoryFeatureCache>();                        
        private ConcurrentDictionary<string, FeatureControl> _controls;              
        private bool _initialized;

        public void Init(IDictionary<string, FeatureControl> controls)
        {
            this._controls = new ConcurrentDictionary<string, FeatureControl>(controls);
            _initialized = true;
        }

        public FeatureControl Get(string key)
        {
            _controls.TryGetValue(key, out var control);            
            return control;
        }        

        public void Set(FeatureControl featureControl)
        {
            _controls.TryAdd(featureControl.Key, featureControl);
        }

        public void Delete(string key)
        {
            if (_controls.TryGetValue(key, out var control))
            {
                _controls.TryRemove(key, out control);    
            }            
        }

        public bool Initialised()
        {
            return _initialized;
        }
    }
}