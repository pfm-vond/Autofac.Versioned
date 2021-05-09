using Autofac.Core;
using System;

namespace Autofac.Versioned
{
    /// <summary>
    /// this Autofac Module represents a Version that can be enable or disable at will
    /// </summary>
    public interface IVersionModule : IModule
    {
        /// <summary>
        /// Autofac.Versioned will use this to know from wich version this Module should be loaded.
        /// </summary>
        Version AvailableSince { get; }
    }

    public interface IIncludeFeature<T>
        where T : IFeatureModule
    {
    }
}
