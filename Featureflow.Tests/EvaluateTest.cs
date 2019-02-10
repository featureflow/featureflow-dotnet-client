using System;
using System.Collections.Generic;
using Xunit;
using Featureflow.Client;
using Newtonsoft.Json;
using static Xunit.Assert;


namespace Featureflow.Tests
{
    public class EvaluateTest
    {
        private const string features = "{\"new-one\":{\"id\":\"598c38cdb7801d000952f798\",\"key\":\"new-one\",\"featureId\":\"598c38cdb7801d000952f797\",\"environmentId\":\"598947c33cb2f2000ac1a636\",\"projectId\":\"598947c33cb2f2000ac1a634\",\"organisationId\":\"5816b146a377640007a89555\",\"archived\":false,\"status\":\"unavailable\",\"hitsToday\":0,\"deleted\":false,\"rules\":[{\"priority\":0,\"defaultRule\":false,\"audience\":{\"id\":null,\"name\":null,\"conditions\":[{\"target\":\"featureflow.user.id\",\"operator\":\"equals\",\"values\":[\"oliver\"]}]},\"variantSplits\":[{\"variantKey\":\"on\",\"split\":100},{\"variantKey\":\"off\",\"split\":0}],\"description\":null},{\"priority\":1,\"defaultRule\":true,\"audience\":null,\"variantSplits\":[{\"variantKey\":\"on\",\"split\":0},{\"variantKey\":\"off\",\"split\":100}],\"description\":null}],\"statistics\":{\"createdDate\":\"2017-09-01T07:00:13.619Z\",\"lastModifiedDate\":\"2017-09-01T07:00:13.635Z\",\"impressions\":{\"today\":0},\"uniqueImpressions\":{},\"mostActiveVariant\":{\"today\":null},\"variants\":{\"off\":{\"impressions\":{\"today\":0},\"uniqueImpressions\":{}},\"on\":{\"impressions\":{\"today\":0},\"uniqueImpressions\":{}}}},\"offVariantKey\":\"off\",\"inClientApi\":true,\"enabled\":true,\"permanent\":false,\"staleDays\":5,\"url\":\"/projects/598947c33cb2f2000ac1a634/\",\"stale\":false,\"entityName\":\"new-one\",\"entityType\":\"feature control\"},\"standard-login\":{\"id\":\"5994de796036fa000a7daa10\",\"key\":\"standard-login\",\"featureId\":\"5994de796036fa000a7daa0f\",\"environmentId\":\"598947c33cb2f2000ac1a636\",\"projectId\":\"598947c33cb2f2000ac1a634\",\"organisationId\":\"5816b146a377640007a89555\",\"archived\":false,\"status\":\"unavailable\",\"hitsToday\":0,\"deleted\":false,\"rules\":[{\"priority\":0,\"defaultRule\":true,\"audience\":null,\"variantSplits\":[{\"variantKey\":\"off\",\"split\":0},{\"variantKey\":\"on\",\"split\":100}],\"description\":\"\"}],\"statistics\":null,\"offVariantKey\":\"off\",\"inClientApi\":true,\"enabled\":false,\"permanent\":false,\"staleDays\":5,\"url\":\"/projects/598947c33cb2f2000ac1a634/\",\"stale\":false,\"entityName\":\"standard-login\",\"entityType\":\"feature control\"},\"dave\":{\"id\":\"59f102df9ab66c000b8247a5\",\"key\":\"dave\",\"featureId\":\"59f102df9ab66c000b8247a4\",\"environmentId\":\"598947c33cb2f2000ac1a636\",\"projectId\":\"598947c33cb2f2000ac1a634\",\"organisationId\":\"5816b146a377640007a89555\",\"archived\":false,\"status\":\"unavailable\",\"hitsToday\":0,\"deleted\":false,\"rules\":[{\"priority\":0,\"defaultRule\":true,\"audience\":null,\"variantSplits\":[{\"variantKey\":\"off\",\"split\":0},{\"variantKey\":\"on\",\"split\":100}],\"description\":\"\"}],\"statistics\":{\"impressions\":{},\"uniqueImpressions\":{},\"mostActiveVariant\":{},\"variants\":{}},\"offVariantKey\":\"off\",\"inClientApi\":true,\"enabled\":false,\"permanent\":false,\"staleDays\":5,\"url\":\"/projects/598947c33cb2f2000ac1a634/\",\"stale\":false,\"entityName\":\"dave\",\"entityType\":\"feature control\"},\"example-feature\":{\"id\":\"598947c33cb2f2000ac1a63e\",\"key\":\"example-feature\",\"featureId\":\"598947c33cb2f2000ac1a63c\",\"environmentId\":\"598947c33cb2f2000ac1a636\",\"projectId\":\"598947c33cb2f2000ac1a634\",\"organisationId\":\"5816b146a377640007a89555\",\"archived\":false,\"status\":\"available\",\"hitsToday\":0,\"deleted\":false,\"rules\":[{\"priority\":0,\"defaultRule\":false,\"audience\":{\"id\":null,\"name\":null,\"conditions\":[{\"target\":\"age\",\"operator\":\"greaterThan\",\"values\":[21]}]},\"variantSplits\":[{\"variantKey\":\"off\",\"split\":33},{\"variantKey\":\"on\",\"split\":49},{\"variantKey\":\"red\",\"split\":8},{\"variantKey\":\"blue\",\"split\":10}],\"description\":null},{\"priority\":1,\"defaultRule\":false,\"audience\":{\"id\":null,\"name\":null,\"conditions\":[{\"target\":\"featureflow.date\",\"operator\":\"after\",\"values\":[\"2018-02-19T13:00:00.000Z\"]}]},\"variantSplits\":[{\"variantKey\":\"off\",\"split\":0},{\"variantKey\":\"on\",\"split\":0},{\"variantKey\":\"red\",\"split\":100},{\"variantKey\":\"blue\",\"split\":0}],\"description\":null},{\"priority\":2,\"defaultRule\":false,\"audience\":{\"id\":null,\"name\":null,\"conditions\":[{\"target\":\"featureflow.user.id\",\"operator\":\"contains\",\"values\":[\"oliver\"]}]},\"variantSplits\":[{\"variantKey\":\"off\",\"split\":0},{\"variantKey\":\"on\",\"split\":0},{\"variantKey\":\"red\",\"split\":0},{\"variantKey\":\"blue\",\"split\":100}],\"description\":null},{\"priority\":3,\"defaultRule\":true,\"audience\":null,\"variantSplits\":[{\"variantKey\":\"off\",\"split\":0},{\"variantKey\":\"on\",\"split\":100},{\"variantKey\":\"red\",\"split\":0},{\"variantKey\":\"blue\",\"split\":0}],\"description\":null}],\"statistics\":{\"createdDate\":\"2018-02-22T05:50:50.627Z\",\"lastModifiedDate\":\"2018-02-22T05:50:50.642Z\",\"impressions\":{\"all\":14298,\"today\":0},\"uniqueImpressions\":{},\"mostActiveVariant\":{\"today\":null},\"variants\":{\"red\":{\"impressions\":{\"today\":0},\"uniqueImpressions\":{}},\"blue\":{\"impressions\":{\"today\":0},\"uniqueImpressions\":{}},\"off\":{\"impressions\":{\"today\":0},\"uniqueImpressions\":{}},\"on\":{\"impressions\":{\"today\":0},\"uniqueImpressions\":{}}}},\"offVariantKey\":\"off\",\"inClientApi\":true,\"enabled\":true,\"permanent\":false,\"staleDays\":5,\"url\":\"/projects/598947c33cb2f2000ac1a634/\",\"stale\":false,\"entityName\":\"example-feature\",\"entityType\":\"feature control\"}}";
        
