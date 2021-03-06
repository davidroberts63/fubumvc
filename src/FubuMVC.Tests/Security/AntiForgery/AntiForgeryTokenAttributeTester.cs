using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security.AntiForgery;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.Security.AntiForgery
{
    [TestFixture]
    public class AntiForgeryTokenAttributeTester
    {


        [Test]
        public void should_put_an_anti_forgery_token_on_the_chain()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<Controller1>();

            registry.BuildGraph().BehaviorFor<Controller1>(x => x.MethodWithAF(null))
                .FirstCall()
                .Previous.ShouldBeOfType<AntiForgeryNode>()
                .Salt.ShouldEqual("abc");
                
        }


        public class Controller1
        {
            [AntiForgeryToken(Salt = "abc")]
            public Output1 MethodWithAF(Input1 input)
            {
                return null;
            }

            public Output1 MethodWithoutAF(Input1 input)
            {
                return null;
            }
        }

        public class Input1{}
        public class Output1{}
    }
}