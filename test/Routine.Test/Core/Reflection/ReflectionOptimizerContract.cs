using Routine.Core.Reflection;
using System.Reflection;

namespace Routine.Test.Core.Reflection;

#region Test Model

public interface IOptimizedInterface<T>
{
    void VoidMethod();

    void ExplicitVoidMethod();
    string StringMethod();
    void OneParameterVoidMethod(string str);
    void TwoParameterVoidMethod(string str, int i);
    string ThreeParameterStringMethod(string str, int i, decimal d);

    List<string> ListMethod();
    void ListParameterVoidMethod(List<string> list);
    void ListListParameterVoidMethod(List<List<string>> listList);
    void DictionaryParameterVoidMethod(Dictionary<string, object> dict);

    string StringProperty { get; set; }

    void Overload();
    void Overload(int i);

    string this[string key, int index] { get; set; }

    void GenericParameterMethod(T param);
    T GenericReturnMethod();

    void PrivateVoidMethod();

    Task AsyncVoidMethod();
    Task<string> AsyncStringMethod();
}

public class OptimizedClass : IOptimizedInterface<string>
{
    public class InnerClass
    {
        public static InnerClass New() => new();
        private InnerClass() { }
        public void VoidMethod() { }
    }

    public static void StaticVoidMethod() { }

    private readonly IOptimizedInterface<string> _real;
    public OptimizedClass(IOptimizedInterface<string> real) => _real = real;

    public void VoidMethod() => _real.VoidMethod();

    void IOptimizedInterface<string>.ExplicitVoidMethod() => _real.ExplicitVoidMethod();

    public string StringMethod() => _real.StringMethod();
    public void OneParameterVoidMethod(string str) => _real.OneParameterVoidMethod(str);
    public void TwoParameterVoidMethod(string str, int i) => _real.TwoParameterVoidMethod(str, i);
    public string ThreeParameterStringMethod(string str, int i, decimal d) => _real.ThreeParameterStringMethod(str, i, d);

    public List<string> ListMethod() => _real.ListMethod();
    public void ListParameterVoidMethod(List<string> list) => _real.ListParameterVoidMethod(list);
    public void ListListParameterVoidMethod(List<List<string>> listList) => _real.ListListParameterVoidMethod(listList);
    public void DictionaryParameterVoidMethod(Dictionary<string, object> dict) => _real.DictionaryParameterVoidMethod(dict);

    // ReSharper disable once UnusedMember.Local
    private void NonPublicVoidMethod() { }

    public string StringProperty { get => _real.StringProperty; set => _real.StringProperty = value; }

    public void Overload() => _real.Overload();
    public void Overload(int i) => _real.Overload(i);

    public string this[string key, int index]
    {
        get => _real[key, index];
        set => _real[key, index] = value;
    }

    public void GenericParameterMethod(string param) => _real.GenericParameterMethod(param);
    public string GenericReturnMethod() => _real.GenericReturnMethod();

    private void PrivateVoidMethod() => _real.PrivateVoidMethod();
    void IOptimizedInterface<string>.PrivateVoidMethod() => PrivateVoidMethod();

    public async Task AsyncVoidMethod() => await _real.AsyncVoidMethod();
    public async Task<string> AsyncStringMethod() => await _real.AsyncStringMethod();
}

public struct Struct
{
    public string Property { get; set; }
}

public record Record(string Data);

#endregion

public abstract class ReflectionOptimizerContract : CoreTestBase
{
    #region Setup & Helpers

    private const BindingFlags ALL_MEMBERS = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
    private const BindingFlags ALL_INSTANCE_MEMBERS = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    protected Mock<IOptimizedInterface<string>> _mock;
    protected OptimizedClass _target;

    public override void SetUp()
    {
        base.SetUp();

        ReflectionOptimizer.Enable();

        _mock = new();

        _target = new(_mock.Object);
    }

