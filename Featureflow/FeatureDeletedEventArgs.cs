using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Featureflow.Client
{
    public class FeatureDeletedEventArgs
    {
        internal FeatureDeletedEventArgs(string featureKey)
        {
            FeatureKey = featureKey;
        }

        public string FeatureKey { get; }
    }
}
