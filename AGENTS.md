# AGENTS.md

This repository is a .NET client SDK for Featureflow.

## What this project is
- A library package built from `featureflow-dotnet-client.sln`.
- Main assembly: `Featureflow/Featureflow.csproj`.
- Unit tests in `Featureflow.Tests/Featureflow.Tests.csproj`.
- Targets: `net45`, `netstandard1.3`, `netstandard2.0` for the library and `netcoreapp2.2` for tests.

## Build and test commands
- `dotnet build featureflow-dotnet-client.sln -c Release`
- `dotnet test Featureflow.Tests/Featureflow.Tests.csproj`
- `dotnet pack Featureflow/Featureflow.csproj -c Release`

## Key areas for agents
- `Featureflow/FeatureflowClient.cs` and `Featureflow/FeatureflowClientFactory.cs` for client creation and runtime evaluation.
- `Featureflow/FeatureflowConfig.cs`, `Featureflow/FeatureflowConfigBuilder.cs`, and `Featureflow/RestConfig.cs` for configuration patterns.
- `Featureflow/Evaluate.cs`, `Featureflow/FeatureControl.cs`, `Featureflow/Rule.cs`, `Featureflow/Variant.cs`, `Featureflow/VariantSplit.cs` for evaluation logic.
- `Featureflow/FeatureflowStreamClient.cs`, `Featureflow/SseClient.cs`, `Featureflow/PollingClient.cs`, and `Featureflow/RestClient.cs` for transport/streaming behavior.
- `Featureflow.Tests/EvaluateTest.cs` and `Featureflow.Tests/FeatureflowClientTest.cs` for functional examples and expected API behavior.

## Project conventions
- Keep public API compatibility in mind: this repo is a NuGet package.
- The library is implemented as a single SDK-style project with multi-targeting.
- `FeatureflowLogger.cs` is excluded from compilation in the project file.

## Documentation references
- Use the root `README.md` for usage examples and SDK intent.

## Agent behavior guidance
- Prefer small, test-covered changes over large refactors.
- Avoid changing public surface area without preserving existing behavior and package compatibility.
- When adding or updating code, validate with `dotnet test`.