    protected IMethodInvoker InvokerFor<T>(string methodName, int? parameterCount = null, bool waitForOptimization = true)
    {
        IMethodInvoker result;
        if (methodName.StartsWith("get:"))
        {
            result = typeof(T).GetProperty(methodName.After("get:"), ALL_MEMBERS)
                ?.GetGetMethod().CreateInvoker();
        }
        else if (methodName.StartsWith("set:"))
        {
            result = typeof(T).GetProperty(methodName.After("set:"), ALL_MEMBERS)
                ?.GetSetMethod().CreateInvoker();
        }
        else if (methodName == "new")
        {
            result = typeof(T).GetConstructors(ALL_INSTANCE_MEMBERS)
                .FirstOrDefault(ci => parameterCount == null || ci.GetParameters().Length == parameterCount)
                .CreateInvoker();
        }
        else
        {
            result = typeof(T).GetMethods(ALL_MEMBERS)
                .FirstOrDefault(mi => mi.Name == methodName && (parameterCount == null || mi.GetParameters().Length == parameterCount))
                .CreateInvoker();
        }

        if (waitForOptimization)
        {
            WaitForOptimization(result);
        }

        return result;
    }

    private void WaitForOptimization(IMethodInvoker invoker)
    {
        if (invoker is not ProxyMethodInvoker proxy || proxy.Real is not SwitchableMethodInvoker switchable) { return; }

        const int timeout = 5000;

        var count = 0;
        var optimized = switchable.Invoker is not ReflectionMethodInvoker;
        var onOptimized = new EventHandler((_, _) => optimized = true);
        ReflectionOptimizer.Optimized += onOptimized;
        while (!optimized && count < timeout)
        {
            Thread.Sleep(1);
            count++;
        }
        ReflectionOptimizer.Optimized -= onOptimized;
    }

    #endregion

    protected abstract object Invoke(IMethodInvoker invoker, object target, params object[] args);

    [Test]
    public void ReflectionOptimizer_generates_and_compiles_code_to_invoke_given_method_via_invoker_interface()
    {
        Invoke(InvokerFor<IOptimizedInterface<string>>("VoidMethod"), _target);
        Invoke(InvokerFor<OptimizedClass>("VoidMethod"), _target);
        Invoke(InvokerFor<IOptimizedInterface<string>>("ExplicitVoidMethod"), _target);

        _mock.Verify(o => o.ExplicitVoidMethod(), Times.Once());
        _mock.Verify(o => o.VoidMethod(), Times.Exactly(2));
    }

    [Test]
    public void CreateInvoker_returns_a_proxy_invoker_that_has_reflection_invoker_for_app_to_have_faster_startup()
    {
        ReflectionOptimizer.Clear();

        var proxy = (ProxyMethodInvoker)InvokerFor<IOptimizedInterface<string>>("VoidMethod", waitForOptimization: false);
        var switchable = (SwitchableMethodInvoker)proxy.Real;

        Assert.That(switchable.Invoker, Is.InstanceOf<ReflectionMethodInvoker>());

        WaitForOptimization(proxy);

        Assert.That(switchable.Invoker, Is.Not.InstanceOf<ReflectionMethodInvoker>());
    }

    [Test]
    public void When_disabled__it_does_not_optimize()
    {
        ReflectionOptimizer.Disable();

        var invoker = InvokerFor<OptimizedClass>("VoidMethod") as ProxyMethodInvoker;

        Assert.That(invoker, Is.Not.Null);
        Assert.That(invoker.Real, Is.InstanceOf<ReflectionMethodInvoker>());
    }

    [Test]
    public void CreateInvoker_throws_ArgumentException_when_null_is_given()
    {
        Assert.That(() => InvokerFor<IOptimizedInterface<string>>("NonExistingMethod"),
            Throws.Exception
                .With.Property("ParamName").EqualTo("method")
        );
    }

    [Test]
    public void Test_void_method()
    {
        var actual = Invoke(InvokerFor<IOptimizedInterface<string>>("VoidMethod"), _target);

        Assert.That(actual, Is.Null);
        _mock.Verify(o => o.VoidMethod(), Times.Once());
    }

    [Test]
    public void Test_return_value()
    {
        _mock.Setup(o => o.StringMethod()).Returns("result");

        var actual = Invoke(InvokerFor<OptimizedClass>("StringMethod"), _target);

        Assert.That(actual, Is.EqualTo("result"));
    }

    [Test]
    public void Test_generic_type_return()
    {
        Invoke(InvokerFor<OptimizedClass>("ListMethod"), _target);
    }

