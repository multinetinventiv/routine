using Routine.Core;
using Routine.Interception.Configuration;
using Routine.Interception.Context;
using Routine.Interception;
using Routine.Test.Core;
using Routine.Test.Engine.Stubs.ObjectServiceInvokers;

namespace Routine.Test.Interception;

[TestFixture(typeof(Sync))]
[TestFixture(typeof(Async))]
public class InterceptedObjectServiceTest_Do<TObjectServiceInvoker> : CoreTestBase
    where TObjectServiceInvoker : IObjectServiceInvoker, new()
{
    #region Setup & Helpers

    private Mock<IObjectService> _mock;
    private IObjectServiceInvoker _invoker;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        _mock = new();
        _mock.Setup(os => os.ApplicationModel).Returns(GetApplicationModel);
        _mock.Setup(os => os.Get(It.IsAny<ReferenceData>())).Returns((ReferenceData id) => _objectDictionary[id]);
        _mock.Setup(os => os.GetAsync(It.IsAny<ReferenceData>())).ReturnsAsync((ReferenceData id) => _objectDictionary[id]);

        _invoker = new TObjectServiceInvoker();
    }

    private InterceptedObjectService Build(
        Func<InterceptionConfigurationBuilder, IInterceptionConfiguration> interceptionConfiguration
    ) => new(_mock.Object, interceptionConfiguration(BuildRoutine.InterceptionConfig()));

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
                    Assert.That(ctx.Target, Is.EqualTo($"{InterceptionTarget.Do}"));
                    Assert.That(ctx, Is.InstanceOf<ServiceInterceptionContext>());

                    var sCtx = (ServiceInterceptionContext)ctx;
                    Assert.That(sCtx.TargetReference, Is.EqualTo(Id("id", "model")));
                    Assert.That(sCtx.Model.Id, Is.EqualTo("model"));
                    Assert.That(sCtx.OperationName, Is.EqualTo("operation"));

                    hit = true;
                }
            )))
        );

        _invoker.InvokeDo(testing, Id("id", "model"), "operation", Params());

        Assert.That(hit, Is.True);
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
        _invoker.InvokeGet(testing, Id("id"));
        _invoker.InvokeDo(testing, Id("id"), "operation", Params());

        Assert.That(hitCount, Is.EqualTo(3));
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
        _invoker.InvokeGet(testing, Id("id"));
        _invoker.InvokeDo(testing, Id("id"), "operation", Params());

        Assert.That(hitCount, Is.EqualTo(1));
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

        _invoker.InvokeDo(testing, Id("id", "model-a"), "operation-a", Params());
        _invoker.InvokeDo(testing, Id("id", "model-a"), "operation-b", Params());
        _invoker.InvokeDo(testing, Id("id", "model-b"), "operation-a", Params());

        Assert.That(hitCount, Is.EqualTo(1));
    }

    [Test]
    public void When_intercepting_do_method__do_interceptors_always_come_before_service_interceptors()
    {
        ModelsAre(Model().Operation("operation"));
        ObjectsAre(Object(Id("id")));

        var hitCount = 0;
        var testing = Build(ic => ic.FromBasic()
            .ServiceInterceptors.Add(c => c.Interceptor(i => i
                .Before(() => Assert.That(hitCount++, Is.EqualTo(1)))
            ))
            .Interceptors.Add(c => c.Interceptor(i => i
                .Before(() => hitCount++)
            ))
        );

        _invoker.InvokeDo(testing, Id("id"), "operation", Params());

        Assert.That(hitCount, Is.EqualTo(2));
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

        _invoker.InvokeDo(testing, Id("id"), "operation", Params());

        Assert.That(expected, Is.EqualTo("test"));
    }
}
