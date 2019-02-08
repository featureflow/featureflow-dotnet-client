using System;
using System.Collections.Generic;

namespace Featureflow.Client
{
    internal class SimpleMemoryFeatureCache : IFeatureControlCache
    {
        // private static readonly ILogger Logger = ApplicationLogging.CreateLogger<SimpleMemoryFeatureCache>();
        private readonly object _guard = new object();
        private Dictionary<string, FeatureControl> _controls;

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
                if (controls != null)
                {
                    _controls = new Dictionary<string, FeatureControl>(controls);
                }
                else
                {
                    _controls.Clear();
                }
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