    [Test]
    public void Test_parameter()
    {
        Invoke(InvokerFor<OptimizedClass>("OneParameterVoidMethod"), _target, "test");
        Invoke(InvokerFor<OptimizedClass>("TwoParameterVoidMethod"), _target, "test1", 1);
        Invoke(InvokerFor<OptimizedClass>("ThreeParameterStringMethod"), _target, "test2", 2, 0.2m);

        _mock.Verify(o => o.OneParameterVoidMethod("test"), Times.Once());
        _mock.Verify(o => o.TwoParameterVoidMethod("test1", 1), Times.Once());
        _mock.Verify(o => o.ThreeParameterStringMethod("test2", 2, 0.2m), Times.Once());
    }

    [Test]
    public void When_null_is_given_for_a_value_type_parameter__default_value_of_that_type_is_used()
    {
        Invoke(InvokerFor<OptimizedClass>("TwoParameterVoidMethod"), _target, "dummy", null);

        _mock.Verify(o => o.TwoParameterVoidMethod(It.IsAny<string>(), 0), Times.Once());
    }

    [Test]
    public void Method_invoker_does_not_check_parameter_compatibility_and_let_IndexOutOfRangeException_or_InvalidCastException_to_be_thrown()
    {
        Assert.That(() => Invoke(InvokerFor<OptimizedClass>("OneParameterVoidMethod"), _target),
            Throws.TypeOf<IndexOutOfRangeException>()
        );
        Assert.That(() => Invoke(InvokerFor<OptimizedClass>("OneParameterVoidMethod"), _target, 0),
            Throws.TypeOf<InvalidCastException>()
        );
    }

    [Test]
    public void Test_generic_type_parameter()
    {
        Invoke(InvokerFor<OptimizedClass>("ListParameterVoidMethod"), _target, new List<string>());
        Invoke(InvokerFor<OptimizedClass>("ListListParameterVoidMethod"), _target, new List<List<string>>());
        Invoke(InvokerFor<OptimizedClass>("DictionaryParameterVoidMethod"), _target, new Dictionary<string, object>());
    }

    [Test]
    public void Test_generic_method()
    {
        Invoke(InvokerFor<IOptimizedInterface<string>>("GenericParameterMethod"), _target, "param");

        _mock.Verify(o => o.GenericParameterMethod("param"));

        _mock.Setup(o => o.GenericReturnMethod()).Returns("result");

        var actual = Invoke(InvokerFor<IOptimizedInterface<string>>("GenericReturnMethod"), _target) as string;

        Assert.That(actual, Is.EqualTo("result"));
    }

    [Test]
    public void Test_static_method()
    {
        Invoke(InvokerFor<OptimizedClass>("StaticVoidMethod"), null);
    }

    [Test]
    public void Test_inner_class()
    {
        Invoke(InvokerFor<OptimizedClass.InnerClass>("VoidMethod"), OptimizedClass.InnerClass.New());
    }

    [Test]
    public void Test_property_get()
    {
        _mock.Setup(o => o.StringProperty).Returns("result");

        var actual = Invoke(InvokerFor<OptimizedClass>("get:StringProperty"), _target);

        Assert.That(actual, Is.EqualTo("result"));
    }

    [Test]
    public void Test_property_set()
    {
        Invoke(InvokerFor<OptimizedClass>("set:StringProperty"), _target, "result");

        _mock.VerifySet(p => p.StringProperty = "result", Times.Once());
    }

    [Test]
    public void Struct_property_setters_are_not_optimized_because_they_are_value_type_and_we_are_not_allowed_to_unbox_a_value_type_and_set_its_property()
    {
        var invoker = typeof(Struct).GetProperty("Property")?.GetSetMethod().CreateInvoker() as ProxyMethodInvoker;

        Assert.That(invoker, Is.Not.Null);
        Assert.That(invoker.Real, Is.InstanceOf<ReflectionMethodInvoker>());
    }

    [Test]
    public void Test_property_get_by_index()
    {
        _mock.Setup(o => o["key", 0]).Returns("result");

        var actual = Invoke(InvokerFor<OptimizedClass>("get:Item"), _target, "key", 0);

        Assert.That(actual, Is.EqualTo("result"));
    }

    [Test]
    public void Test_property_set_by_index()
    {
        Invoke(InvokerFor<OptimizedClass>("set:Item"), _target, "key", 0, "result");

        _mock.VerifySet(o => o["key", 0] = "result", Times.Once());
    }

