using Autofac;
using Autofac.Versioned;
using System;

namespace TestProjectV1V2V3
{
    public class Version_1_ : Module, IVersionModule
    {
        public Version AvailableSince => new Version(1,0);

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterInstance(AvailableSince);
        }
    }
}
