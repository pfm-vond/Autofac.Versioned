using Autofac;
using Autofac.Versioned;
using System;

namespace TestProjectV1V2V3
{
    public class CRSurnameFeature : Module, IFeatureModule, IFeatureDescription
    {
        public string FeatureName => "CrudSurname";

        public bool ActiveByDefault => true;

        public string Description => "to use this feature just request a CSurname " +
            $"to add surname that you will be able to read via the RSurname interfaces.{Environment.NewLine}" +
            "we don't support neither Delete nor Update for now";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterInstance(new CRUString())
                .As<CSurname>()
                .As<RSurname>();
        }
    }
}