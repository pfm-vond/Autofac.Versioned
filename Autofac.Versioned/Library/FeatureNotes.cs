using System;

namespace Autofac.Versioned
{
    public record FeatureNotes(string FeatureName, string Description, Version Version);
}
