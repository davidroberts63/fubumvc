using System;
using Fubu.Applications;
using FubuMVC.Core;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;

namespace FubuMVC.Tests.Applications
{
    [TestFixture]
    public class ApplicationRunnerTester : InteractionContext<ApplicationRunner>
    {
        private IApplicationSourceFinder theFinder;
        private ApplicationStartResponse theResponse;

        protected override void beforeEach()
        {
            theFinder = MockFor<IApplicationSourceFinder>();
            theResponse = new ApplicationStartResponse();
        }

        [Test]
        public void find_the_application_source_when_it_is_explicitly_defined_by_the_settings()
        {
            var settings = new ApplicationSettings{
                ApplicationSourceName = typeof(Source1).AssemblyQualifiedName
            };


            ClassUnderTest.FindSource(settings, theResponse).ShouldBeOfType<Source1>();
            theResponse.ApplicationSourceTypes.ShouldBeNull();
        }

        [Test]
        public void find_the_application_source_is_null_if_there_are_no_sources()
        {
            var settings = new ApplicationSettings
            {
                ApplicationSourceName = null
            };

            theFinder.Stub(x => x.FindApplicationSourceTypes())
                .Return(new Type[0]);

            ClassUnderTest.FindSource(settings, theResponse).ShouldBeNull();

            theResponse.ApplicationSourceTypes.Any().ShouldBeFalse();
        }

        [Test]
        public void find_the_application_source_when_it_is_not_in_the_settings_but_only_one_source_can_be_found()
        {
            var settings = new ApplicationSettings{
                ApplicationSourceName = null
            };

            theFinder.Stub(x => x.FindApplicationSourceTypes())
                .Return(new Type[]{typeof (Source2)});

            ClassUnderTest.FindSource(settings, theResponse).ShouldBeOfType<Source2>();
        }


        [Test]
        public void find_the_application_source_with_multiple_sources_returned_no_explicit_setting_but_name_matches()
        {
            var settings = new ApplicationSettings
            {
                ApplicationSourceName = null,
                Name = typeof(Source3).Name
            };

            var sourceTypes = new Type[] { typeof(Source1), typeof(Source2), typeof(Source3) };
            theFinder.Stub(x => x.FindApplicationSourceTypes())
                .Return(sourceTypes);

            ClassUnderTest.FindSource(settings, theResponse).ShouldBeOfType<Source3>();

            theResponse.ApplicationSourceTypes
                .ShouldHaveTheSameElementsAs(sourceTypes.Select(x => x.AssemblyQualifiedName));
        }
    }


    public class Source1 : IApplicationSource{
        public FubuApplication BuildApplication()
        {
            throw new NotImplementedException();
        }
    }

    public class Source2 : Source1{}
    public class Source3 : Source1{}
    public class Source4 : Source1{}
    public class Source5 : Source1{}
}