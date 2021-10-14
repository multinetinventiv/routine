using Moq;
using NUnit.Framework;
using Routine.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Routine.Test.Core.Reflection
{
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

        private readonly IOptimizedInterface<string> real;
        public OptimizedClass(IOptimizedInterface<string> real) => this.real = real;

        public void VoidMethod() => real.VoidMethod();

        void IOptimizedInterface<string>.ExplicitVoidMethod() => real.ExplicitVoidMethod();

        public string StringMethod() => real.StringMethod();
        public void OneParameterVoidMethod(string str) => real.OneParameterVoidMethod(str);
        public void TwoParameterVoidMethod(string str, int i) => real.TwoParameterVoidMethod(str, i);
        public string ThreeParameterStringMethod(string str, int i, decimal d) => real.ThreeParameterStringMethod(str, i, d);

        public List<string> ListMethod() => real.ListMethod();
        public void ListParameterVoidMethod(List<string> list) => real.ListParameterVoidMethod(list);
        public void ListListParameterVoidMethod(List<List<string>> listList) => real.ListListParameterVoidMethod(listList);
        public void DictionaryParameterVoidMethod(Dictionary<string, object> dict) => real.DictionaryParameterVoidMethod(dict);

        // ReSharper disable once UnusedMember.Local
        private void NonPublicVoidMethod() { }

        public string StringProperty { get => real.StringProperty; set => real.StringProperty = value; }

        public void Overload() => real.Overload();
        public void Overload(int i) => real.Overload(i);

        public string this[string key, int index]
        {
            get => real[key, index];
            set => real[key, index] = value;
        }

        public void GenericParameterMethod(string param) => real.GenericParameterMethod(param);
        public string GenericReturnMethod() => real.GenericReturnMethod();

        private void PrivateVoidMethod() => real.PrivateVoidMethod();
        void IOptimizedInterface<string>.PrivateVoidMethod() => PrivateVoidMethod();

        public async Task AsyncVoidMethod() => await real.AsyncVoidMethod();
        public async Task<string> AsyncStringMethod() => await real.AsyncStringMethod();
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

        protected Mock<IOptimizedInterface<string>> mock;
        protected OptimizedClass target;

        public override void SetUp()
        {
            base.SetUp();

            ReflectionOptimizer.Enable();

            mock = new Mock<IOptimizedInterface<string>>();

            target = new OptimizedClass(mock.Object);
        }

        protected IMethodInvoker InvokerFor<T>(string methodName) { return InvokerFor<T>(methodName, null); }
        protected IMethodInvoker InvokerFor<T>(string methodName, int? parameterCount)
        {
            if (methodName.StartsWith("get:"))
            {
                return typeof(T).GetProperty(methodName.After("get:"), ALL_MEMBERS)
                    ?.GetGetMethod().CreateInvoker();
            }

            if (methodName.StartsWith("set:"))
            {
                return typeof(T).GetProperty(methodName.After("set:"), ALL_MEMBERS)
                    ?.GetSetMethod().CreateInvoker();
            }

            if (methodName == "new")
            {
                return typeof(T).GetConstructors(ALL_INSTANCE_MEMBERS)
                    .FirstOrDefault(ci => parameterCount == null || ci.GetParameters().Length == parameterCount)
                    .CreateInvoker();
            }

            return typeof(T).GetMethods(ALL_MEMBERS)
                .FirstOrDefault(mi => mi.Name == methodName && (parameterCount == null || mi.GetParameters().Length == parameterCount))
                .CreateInvoker();
        }

        #endregion

        protected abstract object Invoke(IMethodInvoker invoker, object target, params object[] args);

        [Test]
        public void ReflectionOptimizer_generates_and_compiles_code_to_invoke_given_method_via_invoker_interface()
        {
            Invoke(InvokerFor<IOptimizedInterface<string>>("VoidMethod"), target);
            Invoke(InvokerFor<OptimizedClass>("VoidMethod"), target);
            Invoke(InvokerFor<IOptimizedInterface<string>>("ExplicitVoidMethod"), target);

            mock.Verify(o => o.ExplicitVoidMethod(), Times.Once());
            mock.Verify(o => o.VoidMethod(), Times.Exactly(2));
        }

        [Test]
        public void When_disabled__it_does_not_optimize()
        {
            ReflectionOptimizer.Disable();

            var invoker = InvokerFor<OptimizedClass>("VoidMethod") as ProxyMethodInvoker;

            Assert.IsNotNull(invoker);
            Assert.IsInstanceOf<ReflectionMethodInvoker>(invoker.Real);
        }

        [Test]
        public void CreateInvoker_throws_ArgumentException_when_null_is_given()
        {
            try
            {
                InvokerFor<IOptimizedInterface<string>>("NonExistingMethod");
                Assert.Fail("exception not thrown");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("method", ex.ParamName, ex.ToString());
            }
        }

        [Test]
        public void Test_void_method()
        {
            var actual = Invoke(InvokerFor<IOptimizedInterface<string>>("VoidMethod"), target);

            Assert.IsNull(actual);
            mock.Verify(o => o.VoidMethod(), Times.Once());
        }

        [Test]
        public void Test_return_value()
        {
            mock.Setup(o => o.StringMethod()).Returns("result");

            var actual = Invoke(InvokerFor<OptimizedClass>("StringMethod"), target);

            Assert.AreEqual("result", actual);
        }

        [Test]
        public void Test_generic_type_return()
        {
            Invoke(InvokerFor<OptimizedClass>("ListMethod"), target);
        }

        [Test]
        public void Test_parameter()
        {
            Invoke(InvokerFor<OptimizedClass>("OneParameterVoidMethod"), target, "test");
            Invoke(InvokerFor<OptimizedClass>("TwoParameterVoidMethod"), target, "test1", 1);
            Invoke(InvokerFor<OptimizedClass>("ThreeParameterStringMethod"), target, "test2", 2, 0.2m);

            mock.Verify(o => o.OneParameterVoidMethod("test"), Times.Once());
            mock.Verify(o => o.TwoParameterVoidMethod("test1", 1), Times.Once());
            mock.Verify(o => o.ThreeParameterStringMethod("test2", 2, 0.2m), Times.Once());
        }

        [Test]
        public void When_null_is_given_for_a_value_type_parameter__default_value_of_that_type_is_used()
        {
            Invoke(InvokerFor<OptimizedClass>("TwoParameterVoidMethod"), target, "dummy", null);

            mock.Verify(o => o.TwoParameterVoidMethod(It.IsAny<string>(), 0), Times.Once());
        }

        [Test]
        public void Method_invoker_does_not_check_parameter_compatibility_and_let_IndexOutOfRangeException_or_InvalidCastException_to_be_thrown()
        {
            try
            {
                Invoke(InvokerFor<OptimizedClass>("OneParameterVoidMethod"), target);
                Assert.Fail("exception not thrown");
            }
            catch (IndexOutOfRangeException) { }

            try
            {
                Invoke(InvokerFor<OptimizedClass>("OneParameterVoidMethod"), target, 0);
                Assert.Fail("exception not thrown");
            }
            catch (InvalidCastException) { }
        }

        [Test]
        public void Test_generic_type_parameter()
        {
            Invoke(InvokerFor<OptimizedClass>("ListParameterVoidMethod"), target, new List<string>());
            Invoke(InvokerFor<OptimizedClass>("ListListParameterVoidMethod"), target, new List<List<string>>());
            Invoke(InvokerFor<OptimizedClass>("DictionaryParameterVoidMethod"), target, new Dictionary<string, object>());
        }

        [Test]
        public void Test_generic_method()
        {
            Invoke(InvokerFor<IOptimizedInterface<string>>("GenericParameterMethod"), target, "param");

            mock.Verify(o => o.GenericParameterMethod("param"));

            mock.Setup(o => o.GenericReturnMethod()).Returns("result");

            var actual = Invoke(InvokerFor<IOptimizedInterface<string>>("GenericReturnMethod"), target) as string;

            Assert.AreEqual("result", actual);
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
            mock.Setup(o => o.StringProperty).Returns("result");

            var actual = Invoke(InvokerFor<OptimizedClass>("get:StringProperty"), target);

            Assert.AreEqual("result", actual);
        }

        [Test]
        public void Test_property_set()
        {
            Invoke(InvokerFor<OptimizedClass>("set:StringProperty"), target, "result");

            mock.VerifySet(p => p.StringProperty = "result", Times.Once());
        }

        [Test]
        public void Struct_property_setters_are_not_optimized_because_they_are_value_type_and_we_are_not_allowed_to_unbox_a_value_type_and_set_its_property()
        {
            var invoker = typeof(Struct).GetProperty("Property")?.GetSetMethod().CreateInvoker() as ProxyMethodInvoker;

            Assert.IsNotNull(invoker);
            Assert.IsInstanceOf<ReflectionMethodInvoker>(invoker.Real);
        }

        [Test]
        public void Test_property_get_by_index()
        {
            mock.Setup(o => o["key", 0]).Returns("result");

            var actual = Invoke(InvokerFor<OptimizedClass>("get:Item"), target, "key", 0);

            Assert.AreEqual("result", actual);
        }

        [Test]
        public void Test_property_set_by_index()
        {
            Invoke(InvokerFor<OptimizedClass>("set:Item"), target, "key", 0, "result");

            mock.VerifySet(o => o["key", 0] = "result", Times.Once());
        }

        [Test]
        public void Test_create_object()
        {
            var actual = Invoke(InvokerFor<OptimizedClass>("new"), null, mock.Object);
            Assert.IsInstanceOf<OptimizedClass>(actual);
        }

        [Test]
        public void For_non_public_methods_ReflectionInvoker_is_created()
        {
            var proxy = InvokerFor<OptimizedClass>("NonPublicVoidMethod") as ProxyMethodInvoker;

            Assert.IsNotNull(proxy);
            Assert.IsInstanceOf<ReflectionMethodInvoker>(proxy.Real);

            proxy = InvokerFor<OptimizedClass>("VoidMethod") as ProxyMethodInvoker;

            Assert.IsNotNull(proxy);
            Assert.IsNotInstanceOf<ReflectionMethodInvoker>(proxy.Real);
        }

        [Test]
        public void For_methods_with_ref_parameters_ReflectionInvoker_is_created()
        {
            var proxy = InvokerFor<Uri>("HexUnescape") as ProxyMethodInvoker;

            Assert.IsNotNull(proxy);
            Assert.IsInstanceOf<ReflectionMethodInvoker>(proxy.Real);
        }

        [Test]
        public void For_the_clone_method_of_records_ReflectionInvoker_is_created()
        {
            var proxy = InvokerFor<Record>("<Clone>$") as ProxyMethodInvoker;

            Assert.IsNotNull(proxy);
            Assert.IsInstanceOf<ReflectionMethodInvoker>(proxy.Real);
        }

        [Test]
        public void For_record_property_setters_ReflectionInvoker_is_created()
        {
            var proxy = InvokerFor<Record>("set:Data") as ProxyMethodInvoker;

            Assert.IsNotNull(proxy);
            Assert.IsInstanceOf<ReflectionMethodInvoker>(proxy.Real);
        }

        [Test]
        public void ReflectionOptimizer_uses_reflection_when_given_method_is_not_public()
        {
            Invoke(InvokerFor<OptimizedClass>("PrivateVoidMethod"), target);

            mock.Verify(o => o.PrivateVoidMethod(), Times.Once());

            Invoke(InvokerFor<OptimizedClass.InnerClass>("new"), target);
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
                InvokerFor<OptimizedClass>("Overload", 1),
            };

            Invoke(invokers[0], target, new List<string>());
            Invoke(invokers[1], target);
            Invoke(invokers[2], target);
            Invoke(invokers[3], target, "test");
            Invoke(invokers[4], target);
            Invoke(invokers[5], target, 1);

            mock.Verify(o => o.ListParameterVoidMethod(It.IsAny<List<string>>()), Times.Once());
            mock.Verify(o => o.PrivateVoidMethod(), Times.Once());
            mock.VerifyGet(o => o.StringProperty, Times.Once());
            mock.VerifySet(o => o.StringProperty = It.IsAny<string>(), Times.Once());
            mock.Verify(o => o.Overload(), Times.Once());
            mock.Verify(o => o.Overload(It.IsAny<int>()), Times.Once());
        }

        [TestCase(nameof(OptimizedClass.VoidMethod))]
        [TestCase(nameof(OptimizedClass.AsyncVoidMethod))]
        public void Throws_exception_without_any_change(string method)
        {
            Assert.Fail("not implemented");
        }
    }
}