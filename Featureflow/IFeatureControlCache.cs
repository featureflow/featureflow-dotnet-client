using System.Collections.Generic;

namespace Featureflow.Client
{
    public interface IFeatureControlCache
    
    {
        void Init(IDictionary<string, FeatureControl> controls);
        FeatureControl Get(string key);
        void Set(FeatureControl control);
        void Delete(string key);
        bool Initialised();
    }
}