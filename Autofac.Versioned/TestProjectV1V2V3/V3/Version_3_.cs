using Autofac;
using Autofac.Features.AttributeFilters;
using Autofac.Versioned;
using System;

namespace TestProjectV1V2V3
{
    public class Version_3_ : Module, IVersionModule,
        IIncludeFeature<CRUNameFeature>,
        IIncludeFeature<CRSurnameFeature>
    {
        public Version AvailableSince => new Version(3, 0);

        private string HelloMessage { get; }

        public Version_3_([KeyFilter("helloMessage")] string versionHelloMessage)
        {
            HelloMessage = versionHelloMessage;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterInstance(AvailableSince);
            builder.RegisterInstance(HelloMessage);
        }
    }
}