        [Fact]
        public void TestEquals()
        {
            //construct the feature an rule
            var control = new FeatureControl();
            var rule = new Rule {VariantSplits = OnSplit()};
            var audience = new Audience();
            var condition = new Condition();

            control.Enabled = true;
            control.OffVariantKey = "off";
                       
            condition.Operator = "equals";
            condition.Target = "country";
            condition.Values.Add("spain");
            
            audience.Conditions.Add(condition);
            rule.Audience = audience;
            control.Rules.Add(rule);
            control.Rules.Add(DefaultOffRule());
            
            //create a matching user
            var user = new User("user1");
            var attributeValues = new List<object>(new[] {"spain"});
            user.Attributes = new Dictionary<string, List<object>> {{"country", attributeValues}};
            var evaluate = new Evaluate(control, user, "off");
            Equal(true, evaluate.IsOn());
        }
        
        
        [Fact]
        public void TestLessThan()
        {
            //construct the feature an rule
            var control = new FeatureControl();
            var rule = new Rule {VariantSplits = OnSplit()};
            var audience = new Audience();
            var condition = new Condition();

            control.Enabled = true;
            control.OffVariantKey = "off";
                       
            condition.Operator = "lessThan";
            condition.Target = "age";
            condition.Values.Add(65L);
            
            audience.Conditions.Add(condition);
            rule.Audience = audience;
            rule.VariantSplits = OnSplit();
            control.Rules.Add(rule);
            
            //create a matching user
            var user = new User("user1");
            var attributeValues = new List<object>{64L};
            user.Attributes = new Dictionary<string, List<object>> {{"age", attributeValues}};
            var evaluate = new Evaluate(control, user, "on");
            Equal(true, evaluate.IsOn());
        }
        
