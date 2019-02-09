using System.Collections.Generic;

namespace Featureflow.Client
{
    public interface IFeatureflowClient
    {
        Evaluate Evaluate(string featureKey, User user);

        Evaluate Evaluate(string featureKey);

        Dictionary<string, Evaluate> EvaluateAll();

        Dictionary<string, Evaluate> EvaluateAll(User user);
    }
}