namespace Featureflow.Client
{
    public interface IFeatureflowClient
    {
        Evaluate Evaluate(string featureKey, User user);

        Evaluate Evaluate(string featureKey);
    }
}