        [Fact]
        public void TestGreaterThan()
        {
            //construct the feature an rule
            var control = new FeatureControl();
            var rule = new Rule();
            rule.VariantSplits = OnSplit();
            var audience = new Audience();
            var condition = new Condition();

            control.Enabled = true;
            control.OffVariantKey = "off";
                       
            condition.Operator = "greaterThan";
            condition.Target = "age";
            condition.Values.Add(65L);
            
            audience.Conditions.Add(condition);
            rule.Audience = audience;
            rule.VariantSplits = OnSplit();
            control.Rules.Add(rule);
            control.Rules.Add(DefaultOffRule());
            
            //create a matching user
            var user = new User("user1");
            var attributeValues = new List<object>{66};
            user.Attributes = new Dictionary<string, List<object>> {{"age", attributeValues}};
            var evaluate = new Evaluate(control, user, "on");
            Equal(true, evaluate.IsOn());   
        }
        
        [Fact]
        public void TestLessThanOrEqual()
        {
            //construct the feature an rule
            var control = new FeatureControl();
            var rule = new Rule {VariantSplits = OnSplit()};
            var audience = new Audience();
            var condition = new Condition();

            control.Enabled = true;
            control.OffVariantKey = "off";
                       
            condition.Operator = "lessThanOrEqual";
            condition.Target = "age";
            condition.Values.Add(65L);
            
            audience.Conditions.Add(condition);
            rule.Audience = audience;
            rule.VariantSplits = OnSplit();
            control.Rules.Add(rule);
            control.Rules.Add(DefaultOffRule());
            
            //create a matching user
            var user = new User("user1");
            var attributeValues = new List<object>{65L};
            user.Attributes = new Dictionary<string, List<object>> {{"age", attributeValues}};
            var evaluate = new Evaluate(control, user, "on");
            Equal(true, evaluate.IsOn());   
        }
        
        [Fact]
        public void TestGreaterThanOrEqual()
        {
            //construct the feature an rule
            var control = new FeatureControl();
            var rule = new Rule {VariantSplits = OnSplit()};
            var audience = new Audience();
            var condition = new Condition();

            control.Enabled = true;
            control.OffVariantKey = "off";
                       
            condition.Operator = "greaterThanOrEqual";
            condition.Target = "age";
            condition.Values.Add(65L);
            
            audience.Conditions.Add(condition);
            rule.Audience = audience;
            rule.VariantSplits = OnSplit();
            control.Rules.Add(rule);
            control.Rules.Add(DefaultOffRule());                       
            
            
            //create a matching user
            var user = new User("user1");
            var attributeValues = new List<object>{65L};
            user.Attributes = new Dictionary<string, List<object>> {{"age", attributeValues}};
            var evaluate = new Evaluate(control, user, "on");
            Equal(true, evaluate.IsOn());
        }       
        
        [Fact]
        public void TestStartsWith()
        {
            //construct the feature an rule
            var control = new FeatureControl();
            var rule = new Rule {VariantSplits = OnSplit()};
            var audience = new Audience();
            var condition = new Condition();

            control.Enabled = true;
            control.OffVariantKey = "off";
                       
            condition.Operator = "startsWith";
            condition.Target = "name";
            condition.Values.Add("oli");
            
            audience.Conditions.Add(condition);
            rule.Audience = audience;
            rule.VariantSplits = OnSplit();
            control.Rules.Add(rule);
            control.Rules.Add(DefaultOffRule());
            
            //create a matching user
            var user = new User("user1");
            var attributeValues = new List<object>{"oliver"};
            user.Attributes = new Dictionary<string, List<object>> {{"name", attributeValues}};
            var evaluate = new Evaluate(control, user, "on");
            Equal(true, evaluate.IsOn());
        }
        
