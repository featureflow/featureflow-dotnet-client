
using System.Collections.Generic;
//using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Featureflow.Client
{
    public class FeatureflowClient : IFeatureflowClient
    {
	    //private static readonly ILogger Logger = ApplicationLogging.CreateLogger<FeatureflowClient>();
	    private readonly FeatureflowConfig _config; //config	   	  
	    
	    private readonly IFeatureControlCache _featureControlCache; //local cache
	    private readonly IFeatureControlClient _featureControlClient; //retrieves features	    
	    private readonly RestClient _restClient;
	    private readonly Dictionary<string, Feature> _featureDefaults = new Dictionary<string, Feature>();
	    
	    
	    
	    public FeatureflowClient(string apiKey) : this(apiKey, new List<Feature>(), new FeatureflowConfig())
	    {
		    
	    }

	    public FeatureflowClient(string apiKey, List<Feature> features) : this(apiKey, features, new FeatureflowConfig())
	    {
		    
	    }
	    
	    public FeatureflowClient(string apiKey, List<Feature> features, FeatureflowConfig config){
	        //Logger.LogInformation("Initialising Featureflow...");
		    features?.ForEach(feature => _featureDefaults[feature.Key] = feature);
		    _featureControlCache = new SimpleMemoryFeatureCache();

		    if (config.Offline)
		    {
			    //Logger.LogWarning("Featureflow is in Offline mode. Registered defaults will be used.");			    
			    return;
		    }
		    
		    var restConfig = new RestConfig
		    {
			    sdkVersion = ((AssemblyInformationalVersionAttribute) typeof(FeatureflowClient)
				    .GetTypeInfo().Assembly.GetCustomAttribute(typeof(AssemblyInformationalVersionAttribute))).InformationalVersion
		    };


		    _restClient = new RestClient(apiKey, config, restConfig);
	        //register feature coded failover values	
	        
	        //start the featureControl Client
	        _featureControlClient = new PollingClient(config, _featureControlCache, _restClient);
		    		    		    
	        var initTask = _featureControlClient.Init(); //initialise  
	        var unused = initTask.Task.Wait(_config.ConnectionTimeout); //wait	        

        }
	
	    public Evaluate Evaluate(string featureKey, User user)
	    {
		    var featureControl = _featureControlCache.Get(featureKey);		    
		    var failoverVariant = "off";
		    if (_featureDefaults.TryGetValue(featureKey, out var failoverFeature))
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
