using Routine.Core;
using Routine.Interception.Configuration;
using Routine.Interception.Context;
using Routine.Interception;
using Routine.Test.Core;
using Routine.Test.Engine.Stubs.ObjectServiceInvokers;

namespace Routine.Test.Interception;

[TestFixture(typeof(Sync))]
[TestFixture(typeof(Async))]
public class InterceptedObjectServiceTest<TObjectServiceInvoker> : CoreTestBase
    where TObjectServiceInvoker : IObjectServiceInvoker, new()
{
    private Mock<IObjectService> _mock;
    private IObjectServiceInvoker _invoker;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        _mock = new Mock<IObjectService>();
        _mock.Setup(os => os.ApplicationModel).Returns(GetApplicationModel);
        _mock.Setup(os => os.Get(It.IsAny<ReferenceData>())).Returns((ReferenceData id) => _objectDictionary[id]);
        _mock.Setup(os => os.GetAsync(It.IsAny<ReferenceData>())).ReturnsAsync((ReferenceData id) => _objectDictionary[id]);

        _invoker = new TObjectServiceInvoker();
    }

    private InterceptedObjectService Build(
        Func<InterceptionConfigurationBuilder, IInterceptionConfiguration> interceptionConfiguration
    ) => new(_mock.Object, interceptionConfiguration(BuildRoutine.InterceptionConfig()));

    [Test]
    public void ApplicationModel_property_is_intercepted_with_default_context()
    {
        var hit = false;
        var testing = Build(ic => ic.FromBasic()
            .Interceptors.Add(c => c.Interceptor(i => i.Before(ctx =>
                {
                    Assert.That(ctx.Target, Is.EqualTo($"{InterceptionTarget.ApplicationModel}"));
                    Assert.That(ctx, Is.InstanceOf<InterceptionContext>());

                    hit = true;
                }
            )))
        );

        var _ = testing.ApplicationModel;

        Assert.That(hit, Is.True);
    }

    [Test]
    public void Get_method_is_intercepted_with_object_reference_context()
    {
        ModelsAre(Model("model"));
        ObjectsAre(Object(Id("id", "model")));

        var hit = false;

        var testing = Build(ic => ic.FromBasic()
            .Interceptors.Add(c => c.Interceptor(i => i.Before(ctx =>
                {
                    Assert.That(ctx.Target, Is.EqualTo($"{InterceptionTarget.Get}"));
                    Assert.That(ctx, Is.InstanceOf<ObjectReferenceInterceptionContext>());

                    var orCtx = (ObjectReferenceInterceptionContext)ctx;
                    Assert.That(orCtx.TargetReference, Is.EqualTo(Id("id", "model")));
                    Assert.That(orCtx.Model.Id, Is.EqualTo("model"));

                    hit = true;
                }
            )))
        );

        _invoker.InvokeGet(testing, Id("id", "model"));

        Assert.That(hit, Is.True);
    }

    [Test]
    public void An_interceptor_can_be_defined_to_both_methods()
    {
        ModelsAre(Model());
        ObjectsAre(Object(Id("id")));

        var hitCount = 0;

        var testing = Build(ic => ic.FromBasic()
            .Interceptors.Add(c => c.Interceptor(i => i.Before(() => hitCount++)))
        );

        var _ = testing.ApplicationModel;
        _invoker.InvokeGet(testing, Id("id"));

        Assert.That(hitCount, Is.EqualTo(2));
    }

    [Test]
    public void An_interceptor_can_be_defined_for_a_specific_method()
    {
        ModelsAre(Model());
        ObjectsAre(Object(Id("id")));

        var hitCount = 0;

        var testing = Build(ic => ic.FromBasic()
            .Interceptors.Add(c => c
                .Interceptor(i => i.Before(() => hitCount++))
                .When(InterceptionTarget.Get))
        );

        var _ = testing.ApplicationModel;
        _invoker.InvokeGet(testing, Id("id"));

        Assert.That(hitCount, Is.EqualTo(1));
    }
}
