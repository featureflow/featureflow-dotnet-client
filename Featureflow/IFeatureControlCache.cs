using System.Collections.Generic;

namespace Featureflow.Client
{
    internal interface IFeatureControlCache
    {
        void Update(IDictionary<string, FeatureControl> controls);

        FeatureControl Get(string key);

        void Set(FeatureControl control);

        void Delete(string key);
    }
}