# Change log

Featureflow Changelog

## [1.0.2] - 2019-02-09
### Added:
Updated SDK 1.0.2

## [1.0.3] - 2019-02-09
### Added:
Polling Client
Improved .net Compatibility Framework 4.5, .Net Standard 1.3 - 2.0.

## [1.1.3] - 2026-08-11
### Fixed:
The package icon is now embedded in the package (`<icon>`) instead of referencing a long-dead
`iconUrl`, so it displays on nuget.org again.

## [1.1.2] - 2026-08-11
### Security:
Updated `Newtonsoft.Json` from 12.0.1 to 13.0.1, resolving the high severity advisory
GHSA-5crp-9r3c-p9vr / CVE-2024-21907 (NU1903) - improper handling of deeply nested JSON, which can
raise a `StackOverflowException` when serialising or consume excessive CPU and memory when
deserialising. As this SDK is a library, the vulnerable version flowed through to every consuming
application.

13.0.1 is the lowest version that resolves the advisory, chosen to keep the version floor imposed on
consumers as low as possible. All three target frameworks - `net45`, `netstandard1.3` and
`netstandard2.0` - remain supported and are unchanged.

### Fixed:
`FeatureflowConfig.BaseUri` now defaults to the REST host (`https://app.featureflow.io`) rather than
the SSE stream host, so a directly constructed `new FeatureflowConfig()` works with
`GetFeaturesMethod.Polling` as well as with streaming. Configs built with `FeatureflowConfigBuilder`
are unaffected - the builder already set the REST host explicitly.

README: removed a `SaveUser(false)` example for a method this SDK does not have (it exists only in
the Java SDK), and corrected the failover example to use `FeatureflowClientFactory`.



