using System.Runtime.CompilerServices;

// The unit tests cover configuration defaults whose setters are internal - FeatureflowConfigBuilder is
// the only supported way to set them from outside the assembly - so the test assembly is a friend.
[assembly: InternalsVisibleTo("Featureflow.Tests")]
