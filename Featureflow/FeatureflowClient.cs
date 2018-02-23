
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Featureflow.Client
{
    public class FeatureflowClient
    {
	    private static readonly ILogger Logger = ApplicationLogging.CreateLogger<FeatureflowClient>();
	    private readonly FeatureflowConfig _config; //config	   	  
	    
	    private readonly IFeatureControlCache featureControlCache; //local cache
	    private readonly IFeatureControlClient featureControlClient; //retrieves features	    
	    private readonly RestClient restClient;
	    private readonly Dictionary<string, Feature> featureDefaults;
	    
	    
	    
	    public FeatureflowClient(string apiKey) : this(apiKey, new List<Feature>(), new FeatureflowConfig())
	    {
		    
	    }

	    public FeatureflowClient(string apiKey, List<Feature> features) : this(apiKey, features, new FeatureflowConfig())
	    {
		    
	    }
	    
	    public FeatureflowClient(string apiKey, List<Feature> features, FeatureflowConfig config){
	        Logger.LogInformation("Initialising Featureflow...");
		    features?.ForEach(feature => featureDefaults[feature.Key] = feature);
		    featureControlCache = new SimpleMemoryFeatureCache();

		    if (config.Offline)
		    {
			    Logger.LogWarning("Featureflow is in Offline mode. Registered defaults will be used.");			    
			    return;
		    }
		    
		    var restConfig = new RestConfig
		    {
			    sdkVersion = ((AssemblyInformationalVersionAttribute) typeof(FeatureflowClient)
				    .GetTypeInfo().Assembly.GetCustomAttribute(typeof(AssemblyInformationalVersionAttribute))).InformationalVersion
		    };


		    restClient = new RestClient(apiKey, config, restConfig);
	        //register feature coded failover values	
	        
	        //start the featureControl Client
	        featureControlClient = new PollingClient(config, featureControlCache, restClient);
		    		    		    
	        var initTask = featureControlClient.Init(); //initialise  
	        var unused = initTask.Task.Wait(300000); //wait	        

        }
	
	    public Evaluate Evaluate(string featureKey, User user)
	    {
		    var featureControl = featureControlCache.Get(featureKey);		    
		    var failoverVariant = "off";
		    if (featureDefaults.TryGetValue(featureKey, out var failoverFeature))
			{
				failoverVariant = failoverFeature.FailoverVariant;
			}		    	    
		    var e = new Evaluate(featureControl, user, failoverVariant);
		    return e;
	    }

	    public Evaluate Evaluate(string featureKey)
	    {
		    return Evaluate(featureKey, User.Anonymous());		    
	    }
    }
}
