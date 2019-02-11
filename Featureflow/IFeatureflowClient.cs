using System;
using System.Collections.Generic;

namespace Featureflow.Client
{
    public delegate void FeatureUpdatedEventHandler(IFeatureflowClient sender, FeatureUpdatedEventArgs args);

    public delegate void FeatureDeletedEventHandler(IFeatureflowClient sender, FeatureDeletedEventArgs args);

    public interface IFeatureflowClient
        : IDisposable
    {
        event FeatureUpdatedEventHandler FeatureUpdated;

        event FeatureDeletedEventHandler FeatureDeleted;

        Evaluate Evaluate(string featureKey);

        Evaluate Evaluate(string featureKey, User user);

        Dictionary<string, Evaluate> EvaluateAll();

        Dictionary<string, Evaluate> EvaluateAll(User user);
    }
}