using Autofac.Core;
using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Autofac.Versioned
{
    public static class AssemblyModules
    {
        /// <summary>
        /// lookup for all IVersionModule in the versionedModuleLocation and register them.
        /// </summary>
        /// <param name="versionedModuleLocation">the location where to look for IVersionModule.</param>
        /// <param name="registerModule">method to register Module, usually use ContainerBuilder.RegisterModule of your container</param>
        /// <param name="loadUntilVersion">all IVersionModule before this version will be register. if null all IVersionModule will be register</param>
        /// <param name="moduleDependencies">any dependencies needed for building the IVersionModule. Beware those will not be available in the application</param>
        public static void RegisterAssemblyVersions(
            this ContainerBuilder cb,
            Assembly versionedModuleLocation,
            string loadUntilVersion = null,
            params IModule[] moduleDependencies)
        {
            Version loadUntil = loadUntilVersion == null ? null : new Version(loadUntilVersion);

            using (var moduleDi = Build(
                moduleDependencies,
                versionedModuleLocation))
            {
                var modules = moduleDi
                    .Resolve<IEnumerable<IVersionModule>>()
                    .OrderBy(t => t.AvailableSince);

                Dictionary<string, (Version Version, IFeatureModule Module)> features = new();

                foreach (var module in modules)
                {
                    if (loadUntil != null && module.AvailableSince > loadUntil)
                        return;

                    cb.RegisterModule(module);
                    RegisterFeatures(features, moduleDi, module);
                }

                foreach (var feature in features.Values.OrderBy(v => v.Version))
                {
                    cb.RegisterModule(feature.Module);
                    cb.RegisterInstance(new FeatureNotes(
                        feature.Module.FeatureName,
                        (feature.Module as IFeatureDescription)?.Description,
                           feature.Version));
                }
            }
        }

        private static IContainer Build(
            IModule[] moduleDependencies,
            Assembly versionedModuleLocation)
        {
            ContainerBuilder container = new();
            
            foreach(var md in moduleDependencies)
                container.RegisterModule(md);

            container.RegisterAssemblyTypes(versionedModuleLocation)
                .Where(t => t.IsAssignableTo(typeof(IVersionModule)))
                .As<IVersionModule>()
                .WithAttributeFiltering();

            container.RegisterAssemblyTypes(versionedModuleLocation)
                .Where(t => t.IsAssignableTo(typeof(IFeatureModule)))
                .AsSelf()
                .WithAttributeFiltering();

            return container.Build();
        }

        private static void RegisterFeatures(
            Dictionary<string, (Version, IFeatureModule)> features, 
            IContainer moduleDi, 
            IVersionModule module)
        {
            var featureModuleTypes = module.GetType()
                .GetInterfaces()
                .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IIncludeFeature<>))
                .Select(t => t.GetGenericArguments()[0]);

            var config = moduleDi.ResolveOptional<IConfiguration>();
            foreach (var featureType in featureModuleTypes)
            {
                var feature = (IFeatureModule)moduleDi.Resolve(featureType);

                bool isActive = feature.ActiveByDefault;
                if(config != null && config[feature.FeatureName] != null)
                {
                    var strIsActive = config[feature.FeatureName];

                    if(strIsActive == "true" || strIsActive == "false")
                    {
                        isActive = strIsActive == "true";
                    }
                    else
                    {
                        throw new ArgumentException($"expected to be 'true' or 'false' but found '{strIsActive}'", feature.FeatureName);
                    }
                }

                if(isActive)
                {
                    features[feature.FeatureName] = (module.AvailableSince, feature);
                }
            }
        }
    }
}
