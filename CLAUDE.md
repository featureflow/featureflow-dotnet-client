# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

The server-side .NET SDK for Featureflow, published to NuGet as the `Featureflow` package
(namespace `Featureflow.Client`). This is a public library — preserve the existing public API
surface and behavior; breaking changes need a deliberate version decision, not a refactor.

This repo sits inside the multi-repo Featureflow workspace; the workspace `CLAUDE.md` one level
up covers the wider system (which backend serves `/api/sdk/`, key types, etc.).

## Commands

```bash
dotnet build featureflow-dotnet-client.sln -c Release        # build all targets
dotnet test Featureflow.Tests/Featureflow.Tests.csproj       # run all tests (xUnit)
dotnet test Featureflow.Tests/Featureflow.Tests.csproj --filter "FullyQualifiedName~EvaluateTest"   # one class
dotnet test Featureflow.Tests/Featureflow.Tests.csproj --filter "FullyQualifiedName~EvaluateTest.TestName"  # one test
dotnet pack Featureflow/Featureflow.csproj -c Release        # build the NuGet package
```

Tests target `net8.0`; if only a newer .NET runtime is installed locally, prefix test runs with
`DOTNET_ROLL_FORWARD=Major` (CI installs 8.0 so doesn't need it).

## Constraints that shape the code

- **Multi-targeting**: the library targets `net45;netstandard1.3;netstandard2.0`. Code must
  compile on all three — no modern BCL APIs or language features that need newer targeting.
  Tests run on `net8.0` (consuming the `netstandard2.0` build), so a passing test run does not
  prove the `net45`/`netstandard1.3` targets compile; build the solution to check.
- **Serialization is Newtonsoft.Json**, not System.Text.Json.
- `FeatureflowLogger.cs` is excluded from compilation in the csproj (dead file).
- **Cross-SDK contract**: evaluation and wire behavior must stay identical across every
  Featureflow SDK and server. Shared scenarios live in the `featureflow-client-sdk-testbed`
  repo (gherkin + `CONTRACT.md`); `ApplicationTagTest.cs` mirrors its application-tag feature.
  In particular, bucketing is `SHA-1("{salt}:{featureKey}:{bucketKey}")`, first 15 hex chars
  parsed as a long, `% 100 + 1`, with salt `"1"` ([Evaluate.cs](Featureflow/Evaluate.cs)) — never
  change it.

## Architecture

Public entry point is `FeatureflowClientFactory.Create/CreateAsync`, which constructs the
internal `FeatureflowClient` (implements `IFeatureflowClient`) and blocks/awaits until the
initial feature set is loaded. Everything else is `internal`; the public surface is the factory,
`IFeatureflowClient`, `Evaluate`, `User`, `Feature`, `FeatureflowConfig(Builder)`,
`GetFeaturesMethod`, and the update/delete event args.

Three backend hosts, all configured in `FeatureflowConfig`:
- `app.featureflow.io` — REST features endpoint (`/api/sdk/v1/features`), used by polling
- `rtm.featureflow.io` — SSE stream host (**not currently served** — see below)
- `events.featureflow.io` — evaluation event ingestion

Data flow:
1. **Transport** — `RestClient` creates `HttpClient`s (Bearer auth with the `sdk-srv-env-` key,
   `User-Agent: DotNetClient/<version>`, optional `X-Featureflow-Application` header, ETag-aware
   feature fetches). Feature rules arrive via `PollingClient` (the default, 30s interval), which
   writes into `SimpleMemoryFeatureCache` and raises `FeatureUpdated`/`FeatureDeleted`.
   An SSE path (`FeatureflowStreamClient` + `SseClient`, opt-in via `GetFeaturesMethod.Sse`)
   exists in code but the Featureflow service does not currently serve the stream — SSE is on
   hold workspace-wide (`ops/decisions/0003-sse-notification-only.md`). It's retained for API
   compatibility only; don't document it as working or make it the default.
2. **Evaluation** — `client.Evaluate(key, user)` returns an `Evaluate` object that computes the
   variant in its constructor: walk `FeatureControl.Rules` in order, match the user against
   `Audience`/`Condition`s (the last rule has a null audience and always matches), then pick a
   variant from `VariantSplit`s via the bucketing hash. Missing feature or offline → the
   failover variant (`"off"` unless registered via default `Feature`s at client creation).
   `Evaluate` also injects `featureflow.user.id`, `featureflow.hourofday` and `featureflow.date`
   session attributes before matching.
3. **Events** — every `IsOn()`/`Is()`/`Value()` call queues an `evaluate` event;
   `FeatureflowEventsClient` batches and posts them every 30s (queue capped at 10,000).
   Session attributes and `BucketKey` are stripped from the user before sending.

`Offline` mode constructs none of the network clients — evaluations work purely off
defaults/failover, which is what unit tests and CI consumers rely on.

**HTTP test seam**: `RestConfig.HttpMessageHandler` (internal) lets tests intercept all HTTP with
a fake handler instead of the network — see `ApplicationTagTest.cs` for the pattern.

## Releasing

Mirrors the other Featureflow SDKs (use the `/sdk-release` skill if available): bump
`<PackageVersion>` in `Featureflow/Featureflow.csproj`, update `CHANGELOG.md`, then publish a
GitHub release whose tag is the bare version (`1.2.0` — **no `v` prefix**).
`.github/workflows/publish.yml` runs the tests and pushes to nuget.org via trusted publishing
(OIDC; the `NuGet/login` user must stay the policy creator's username, not the package owner).
The version-matches-tag check in the workflow fails the publish if the csproj wasn't bumped.
