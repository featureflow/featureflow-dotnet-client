using System.Collections.Concurrent;
using System.Collections.Generic;
//using Microsoft.Extensions.Logging;

namespace Featureflow.Client
{
    class SimpleMemoryFeatureCache : IFeatureControlCache
    {
        //private static readonly ILogger Logger = ApplicationLogging.CreateLogger<SimpleMemoryFeatureCache>();
        private Dictionary<string, FeatureControl> _controls;
        private readonly object _guard = new object();

        internal SimpleMemoryFeatureCache()
            : this(new Dictionary<string, FeatureControl>())
        {
        }

        internal SimpleMemoryFeatureCache(Dictionary<string, FeatureControl> controls)
        {
            _controls = controls;
        }

        public void Update(IDictionary<string, FeatureControl> controls)
        {
            lock (_guard)
            {
                _controls = new Dictionary<string, FeatureControl>(controls);
            }
        }

        public FeatureControl Get(string key)
        {
            lock (_guard)
            {
                _controls.TryGetValue(key, out var control);
                return control;
            }
        }

        public void Set(FeatureControl featureControl)
        {
            lock (_guard)
            {
                _controls.Add(featureControl.Key, featureControl);
            }
        }

        public void Delete(string key)
        {
            lock (_guard)
            {
                _controls.Remove(key);
            }
        }
    }
}