    [Test]
    public void Test_create_object()
    {
        var actual = Invoke(InvokerFor<OptimizedClass>("new"), null, _mock.Object);
        Assert.That(actual, Is.InstanceOf<OptimizedClass>());
    }

    [Test]
    public void For_non_public_methods_ReflectionInvoker_is_created()
    {
        var proxy = InvokerFor<OptimizedClass>("NonPublicVoidMethod") as ProxyMethodInvoker;

        Assert.That(proxy, Is.Not.Null);
        Assert.That(proxy.Real, Is.InstanceOf<ReflectionMethodInvoker>());

        proxy = InvokerFor<OptimizedClass>("VoidMethod") as ProxyMethodInvoker;

        Assert.That(proxy, Is.Not.Null);
        Assert.That(proxy.Real, Is.Not.InstanceOf<ReflectionMethodInvoker>());
    }

    [Test]
    public void For_methods_with_ref_parameters_ReflectionInvoker_is_created()
    {
        var proxy = InvokerFor<Uri>("HexUnescape") as ProxyMethodInvoker;

        Assert.That(proxy, Is.Not.Null);
        Assert.That(proxy.Real, Is.InstanceOf<ReflectionMethodInvoker>());
    }

    [Test]
    public void For_the_clone_method_of_records_ReflectionInvoker_is_created()
    {
        var proxy = InvokerFor<Record>("<Clone>$") as ProxyMethodInvoker;

        Assert.That(proxy, Is.Not.Null);
        Assert.That(proxy.Real, Is.InstanceOf<ReflectionMethodInvoker>());
    }

    [Test]
    public void For_record_property_setters_ReflectionInvoker_is_created()
    {
        var proxy = InvokerFor<Record>("set:Data") as ProxyMethodInvoker;

        Assert.That(proxy, Is.Not.Null);
        Assert.That(proxy.Real, Is.InstanceOf<ReflectionMethodInvoker>());
    }

    [Test]
    public void ReflectionOptimizer_uses_reflection_when_given_method_is_not_public()
    {
        Invoke(InvokerFor<OptimizedClass>("PrivateVoidMethod"), _target);

        _mock.Verify(o => o.PrivateVoidMethod(), Times.Once());

        Invoke(InvokerFor<OptimizedClass.InnerClass>("new"), _target);
    }

    [Test]
    public void ReflectionOptimizer_collects_methods_and_compiles_at_once()
    {
        var invokers = new List<IMethodInvoker>
        {
            InvokerFor<OptimizedClass>("ListParameterVoidMethod"),
            InvokerFor<OptimizedClass>("PrivateVoidMethod"),
            InvokerFor<OptimizedClass>("get:StringProperty"),
            InvokerFor<OptimizedClass>("set:StringProperty"),
            InvokerFor<OptimizedClass>("Overload", 0),
            InvokerFor<OptimizedClass>("Overload", 1)
        };

        Invoke(invokers[0], _target, new List<string>());
        Invoke(invokers[1], _target);
        Invoke(invokers[2], _target);
        Invoke(invokers[3], _target, "test");
        Invoke(invokers[4], _target);
        Invoke(invokers[5], _target, 1);

        _mock.Verify(o => o.ListParameterVoidMethod(It.IsAny<List<string>>()), Times.Once());
        _mock.Verify(o => o.PrivateVoidMethod(), Times.Once());
        _mock.VerifyGet(o => o.StringProperty, Times.Once());
        _mock.VerifySet(o => o.StringProperty = It.IsAny<string>(), Times.Once());
        _mock.Verify(o => o.Overload(), Times.Once());
        _mock.Verify(o => o.Overload(It.IsAny<int>()), Times.Once());
    }

    [TestCase(nameof(OptimizedClass.VoidMethod))]
    [TestCase(nameof(OptimizedClass.AsyncVoidMethod))]
    public void Throws_exception_without_any_change(string method)
    {
        _mock.Setup(m => m.VoidMethod()).Throws(new Exception("test"));
        _mock.Setup(m => m.AsyncVoidMethod()).ThrowsAsync(new Exception("test"));

        var testing = InvokerFor<OptimizedClass>(method);

        Assert.That(() => Invoke(testing, _target),
            Throws.Exception.With.Property("Message").EqualTo("test")
        );
    }
}
