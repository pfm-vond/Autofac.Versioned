using Autofac;

namespace TestProjectV1V2V3
{
    public class Version_3_Dependencies : Module
    {
        private string Hello;
        public Version_3_Dependencies(string hello)
        {
            Hello = hello;
        }
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterInstance(Hello)
                .Keyed<string>("helloMessage");
        }
    }
}
