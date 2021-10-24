using Moq;
using NUnit.Framework;
using Routine.Core;
using Routine.Interception;
using Routine.Interception.Configuration;
using Routine.Test.Core;
using System;

namespace Routine.Test.Interception
{
    public class Sync { }
    public class Async { }

    [TestFixture(typeof(Sync))]
    [TestFixture(typeof(Async))]
    public class InterceptedObjectServiceTest<TInvocation> : CoreTestBase
    {
        private Mock<IObjectService> mock;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            mock = new Mock<IObjectService>();
            mock.Setup(os => os.ApplicationModel).Returns(GetApplicationModel());
        }

        private InterceptedObjectService InterceptedObjectService(
            Func<InterceptionConfigurationBuilder, IInterceptionConfiguration> interceptionConfiguration
        ) => new(mock.Object, interceptionConfiguration(BuildRoutine.InterceptionConfig()));

        [Test]
        public void ApplicationModel_property_is_intercepted_with_default_context()
        {
            var hit = false;
            var testing = InterceptedObjectService(ic => ic.FromBasic()
                .Interceptors.Add(c => c.Interceptor(i => i.Before(ctx =>
                    {
                        Assert.AreEqual($"{InterceptionTarget.ApplicationModel}", ctx.Target);

                        hit = true;
                    }
                )))
            );

            var _ = testing.ApplicationModel;

            Assert.IsTrue(hit);
        }

        [Test]
        public void Get_method_is_intercepted_with_object_reference_context()
        {
            Assert.Fail();
        }

        [Test]
        public void Do_method_is_intercepted_with_service_context()
        {
            Assert.Fail();
        }

        [Test]
        public void An_interceptor_can_be_defined_for_all_three_methods()
        {
            Assert.Fail();
        }

        [Test]
        public void An_interceptor_can_be_defined_for_a_specific_method()
        {
            Assert.Fail();
        }

        [Test]
        public void Service_interceptors_can_be_defined_for_a_specific_target_model_and_or_operation_model()
        {
            Assert.Fail();
        }

        [Test]
        public void Service_interceptors_use_the_same_context_with_do_interceptors_within_the_same_invocation()
        {
            Assert.Fail();
        }

        [Test]
        public void When_intercepting_do_method__do_interceptors_always_come_before_service_interceptors()
        {
            Assert.Fail();
        }
    }
}
