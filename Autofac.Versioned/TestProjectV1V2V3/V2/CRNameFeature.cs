using Autofac;
using Autofac.Versioned;

namespace TestProjectV1V2V3
{
    public class CRNameFeature : Module, IFeatureModule
    {
        public string FeatureName => "CrudName";

        public bool ActiveByDefault => true;

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterInstance(new CRName())
                .As<CName>()
                .As<RName>();
        }
    }
}
