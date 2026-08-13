# Featureflow .NET SDK

Server-side .NET SDK for [Featureflow](https://www.featureflow.io) feature management.

Compatible with .NET Framework 4.5+, and any runtime supporting .NET Standard 1.3 or 2.0 (.NET Core, .NET 5+, Mono, Xamarin).

## Installation

```shell
dotnet add package Featureflow
```

or via the Package Manager console:

```powershell
Install-Package Featureflow
```

## Quick start

You'll need the **server environment API key** (`sdk-srv-env-...`) from the environment page of your [Featureflow dashboard](https://app.featureflow.io). It's a secret — keep it out of source control.

Create one client for the lifetime of your application:

```csharp
using Featureflow.Client;

var client = FeatureflowClientFactory.Create("sdk-srv-env-YOUR_KEY");
```

`Create` blocks until the initial feature set has loaded, so evaluations are ready immediately. From an async context:

```csharp
var client = await FeatureflowClientFactory.CreateAsync("sdk-srv-env-YOUR_KEY");
```

The client is thread-safe, keeps itself up to date in the background, and implements `IDisposable` — dispose it on shutdown. Register it as a singleton in your DI container:

```csharp
builder.Services.AddSingleton<IFeatureflowClient>(
    FeatureflowClientFactory.Create(builder.Configuration["Featureflow:ApiKey"]));
```

Then evaluate features anywhere:

```csharp
if (client.Evaluate("my-feature-key", user).IsOn())
{
    // feature code
}
```

## Targeting users

Pass a `User` so targeting rules can match on who they are:

```csharp
var user = new User("user-1234");
user.WithAttribute("tier", "gold");
user.WithAttribute("region", "sydney");
var result = client.Evaluate("my-feature-key", user).IsOn();
```

Attribute values may be a `string`, any numeric type, a `DateTime`, or a `List<object>` of those. When an attribute holds a list, a rule matches if it matches **any** value in the list.

Attributes are stored in Featureflow so you can build rules against them later. Use `WithSessionAttribute` instead for values that should be evaluated but **not** persisted:

```csharp
user.WithSessionAttribute("dayofweek", 5);
```

For evaluations with no meaningful user (batch jobs, health checks):

```csharp
client.Evaluate("my-feature-key", User.Anonymous()).IsOn();
```

Percentage rollouts hash the user's `Id` by default; set `user.BucketKey` if you want rollout buckets keyed by something else (e.g. an account id so a whole organisation gets the same variant).

## Beyond on and off

Features can have any number of variants. Test for a specific one, or read the evaluated variant key directly:

```csharp
if (client.Evaluate("checkout-flow", user).Is("v2"))
{
    // show the v2 checkout
}

string variant = client.Evaluate("checkout-flow", user).Value(); // e.g. "v2"
```

`EvaluateAll(user)` returns a `Dictionary<string, Evaluate>` of every feature, which is handy for passing a full flag set to a front end.

## Failover values

If a feature can't be found (typo'd key, network failure before first load), evaluation returns the **failover variant** — `"off"` by default. You can register features in code with explicit failover variants:

```csharp
var client = FeatureflowClientFactory.Create("sdk-srv-env-YOUR_KEY", new List<Feature>
{
    new Feature { Key = "checkout-flow", FailoverVariant = "v1" },
});
```

## Configuration

Build a `FeatureflowConfig` for anything non-default:

```csharp
var config = new FeatureflowConfigBuilder()
    .WithGetFeaturesMethod(GetFeaturesMethod.Polling) // default: GetFeaturesMethod.Sse (streaming)
    .WithConnectionTimeout(TimeSpan.FromSeconds(10))  // default: 30s
    .Build();

var client = FeatureflowClientFactory.Create("sdk-srv-env-YOUR_KEY", config);
```

- **`GetFeaturesMethod.Sse`** (default) holds a streaming connection open so rule changes apply in near real time.
- **`GetFeaturesMethod.Polling`** periodically re-fetches the feature set instead — use it where long-lived connections are impractical.
- **`.WithOffline(true)`** makes no network calls at all: every evaluation returns the failover variant. Useful in unit tests and CI.

### Naming your application

Optionally tag this workload with an application name so the Featureflow dashboard can attribute SDK usage and flag evaluations to it (Admin → SDKs, and the "Evaluated by" panel on each feature's statistics tab):

```csharp
var config = new FeatureflowConfigBuilder()
    .WithApplication("checkout-api")
    .Build();
```

The name is a slug — lowercase letters, numbers, `.`, `_` and `-`, at most 64 characters. An invalid value is dropped with a warning and no tag is sent. The `FEATUREFLOW_APPLICATION` environment variable is used when the option is not set in code.

## Reacting to changes

The client raises events when feature rules change:

```csharp
client.FeatureUpdated += (sender, args) => logger.LogInformation("Feature {Key} updated", args.FeatureKey);
client.FeatureDeleted += (sender, args) => logger.LogInformation("Feature {Key} deleted", args.FeatureKey);
```

## More

- [Featureflow docs](https://docs.featureflow.io)
- [Changelog](CHANGELOG.md)
- All Featureflow SDKs: [github.com/featureflow](https://github.com/featureflow)

## License

Apache-2.0
