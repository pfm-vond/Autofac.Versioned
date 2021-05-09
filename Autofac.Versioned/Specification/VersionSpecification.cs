using Autofac.Features.AttributeFilters;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Xunit;

namespace Autofac.Versioned
{
    public class VersionSpecification
    {
        [Fact]
        public void versioned_module_are_all_loaded_in_order()
        {
            ContainerBuilder cb = new();
            cb.RegisterAssemblyVersions(typeof(TestProjectV1V2.Version_1_).Assembly);
            var container = cb.Build();

            container.Resolve<Version>().Should().Be(new Version(2, 0));

            container.Resolve<IEnumerable<Version>>().Should().Contain(new Version(2, 0));
            container.Resolve<IEnumerable<Version>>().Should().Contain(new Version(1, 0));
        }

        [Fact]
        public void versioned_module_are_up_to_the_requested_Version()
        {
            ContainerBuilder cb = new();
            cb.RegisterAssemblyVersions(
                typeof(TestProjectV1V2.Version_1_).Assembly,
                "1.0");
            var container = cb.Build();

            container.Resolve<Version>().Should().Be(new Version(1, 0));
        }

        [Fact]
        public void versioned_module_can_have_dependencies()
        {
            ContainerBuilder cb = new();
            cb.RegisterAssemblyVersions(
                typeof(TestProjectV1V2V3.Version_1_).Assembly,
                moduleDependencies: new TestProjectV1V2V3.Version_3_Dependencies("hey Version 3 need this !!"));
            var container = cb.Build();

            container.Resolve<Version>().Should().Be(new Version(3, 0));
            container.Resolve<string>().Should().Be("hey Version 3 need this !!");
        }

        [Fact]
        public void feature_in_module_can_be_enable_by_default()
        {
            ContainerBuilder cb = new();
            cb.RegisterAssemblyVersions(
                typeof(TestProjectV1V2V3.Version_1_).Assembly,
                moduleDependencies: new TestProjectV1V2V3.Version_3_Dependencies("hey Version 3 need this !!"));
            var container = cb.Build();

            container.Resolve<TestProjectV1V2V3.CName>().Create("toto");
            container.Resolve<TestProjectV1V2V3.RName>().Read().Should().Contain("toto");
        }

        [Fact]
        public void feature_in_module_can_be_disable_by_default()
        {
            ContainerBuilder cb = new();
            cb.RegisterAssemblyVersions(
                typeof(TestProjectV1V2V3.Version_1_).Assembly,
                moduleDependencies: new TestProjectV1V2V3.Version_3_Dependencies("hey Version 3 need this !!"));
            var container = cb.Build();

            container.ResolveOptional<TestProjectV1V2V3.UName>().Should().BeNull();
        }

        [Fact]
        public void a_disable_by_default_feature_can_be_enable_via_configuration()
        {
            ContainerBuilder cb = new();
            cb.RegisterAssemblyVersions(
                typeof(TestProjectV1V2V3.Version_1_).Assembly,
                "3.0",
                new TestProjectV1V2V3.Version_3_Dependencies("hey Version 3 need this !!"), 
                new ConfigModule(new string[] { "-crudName", "true" }));
            var container = cb.Build();

            container.ResolveOptional<TestProjectV1V2V3.UName>().Should().NotBeNull();
        }

        [Fact]
        public void a_enable_by_default_feature_can_be_disable_via_configuration()
        {
            ContainerBuilder cb = new();
            cb.RegisterAssemblyVersions(
                typeof(TestProjectV1V2V3.Version_1_).Assembly,
                "3.0",
                new TestProjectV1V2V3.Version_3_Dependencies("hey Version 3 need this !!"),
                new ConfigModule(new string[] { "-crudName", "false" }));
            var container = cb.Build();

            container.ResolveOptional<TestProjectV1V2V3.CName>().Should().BeNull();
            container.ResolveOptional<TestProjectV1V2V3.RName>().Should().BeNull();
        }

        [Fact]
        public void if_the_config_for_a_feature_is_incorrect_we_receive_an_exception()
        {
            ContainerBuilder cb = new();
            var e = Record.Exception(()=> cb.RegisterAssemblyVersions(
                typeof(TestProjectV1V2V3.Version_1_).Assembly,
                "3.0",
                new TestProjectV1V2V3.Version_3_Dependencies("hey Version 3 need this !!"),
                new ConfigModule(new string[] { "-crudName", "toto" })));

            e.Should().NotBeNull();
            var ae = e.Should().BeOfType<ArgumentException>().Which;
            ae.Message.Should().Contain("toto");
            ae.Message.Should().Be("expected to be 'true' or 'false' but found 'toto' (Parameter 'CrudName')");
        }

        [Fact]
        public void only_the_last_version_of_an_active_feature_is_loaded()
        {
            ContainerBuilder cb = new();
            cb.RegisterAssemblyVersions(
                typeof(TestProjectV1V2V3.Version_1_).Assembly,
                "3.0",
                new TestProjectV1V2V3.Version_3_Dependencies("hey Version 3 need this !!"),
                new ConfigModule(new string[] { "-crudName", "true" }));
            var container = cb.Build();

            container.Resolve<IEnumerable<TestProjectV1V2V3.CName>>().Should().HaveCount(1);
            container.Resolve<IEnumerable<TestProjectV1V2V3.RName>>().Should().HaveCount(1);
            container.Resolve<IEnumerable<TestProjectV1V2V3.UName>>().Should().HaveCount(1);
        }

        [Fact]
        public void you_can_request_a_feature_note_containing_all_feature_with_the_version_of_introduction_and_a_description()
        {
            ContainerBuilder cb = new();
            cb.RegisterAssemblyVersions(
                typeof(TestProjectV1V2V3.Version_1_).Assembly,
                "3.0",
                new TestProjectV1V2V3.Version_3_Dependencies("hey Version 3 need this !!"));
            var container = cb.Build();

            var notes = container.Resolve<IEnumerable<FeatureNotes>>();
            notes.Should().HaveCount(2);
            var appFeatures = string.Join(Environment.NewLine, notes);

            appFeatures.Should().Be(@"FeatureNotes { FeatureName = CrudName, Description = , Version = 2.0 }
FeatureNotes { FeatureName = CrudSurname, Description = to use this feature just request a CSurname to add surname that you will be able to read via the RSurname interfaces.
we don't support neither Delete nor Update for now, Version = 3.0 }");
        }

        private class ConfigModule : Module
        {
            private string[] args;

            public ConfigModule(string[] args)
            {
                this.args = args;
            }

            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);


                IConfiguration cfg = new ConfigurationBuilder()
                    .AddCommandLine(args,
                    new Dictionary<string, string>
                    {
                        { "-crudName", "CrudName" }
                    })
                    .Build();

                builder.RegisterInstance(cfg);
            }
        }
    }
}
