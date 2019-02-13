# featureflow-dotnet-client
[![][dependency-img]][dependency-url]

.NET client SDK for Featureflow compatible with .NET Framework 4.5, .NET Standard 1.3, .NET Standard 2.0.

Get your Featureflow account at [featureflow.io](http://www.featureflow.io)

## Get Started

The easiest way to get started is to follow the [Featureflow quick start guides](http://docs.featureflow.io/docs)

## Change Log

Please see [CHANGELOG](https://github.com/featureflow/featureflow-dotnet-sdk/blob/master/CHANGELOG.md).

## Installation

Using NuGet
```xml
Install-Package Featureflow
```

## Usage

### Quick start

Get your 'Server Environment Api Key' from the environment page in featureflow and instantiate a singleton client:
#### Instantiate the client
```c#
  var client = FeatureflowClientFactory.Create("srv-env-YOUR_SERVER_ENVIRONMENT_API_KEY");
```
or
```c#
  var client = await FeatureflowClientFactory.CreateAsync("srv-env-YOUR_SERVER_ENVIRONMENT_API_KEY");

```
#### Evaluate a feature
In your code, you can test the value of your feature where the value of `my-feature-key` is equal to `'on'` 
```c#
  var result = client.Evaluate("example-feature", user).Is("on");

```

Because the most commonly used variants for any feature are `'on'` and `'off'`, we have provided two helper methods `.IsOn()` and `.IsOff()`

```c#

    if (client.Evaluate("example-feature").IsOn())
    {
	    //do something
    }
    if (client.Evaluate("example-feature").IsOff())
    {
	    //do something
    }    
```
### Setting feature failover values

The feature failover value is used if the feature is not found in the features cache for any reason.
The default value is 'off'. You may define your features in code and specify a failover value.
```
var client = new FeatureflowClient("srv-env-YOUR_KEY", new List<Feature>
		    {
			    new Feature
			    {
				    Key = "feature-one",
				    FailoverVariant = "unavailable"
			    },
			    new Feature
			    {
				    Key = "new-one",
				    FailoverVariant = "green"
			    }
		    }, new FeatureflowConfig());

```


### Adding a User
You can pass user information in to allow features to be targeted.
At the point in time of evaluation (e.g. on a rest call or other call) you can create and pass in a user by creating a `FeatureflowUser` object. We have a builder to help:

```c#
var user = new User("1234");
user.WithAttribute("region", "sydney");
user.WithAttribute("days", new List<int> { 11, 1, 4, 29 });
user.WithSessionAttribute("dayofweek", 5);
var result = client.Evaluate("example-feature", user).Value();
```
User attributes can be of type `DateTime`, `String`, any numeric type (like `int` or `double`) or `IEnumerable<DateTime>`, `IEnumerable<String>` or `IEnumerable` of any numeric type

When a list of user attributes is passed in, each rule may match any of the attribute values, additionally each attribute is stored in featureflow for subsequent lookup in rule creation.

If you do not want the user saved in featureflow set '.SaveUser(false)' on the FeatureflowUser object.
 
Evaluate by passing the user into the evaluate method:

```
var result = client.Evaluate("example-feature", user).Value();

```


Further documentation can be found [here](http://docs.featureflow.io/docs)


## About featureflow
* Featureflow is an application feature management tool that allows you to safely and effectively release, manage and evaluate your applications features across multiple applications, platforms and languages.
    * Dark / Silent Release with features turned off
    * Gradual rollout to a percent of users
    * Virtual Rollout and Rollback of features
    * Environment and Component feature itinerary
    * Target features to specific audiences
    * A/B and Multivariant test new feature variants - migrate to the winner
    All without devops, engineering or downtime.
* We have SDKs in the following languages
    * [Javascript] (https://github.com/featureflow/featureflow-javascript-sdk)
    * [Java] (https://github.com/featureflow/featureflow-java-sdk)
    * [NodeJS] (https://github.com/featureflow/featureflow-node-sdk)
    * [ReactJS] (https://github.com/featureflow/react-featureflow-client)
    * [angular] (https://github.com/featureflow/featureflow-ng)
    * [PHP] (https://github.com/featureflow/featureflow-php-sdk)
    * [.NET] (https://github.com/featureflow/featureflow-dotnet-client)
* Find out more
    * [Docs] http://docs.featureflow.io/docs
    * [Web] https://www.featureflow.io/     


## Roadmap
- [x] Improved .NET standard compatibility
- [x] Polling feature stream client
- [x] Improve README
- [ ] SSE feature stream client
- [ ] Post Events Client
- [ ] Register features on Startup
- [ ] Optimised events summaries

## License

Apache-2.0

[dependency-url]: https://www.featureflow.io
[dependency-img]: https://www.featureflow.io/wp-content/uploads/2016/12/featureflow-web.png
