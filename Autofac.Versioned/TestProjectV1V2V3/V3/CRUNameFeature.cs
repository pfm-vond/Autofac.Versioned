using Autofac;
using Autofac.Versioned;

namespace TestProjectV1V2V3
{
    public class CRUNameFeature : Module, IFeatureModule
    {
        public string FeatureName => "CrudName";

        public bool ActiveByDefault => false;

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterInstance(new CRUString())
                .As<CName>()
                .As<RName>()
                .As<UName>();
        }
    }
}