        [Fact]
        public void TestEndsWith()
        {
            //construct the feature an rule
            var control = new FeatureControl();
            var rule = new Rule {VariantSplits = OnSplit()};
            var audience = new Audience();
            var condition = new Condition();

            control.Enabled = true;
            control.OffVariantKey = "off";
                       
            condition.Operator = "endsWith";
            condition.Target = "name";
            condition.Values.Add("ver");
            
            audience.Conditions.Add(condition);
            rule.Audience = audience;
            rule.VariantSplits = OnSplit();
            control.Rules.Add(rule);
            control.Rules.Add(DefaultOffRule());
            
            //create a matching user
            var user = new User("user1");
            var attributeValues = new List<object>{"oliver"};
            user.Attributes = new Dictionary<string, List<object>> {{"name", attributeValues}};
            var evaluate = new Evaluate(control, user, "on");
            Equal(true, evaluate.IsOn());
        }        
        
        [Fact]
        public void TestMatches()
        {
            //construct the feature an rule
            var control = new FeatureControl();
            var rule = new Rule();
            rule.VariantSplits = OnSplit();
            var audience = new Audience();
            var condition = new Condition();

            control.Enabled = true;
            control.OffVariantKey = "off";
                       
            condition.Operator = "matches";
            condition.Target = "name";
            condition.Values.Add("^(oliver).*$");
            
            audience.Conditions.Add(condition);
            rule.Audience = audience;
            rule.VariantSplits = OnSplit();
            control.Rules.Add(rule);
            control.Rules.Add(DefaultOffRule());
            
            //create a matching user
            var user = new User("user1");
            var attributeValues = new List<object>{"oliver@featureflow.io"};
            user.Attributes = new Dictionary<string, List<object>> {{"name", attributeValues}};
            var evaluate = new Evaluate(control, user, "on");
            Equal(true, evaluate.IsOn());   
        }
        
        [Fact]
        public void TestIn()
        {
            //construct the feature an rule
            var control = new FeatureControl();
            var rule = new Rule();
            rule.VariantSplits = OnSplit();
            var audience = new Audience();
            var condition = new Condition();
            var values = new List<object>(new[] {"spain", "france", "uk"});

            control.Enabled = true;
            control.OffVariantKey = "off";

            control.Rules = new List<Rule>(new[] {rule});
            rule.Audience = audience;
            audience.Conditions = new List<Condition>(new[] {condition});
            condition.Operator = "in";
            condition.Target = "country";
            condition.Values = values;

            //create a matching user
            var user = new User("user1");
            var attributeValues = new List<object>(new[] {"spain"});
            user.Attributes = new Dictionary<string, List<object>> {{"country", attributeValues}};
            var evaluate = new Evaluate(control, user, "off");

            Equal(true, evaluate.IsOn());
            
        }
        [Fact]
        public void TestNotIn()
        {
            True(Condition.Evaluate("netherlands", "notIn", new List<object>(new[] {"spain", "france", "uk"})));
            
            
            //construct the feature an rule
            var control = new FeatureControl();
            var rule = new Rule {VariantSplits = OnSplit()};
            var audience = new Audience();
            var condition = new Condition();
            var values = new List<object>(new[] {"spain", "france", "uk"});

            control.Enabled = true;
            control.OffVariantKey = "off";

            control.Rules = new List<Rule>(new[] {rule});
            rule.Audience = audience;
            audience.Conditions = new List<Condition>(new[] {condition});
            condition.Operator = "notIn";
            condition.Target = "country";
            condition.Values = values;

            //create a matching user
            var user = new User("user1");
            var attributeValues = new List<object>(new[] {"netherlands"});
            user.Attributes = new Dictionary<string, List<object>> {{"country", attributeValues}};
            var evaluate = new Evaluate(control, user, "off");

            Equal(true, evaluate.IsOn());   
        }
        [Fact]
        public void TestContains()
        {
            
        }
        
        [Fact]
        public void TestBefore()
        {
            //construct the feature an rule
            var control = new FeatureControl();
            var rule = new Rule {VariantSplits = OnSplit()};
            var audience = new Audience();
            var condition = new Condition();
             
            
            control.Enabled = true;
            control.OffVariantKey = "off";
                       
            condition.Operator = "before";
            condition.Target = "signupDate";
            condition.Values.Add(new DateTime(2017, 1, 18));
            
            audience.Conditions.Add(condition);
            rule.Audience = audience;
            rule.VariantSplits = OnSplit();
            
            control.Rules.Add(rule);
            control.Rules.Add(DefaultOffRule());                       
            
            
            //create a matching user
            var user = new User("123");
            user.WithAttribute("signupDate", new DateTime(2017, 1, 17));            
            
            var evaluate = new Evaluate(control, user, "on");
            Equal(true, evaluate.IsOn());
        }

