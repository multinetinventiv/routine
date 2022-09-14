using Routine.Core;
using Routine.Interception.Configuration;
using Routine.Interception.Context;
using Routine.Interception;
using Routine.Test.Core;
using Routine.Test.Engine.Stubs.DoInvokers;

namespace Routine.Test.Interception;

[TestFixture(typeof(Sync))]
[TestFixture(typeof(Async))]
public class InterceptedObjectServiceTest_Do<TDoInvoker> : CoreTestBase
    where TDoInvoker : IDoInvoker, new()
{
    #region Setup & Helpers

    private Mock<IObjectService> mock;
    private IDoInvoker invoker;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        mock = new Mock<IObjectService>();
        mock.Setup(os => os.ApplicationModel).Returns(GetApplicationModel);
        mock.Setup(os => os.Get(It.IsAny<ReferenceData>())).Returns((ReferenceData id) => objectDictionary[id]);

        invoker = new TDoInvoker();
    }

    private InterceptedObjectService Build(
        Func<InterceptionConfigurationBuilder, IInterceptionConfiguration> interceptionConfiguration
    ) => new(mock.Object, interceptionConfiguration(BuildRoutine.InterceptionConfig()));

    #endregion

    [Test]
    public void Do_method_is_intercepted_with_service_context()
    {
        ModelsAre(Model("model").Operation("operation"));
        ObjectsAre(Object(Id("id", "model")));

        var hit = false;

        var testing = Build(ic => ic.FromBasic()
            .Interceptors.Add(c => c.Interceptor(i => i.Before(ctx =>
                {
                    Assert.AreEqual($"{InterceptionTarget.Do}", ctx.Target);
                    Assert.IsInstanceOf<ServiceInterceptionContext>(ctx);

                    var sCtx = (ServiceInterceptionContext)ctx;
                    Assert.AreEqual(Id("id", "model"), sCtx.TargetReference);
                    Assert.AreEqual("model", sCtx.Model.Id);
                    Assert.AreEqual("operation", sCtx.OperationName);

                    hit = true;
                }
            )))
        );

        invoker.InvokeDo(testing, Id("id", "model"), "operation", Params());

        Assert.IsTrue(hit);
    }

    [Test]
    public void An_interceptor_can_be_defined_for_all_three_methods()
    {
        ModelsAre(Model().Operation("operation"));
        ObjectsAre(Object(Id("id")));

        var hitCount = 0;

        var testing = Build(ic => ic.FromBasic()
            .Interceptors.Add(c => c.Interceptor(i => i.Before(() => hitCount++)))
        );

        var _ = testing.ApplicationModel;
        testing.Get(Id("id"));
        invoker.InvokeDo(testing, Id("id"), "operation", Params());

        Assert.AreEqual(3, hitCount);
    }

    [Test]
    public void An_interceptor_can_be_defined_for_a_specific_method()
    {
        ModelsAre(Model().Operation("operation"));
        ObjectsAre(Object(Id("id")));

        var hitCount = 0;

        var testing = Build(ic => ic.FromBasic()
            .Interceptors.Add(c => c
                .Interceptor(i => i.Before(() => hitCount++))
                .When(InterceptionTarget.Do)
            )
        );

        var _ = testing.ApplicationModel;
        testing.Get(Id("id"));
        invoker.InvokeDo(testing, Id("id"), "operation", Params());

        Assert.AreEqual(1, hitCount);
    }

    [Test]
    public void Service_interceptors_can_be_defined_for_a_specific_target_model_and_or_operation_model()
    {
        ModelsAre(
            Model("model-a").Operation("operation-a").Operation("operation-b"),
            Model("model-b").Operation("operation-a")
        );
        ObjectsAre(
            Object(Id("id", "model-a")),
            Object(Id("id", "model-b"))
        );

        var hitCount = 0;

        var testing = Build(ic => ic.FromBasic()
            .ServiceInterceptors.Add(c => c
                .Interceptor(i => i.Before(() => hitCount++))
                .When(owom => owom.ObjectModel.Id == "model-a" && owom.OperationModel.Name == "operation-a")
            )
        );

        invoker.InvokeDo(testing, Id("id", "model-a"), "operation-a", Params());
        invoker.InvokeDo(testing, Id("id", "model-a"), "operation-b", Params());
        invoker.InvokeDo(testing, Id("id", "model-b"), "operation-a", Params());

        Assert.AreEqual(1, hitCount);
    }

    [Test]
    public void When_intercepting_do_method__do_interceptors_always_come_before_service_interceptors()
    {
        ModelsAre(Model().Operation("operation"));
        ObjectsAre(Object(Id("id")));

        var hitCount = 0;
        var testing = Build(ic => ic.FromBasic()
            .ServiceInterceptors.Add(c => c.Interceptor(i => i
                .Before(() => Assert.AreEqual(1, hitCount++))
            ))
            .Interceptors.Add(c => c.Interceptor(i => i
                .Before(() => hitCount++)
            ))
        );

        invoker.InvokeDo(testing, Id("id"), "operation", Params());

        Assert.AreEqual(2, hitCount);
    }

    [Test]
    public void Service_interceptors_use_the_same_context_with_do_interceptors_within_the_same_invocation()
    {
        ModelsAre(Model().Operation("operation"));
        ObjectsAre(Object(Id("id")));

        var expected = string.Empty;

        var testing = Build(ic => ic.FromBasic()
            .Interceptors.Add(c => c.Interceptor(i => i
                .Before(ctx => ctx["expected"] = "test")
            ))
            .ServiceInterceptors.Add(c => c.Interceptor(i => i
                .Before(ctx => expected = (string)ctx["expected"])
            ))
        );

        invoker.InvokeDo(testing, Id("id"), "operation", Params());

        Assert.AreEqual("test", expected);
    }
}
