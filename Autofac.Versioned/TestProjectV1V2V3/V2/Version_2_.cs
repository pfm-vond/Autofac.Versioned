using Autofac;
using Autofac.Versioned;
using System;

namespace TestProjectV1V2V3
{

    public class Version_2_ : Module, IVersionModule,
        IIncludeFeature<CRNameFeature>
    {
        public Version AvailableSince => new Version(2, 0);

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterInstance(AvailableSince);
        }
    }
}
