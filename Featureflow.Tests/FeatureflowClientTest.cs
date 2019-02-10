using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using Xunit;
using Featureflow.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Featureflow.Tests
{
    public class FeatureflowClientTest
    {
	    private const string features = "{\"new-one\":{\"id\":\"598c38cdb7801d000952f798\",\"key\":\"new-one\",\"featureId\":\"598c38cdb7801d000952f797\",\"environmentId\":\"598947c33cb2f2000ac1a636\",\"projectId\":\"598947c33cb2f2000ac1a634\",\"organisationId\":\"5816b146a377640007a89555\",\"archived\":false,\"status\":\"unavailable\",\"hitsToday\":0,\"deleted\":false,\"rules\":[{\"priority\":0,\"defaultRule\":false,\"audience\":{\"id\":null,\"name\":null,\"conditions\":[{\"target\":\"featureflow.user.id\",\"operator\":\"equals\",\"values\":[\"oliver\"]}]},\"variantSplits\":[{\"variantKey\":\"on\",\"split\":100},{\"variantKey\":\"off\",\"split\":0}],\"description\":null},{\"priority\":1,\"defaultRule\":true,\"audience\":null,\"variantSplits\":[{\"variantKey\":\"on\",\"split\":0},{\"variantKey\":\"off\",\"split\":100}],\"description\":null}],\"statistics\":{\"createdDate\":\"2017-09-01T07:00:13.619Z\",\"lastModifiedDate\":\"2017-09-01T07:00:13.635Z\",\"impressions\":{\"today\":0},\"uniqueImpressions\":{},\"mostActiveVariant\":{\"today\":null},\"variants\":{\"off\":{\"impressions\":{\"today\":0},\"uniqueImpressions\":{}},\"on\":{\"impressions\":{\"today\":0},\"uniqueImpressions\":{}}}},\"offVariantKey\":\"off\",\"inClientApi\":true,\"enabled\":true,\"permanent\":false,\"staleDays\":5,\"url\":\"/projects/598947c33cb2f2000ac1a634/\",\"stale\":false,\"entityName\":\"new-one\",\"entityType\":\"feature control\"},\"standard-login\":{\"id\":\"5994de796036fa000a7daa10\",\"key\":\"standard-login\",\"featureId\":\"5994de796036fa000a7daa0f\",\"environmentId\":\"598947c33cb2f2000ac1a636\",\"projectId\":\"598947c33cb2f2000ac1a634\",\"organisationId\":\"5816b146a377640007a89555\",\"archived\":false,\"status\":\"unavailable\",\"hitsToday\":0,\"deleted\":false,\"rules\":[{\"priority\":0,\"defaultRule\":true,\"audience\":null,\"variantSplits\":[{\"variantKey\":\"off\",\"split\":0},{\"variantKey\":\"on\",\"split\":100}],\"description\":\"\"}],\"statistics\":null,\"offVariantKey\":\"off\",\"inClientApi\":true,\"enabled\":false,\"permanent\":false,\"staleDays\":5,\"url\":\"/projects/598947c33cb2f2000ac1a634/\",\"stale\":false,\"entityName\":\"standard-login\",\"entityType\":\"feature control\"},\"dave\":{\"id\":\"59f102df9ab66c000b8247a5\",\"key\":\"dave\",\"featureId\":\"59f102df9ab66c000b8247a4\",\"environmentId\":\"598947c33cb2f2000ac1a636\",\"projectId\":\"598947c33cb2f2000ac1a634\",\"organisationId\":\"5816b146a377640007a89555\",\"archived\":false,\"status\":\"unavailable\",\"hitsToday\":0,\"deleted\":false,\"rules\":[{\"priority\":0,\"defaultRule\":true,\"audience\":null,\"variantSplits\":[{\"variantKey\":\"off\",\"split\":0},{\"variantKey\":\"on\",\"split\":100}],\"description\":\"\"}],\"statistics\":{\"impressions\":{},\"uniqueImpressions\":{},\"mostActiveVariant\":{},\"variants\":{}},\"offVariantKey\":\"off\",\"inClientApi\":true,\"enabled\":false,\"permanent\":false,\"staleDays\":5,\"url\":\"/projects/598947c33cb2f2000ac1a634/\",\"stale\":false,\"entityName\":\"dave\",\"entityType\":\"feature control\"},\"example-feature\":{\"id\":\"598947c33cb2f2000ac1a63e\",\"key\":\"example-feature\",\"featureId\":\"598947c33cb2f2000ac1a63c\",\"environmentId\":\"598947c33cb2f2000ac1a636\",\"projectId\":\"598947c33cb2f2000ac1a634\",\"organisationId\":\"5816b146a377640007a89555\",\"archived\":false,\"status\":\"available\",\"hitsToday\":0,\"deleted\":false,\"rules\":[{\"priority\":0,\"defaultRule\":false,\"audience\":{\"id\":null,\"name\":null,\"conditions\":[{\"target\":\"age\",\"operator\":\"greaterThan\",\"values\":[21]}]},\"variantSplits\":[{\"variantKey\":\"off\",\"split\":33},{\"variantKey\":\"on\",\"split\":49},{\"variantKey\":\"red\",\"split\":8},{\"variantKey\":\"blue\",\"split\":10}],\"description\":null},{\"priority\":1,\"defaultRule\":false,\"audience\":{\"id\":null,\"name\":null,\"conditions\":[{\"target\":\"featureflow.date\",\"operator\":\"before\",\"values\":[\"2018-02-19T13:00:00.000Z\"]}]},\"variantSplits\":[{\"variantKey\":\"off\",\"split\":0},{\"variantKey\":\"on\",\"split\":0},{\"variantKey\":\"red\",\"split\":100},{\"variantKey\":\"blue\",\"split\":0}],\"description\":null},{\"priority\":2,\"defaultRule\":false,\"audience\":{\"id\":null,\"name\":null,\"conditions\":[{\"target\":\"featureflow.user.id\",\"operator\":\"contains\",\"values\":[\"oliver\"]}]},\"variantSplits\":[{\"variantKey\":\"off\",\"split\":0},{\"variantKey\":\"on\",\"split\":0},{\"variantKey\":\"red\",\"split\":0},{\"variantKey\":\"blue\",\"split\":100}],\"description\":null},{\"priority\":3,\"defaultRule\":true,\"audience\":null,\"variantSplits\":[{\"variantKey\":\"off\",\"split\":0},{\"variantKey\":\"on\",\"split\":100},{\"variantKey\":\"red\",\"split\":0},{\"variantKey\":\"blue\",\"split\":0}],\"description\":null}],\"statistics\":{\"createdDate\":\"2018-02-22T05:50:50.627Z\",\"lastModifiedDate\":\"2018-02-22T05:50:50.642Z\",\"impressions\":{\"all\":14298,\"today\":0},\"uniqueImpressions\":{},\"mostActiveVariant\":{\"today\":null},\"variants\":{\"red\":{\"impressions\":{\"today\":0},\"uniqueImpressions\":{}},\"blue\":{\"impressions\":{\"today\":0},\"uniqueImpressions\":{}},\"off\":{\"impressions\":{\"today\":0},\"uniqueImpressions\":{}},\"on\":{\"impressions\":{\"today\":0},\"uniqueImpressions\":{}}}},\"offVariantKey\":\"off\",\"inClientApi\":true,\"enabled\":true,\"permanent\":false,\"staleDays\":5,\"url\":\"/projects/598947c33cb2f2000ac1a634/\",\"stale\":false,\"entityName\":\"example-feature\",\"entityType\":\"feature control\"}}";
	    	    
	    
		//[Fact]
		public void FeatureflowManualTest()
		{
			/*FeatureflowConfig config = FeatureflowConfig.Create();
			config.BaseUri = "http://app.featureflow.localdev";*/
			Console.WriteLine("Starting out");
			var client = new FeatureflowClient("srv-env-685e066dea464f88be14effbf65cf69c"); //
			Console.WriteLine("Created client");
			User user = new User("1234");
			user.Attributes.Add("age", new List<object> {11L});
			
			var result = client.Evaluate("example-feature", user).Value();
			Console.WriteLine(result);
		}

	    [Fact]
	    public void FeatureflowManualTestWithFeatureDefaults()
	    {
		    /*FeatureflowConfig config = FeatureflowConfig.Create();
		    config.BaseUri = "http://app.featureflow.localdev";*/
		    Console.WriteLine("Creating client");

		    var client = new FeatureflowClient("srv-env-685e066dea464f88be14effbf65cf69c", new List<Feature>
		    {
			    new Feature
			    {
				    Key = "unknown",
				    FailoverVariant = "unavailable"
			    },
			    new Feature
			    {
				    Key = "new-one",
				    FailoverVariant = "green"
			    },
			    new Feature
			    {
			 	   Key = "example-feature",
			    	FailoverVariant = "off"
		    	}
		    }, new FeatureflowConfig());		    
		    var user = new User("1234");
		    user.WithAttribute("region", "sydney");
		    user.WithAttribute("days", new List<object> {11L, 1L, 4L, 29L});
		    user.WithSessionAttribute("dayofweek", 11L);
		    
		    var result = client.Evaluate("example-feature", user).Value();
		    if (client.Evaluate("example-feature", user).IsOn())
		    {
			    //do something
			    Console.WriteLine("Example feature is on");
		    }
		    else
		    {
			    //do not do something
			    Console.WriteLine("Example feature is off");
		    }
		    var result2 = client.Evaluate("unknown", user).Value();
		    Console.WriteLine("example feature: " + result);
		    Console.WriteLine("unknown feature: " + result2);
		    
		    Assert.Equal("off", result);	
		    Assert.Equal("unavailable", result2);	
	    }


	    [Fact]
	    public void testJsonDeserialize()
	    {
		    const string result = "{\"new-one\":{\"id\":\"598c38cdb7801d000952f798\",\"key\":\"new-one\",\"featureId\":\"598c38cdb7801d000952f797\",\"environmentId\":\"598947c33cb2f2000ac1a636\",\"projectId\":\"598947c33cb2f2000ac1a634\",\"organisationId\":\"5816b146a377640007a89555\",\"archived\":false,\"status\":\"unavailable\",\"hitsToday\":0,\"deleted\":false,\"rules\":[{\"priority\":0,\"defaultRule\":false,\"audience\":{\"id\":null,\"name\":null,\"conditions\":[{\"target\":\"featureflow.user.id\",\"operator\":\"equals\",\"values\":[\"oliver\"]}]},\"variantSplits\":[{\"variantKey\":\"on\",\"split\":100},{\"variantKey\":\"off\",\"split\":0}],\"description\":null},{\"priority\":1,\"defaultRule\":true,\"audience\":null,\"variantSplits\":[{\"variantKey\":\"on\",\"split\":0},{\"variantKey\":\"off\",\"split\":100}],\"description\":null}],\"statistics\":{\"createdDate\":\"2017-09-01T07:00:13.619Z\",\"lastModifiedDate\":\"2017-09-01T07:00:13.635Z\",\"impressions\":{\"today\":0},\"uniqueImpressions\":{},\"mostActiveVariant\":{\"today\":null},\"variants\":{\"off\":{\"impressions\":{\"today\":0},\"uniqueImpressions\":{}},\"on\":{\"impressions\":{\"today\":0},\"uniqueImpressions\":{}}}},\"offVariantKey\":\"off\",\"inClientApi\":true,\"enabled\":true,\"permanent\":false,\"staleDays\":5,\"url\":\"/projects/598947c33cb2f2000ac1a634/\",\"stale\":false,\"entityName\":\"new-one\",\"entityType\":\"feature control\"},\"standard-login\":{\"id\":\"5994de796036fa000a7daa10\",\"key\":\"standard-login\",\"featureId\":\"5994de796036fa000a7daa0f\",\"environmentId\":\"598947c33cb2f2000ac1a636\",\"projectId\":\"598947c33cb2f2000ac1a634\",\"organisationId\":\"5816b146a377640007a89555\",\"archived\":false,\"status\":\"unavailable\",\"hitsToday\":0,\"deleted\":false,\"rules\":[{\"priority\":0,\"defaultRule\":true,\"audience\":null,\"variantSplits\":[{\"variantKey\":\"off\",\"split\":0},{\"variantKey\":\"on\",\"split\":100}],\"description\":\"\"}],\"statistics\":null,\"offVariantKey\":\"off\",\"inClientApi\":true,\"enabled\":false,\"permanent\":false,\"staleDays\":5,\"url\":\"/projects/598947c33cb2f2000ac1a634/\",\"stale\":false,\"entityName\":\"standard-login\",\"entityType\":\"feature control\"},\"dave\":{\"id\":\"59f102df9ab66c000b8247a5\",\"key\":\"dave\",\"featureId\":\"59f102df9ab66c000b8247a4\",\"environmentId\":\"598947c33cb2f2000ac1a636\",\"projectId\":\"598947c33cb2f2000ac1a634\",\"organisationId\":\"5816b146a377640007a89555\",\"archived\":false,\"status\":\"unavailable\",\"hitsToday\":0,\"deleted\":false,\"rules\":[{\"priority\":0,\"defaultRule\":true,\"audience\":null,\"variantSplits\":[{\"variantKey\":\"off\",\"split\":0},{\"variantKey\":\"on\",\"split\":100}],\"description\":\"\"}],\"statistics\":{\"impressions\":{},\"uniqueImpressions\":{},\"mostActiveVariant\":{},\"variants\":{}},\"offVariantKey\":\"off\",\"inClientApi\":true,\"enabled\":false,\"permanent\":false,\"staleDays\":5,\"url\":\"/projects/598947c33cb2f2000ac1a634/\",\"stale\":false,\"entityName\":\"dave\",\"entityType\":\"feature control\"},\"example-feature\":{\"id\":\"598947c33cb2f2000ac1a63e\",\"key\":\"example-feature\",\"featureId\":\"598947c33cb2f2000ac1a63c\",\"environmentId\":\"598947c33cb2f2000ac1a636\",\"projectId\":\"598947c33cb2f2000ac1a634\",\"organisationId\":\"5816b146a377640007a89555\",\"archived\":false,\"status\":\"available\",\"hitsToday\":0,\"deleted\":false,\"rules\":[{\"priority\":1,\"defaultRule\":false,\"audience\":{\"id\":null,\"name\":null,\"conditions\":[{\"target\":\"age\",\"operator\":\"greaterThan\",\"values\":[21]}]},\"variantSplits\":[{\"variantKey\":\"off\",\"split\":33},{\"variantKey\":\"on\",\"split\":67}],\"description\":null},{\"priority\":2,\"defaultRule\":false,\"audience\":{\"id\":null,\"name\":null,\"conditions\":[{\"target\":\"featureflow.date\",\"operator\":\"before\",\"values\":[\"2018-02-19T13:00:00.000Z\"]}]},\"variantSplits\":[{\"variantKey\":\"off\",\"split\":100},{\"variantKey\":\"on\",\"split\":0}],\"description\":null},{\"priority\":3,\"defaultRule\":false,\"audience\":{\"id\":null,\"name\":null,\"conditions\":[{\"target\":\"featureflow.user.id\",\"operator\":\"contains\",\"values\":[\"oliver\"]}]},\"variantSplits\":[{\"variantKey\":\"off\",\"split\":0},{\"variantKey\":\"on\",\"split\":100}],\"description\":null},{\"priority\":0,\"defaultRule\":true,\"audience\":null,\"variantSplits\":[{\"variantKey\":\"off\",\"split\":100},{\"variantKey\":\"on\",\"split\":0}],\"description\":null}],\"statistics\":{\"createdDate\":\"2017-08-08T05:55:39.928Z\",\"lastModifiedDate\":\"2017-09-04T13:11:59.033Z\",\"impressions\":{\"all\":14298,\"today\":0},\"uniqueImpressions\":{},\"mostActiveVariant\":{\"today\":null},\"variants\":{\"off\":{\"impressions\":{\"today\":0},\"uniqueImpressions\":{}},\"on\":{\"impressions\":{\"today\":0},\"uniqueImpressions\":{}}}},\"offVariantKey\":\"off\",\"inClientApi\":true,\"enabled\":true,\"permanent\":false,\"staleDays\":5,\"url\":\"/projects/598947c33cb2f2000ac1a634/\",\"stale\":false,\"entityName\":\"example-feature\",\"entityType\":\"feature control\"}}";
		    var flags = JsonConvert.DeserializeObject<IDictionary<string, FeatureControl>>(result);
		    Console.WriteLine(flags);
		    //Assert something here		   
		    Assert.True(flags.TryGetValue("new-one", out var control));
		    Assert.Equal("new-one", control.Key);		    
	    }
	    	    
	    
	    
	    
	    
	    
    
    }
}