        [Fact]
        public void TestAfter()
        {
            /*var features = JsonConvert.DeserializeObject<IDictionary<string, FeatureControl>>(EvaluateTest.features);
            var user = new User("user1");
            
            features.TryGetValue("example-feature", out var feature);
            var evaluate = new Evaluate(feature, user, "off");
            Equal("red", evaluate.Value());*/ 
            //construct the feature an rule
            var control = new FeatureControl();
            var rule = new Rule {VariantSplits = OnSplit()};
            var audience = new Audience();
            var condition = new Condition();
             
            
            control.Enabled = true;
            control.OffVariantKey = "off";
                       
            condition.Operator = "after";
            condition.Target = "signupDate";
            condition.Values.Add(new DateTime(2017, 1, 17));
            
            audience.Conditions.Add(condition);
            rule.Audience = audience;
            rule.VariantSplits = OnSplit();
            
            control.Rules.Add(rule);
            control.Rules.Add(DefaultOffRule());                       
            
            
            //create a matching user
            var user = new User("123");
            user.WithAttribute("signupDate", new DateTime(2017, 1, 18));            
            
            var evaluate = new Evaluate(control, user, "on");
            Equal(true, evaluate.IsOn());
        }
        
        [Fact]
        public void TestDefaultRuleWhenNotMatching()
        {
            //construct the feature an rule
            var control = new FeatureControl();
            var rule = new Rule {VariantSplits = OnSplit()};
            var audience = new Audience();
            var condition = new Condition();

            control.Enabled = true;
            control.OffVariantKey = "off";
                       
            condition.Operator = "greaterThanOrEqual";
            condition.Target = "age";
            condition.Values.Add(65L);
            
            audience.Conditions.Add(condition);
            rule.Audience = audience;
            rule.VariantSplits = OnSplit();
            control.Rules.Add(rule);
            control.Rules.Add(DefaultOffRule());                       
            
            
            //create a matching user
            var user = new User("user1");
            var attributeValues = new List<object>{64L};
            user.Attributes = new Dictionary<string, List<object>> {{"age", attributeValues}};
            var evaluate = new Evaluate(control, user, "on");
            Equal(true, evaluate.IsOff());
        }  
        
        [Fact]
        public void TestDisabledIsOffVariant()
        {
            //construct the feature and rule
            var control = new FeatureControl
            {
                Enabled = false,
                OffVariantKey = "off" //should be equal to Variant.off
            };

            //create a user
            var user = new User("user1");
            var evaluate = new Evaluate(control, user, "off");

            Equal(true, evaluate.IsOff());
        }
        
        
        [Fact]
        public void TestDateComparison()
        {
            var features = JsonConvert.DeserializeObject<IDictionary<string, FeatureControl>>(EvaluateTest.features);
            var user = new User("user1");
            
            features.TryGetValue("example-feature", out var feature);
            var evaluate = new Evaluate(feature, user, "off");
            Equal("red", evaluate.Value());            
        }
        
        [Fact]
        public void TestLongFeatureDoesntNegateRolloutBucketDistribution()
        {
            //construct the feature an rule
            var control = new FeatureControl();
            var rule = new Rule();
            var off = new VariantSplit
            {
                VariantKey = "off",
                Split = 49
            };
            var on = new VariantSplit();
            on.VariantKey = "on";
            on.Split = 51;
            
            rule.VariantSplits = new List<VariantSplit>{on, off};

            control.Key = "a-long-key-of-16";
            control.Enabled = true;
            control.OffVariantKey = "off";

            control.Rules = new List<Rule>(new[] {rule});            
            //create a user
            var user = new User("user1");
            var evaluate1 = new Evaluate(control, user, "off");
            
            var user2 = new User ("user2");
            var evaluate2 = new Evaluate(control, user2, "off");
            
            Equal(true, evaluate1.IsOff());
            Equal(true, evaluate2.IsOn());
        }

        private List<VariantSplit> OnSplit()
        {
            var off = new VariantSplit
            {
                VariantKey = "off",
                Split = 0
            };
            var on = new VariantSplit
            {
                VariantKey = "on",
                Split = 100
            };
            
            return new List<VariantSplit>{on, off};
        }
        private List<VariantSplit> OffSplit()
        {
            var off = new VariantSplit
            {
                VariantKey = "off",
                Split = 100
            };
            var on = new VariantSplit
            {
                VariantKey = "on",
                Split = 0
            };
            
            return new List<VariantSplit>{on, off};
        }

        private Rule DefaultOffRule()
        {
            return new Rule {VariantSplits = OffSplit()};
        }        
    }
}
