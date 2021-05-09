using Autofac.Core;
using System;

namespace Autofac.Versioned
{
    /// <summary>
    /// this Autofac Module represents a Feature that can be enable or disable at will
    /// </summary>
    public interface IFeatureModule : IModule
    {
        /// <summary>
        /// Autofac.Versioned will use this name to look up config in the Iconfiguration
        /// to see if Module should be loaded.
        /// 
        /// example : if name is "displayLogo"
        /// if displayLogo is not in Iconfiguration we will use the ActiveBydefault prop value
        /// otherwise if displayLogo is true the module will be register
        /// otherwise if displayLogo is false the module will not be register
        /// otherwise if displayLogo exist bug contains neither true nor false an Argument exception will be thrown.
        /// </summary>
        string FeatureName { get; }

        /// <summary>
        /// Autofac.Versioned will use this prop as default value for this feature
        /// to see if Module should be loaded.
        /// 
        /// example : if ConfigName prop contain "displayLogo"
        /// if displayLogo is not in Iconfiguration and ActiveBydefault is true Module will be register
        /// otherwise if displayLogo is not in Iconfiguration and ActiveBydefault is false Module will not be register
        /// this prop is ignore if displayLogo appear in Iconfiguration.
        /// </summary>
        bool ActiveByDefault { get; }
    }
}
