using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moq;
using NUnit.Framework;
using Routine.Core.Reflection;

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
    }

    public struct Struct
    {
        public string Property { get; set; }
    }

    public record Record(string Data);

    #endregion

    [TestFixture]
    public class ReflectionOptimizerTest : CoreTestBase
    {
        #region Setup & Helpers

        private const BindingFlags ALL_MEMBERS = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
        private const BindingFlags ALL_INSTANCE_MEMBERS = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        private Mock<IOptimizedInterface<string>> mock;
        private OptimizedClass target;

        public override void SetUp()
        {
            base.SetUp();

            mock = new Mock<IOptimizedInterface<string>>();

            target = new OptimizedClass(mock.Object);
        }

        private IMethodInvoker InvokerFor<T>(string methodName) { return InvokerFor<T>(methodName, null); }
        private IMethodInvoker InvokerFor<T>(string methodName, int? parameterCount)
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

        [Test]
        public void ReflectionOptimizer_generates_and_compiles_code_to_invoke_given_method_via_invoker_interface()
        {
            InvokerFor<IOptimizedInterface<string>>("VoidMethod").Invoke(target);
            InvokerFor<OptimizedClass>("VoidMethod").Invoke(target);
            InvokerFor<IOptimizedInterface<string>>("ExplicitVoidMethod").Invoke(target);

            mock.Verify(o => o.ExplicitVoidMethod(), Times.Once());
            mock.Verify(o => o.VoidMethod(), Times.Exactly(2));
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
            var actual = InvokerFor<IOptimizedInterface<string>>("VoidMethod").Invoke(target);

            Assert.IsNull(actual);
            mock.Verify(o => o.VoidMethod(), Times.Once());
        }

        [Test]
        public void Test_return_value()
        {
            mock.Setup(o => o.StringMethod()).Returns("result");

            var actual = InvokerFor<OptimizedClass>("StringMethod").Invoke(target);

            Assert.AreEqual("result", actual);
        }

        [Test]
        public void Test_generic_type_return()
        {
            InvokerFor<OptimizedClass>("ListMethod").Invoke(target);
        }

        [Test]
        public void Test_parameter()
        {
            InvokerFor<OptimizedClass>("OneParameterVoidMethod").Invoke(target, "test");
            InvokerFor<OptimizedClass>("TwoParameterVoidMethod").Invoke(target, "test1", 1);
            InvokerFor<OptimizedClass>("ThreeParameterStringMethod").Invoke(target, "test2", 2, 0.2m);

            mock.Verify(o => o.OneParameterVoidMethod("test"), Times.Once());
            mock.Verify(o => o.TwoParameterVoidMethod("test1", 1), Times.Once());
            mock.Verify(o => o.ThreeParameterStringMethod("test2", 2, 0.2m), Times.Once());
        }

        [Test]
        public void When_null_is_given_for_a_value_type_parameter__default_value_of_that_type_is_used()
        {
            InvokerFor<OptimizedClass>("TwoParameterVoidMethod").Invoke(target, "dummy", null);

            mock.Verify(o => o.TwoParameterVoidMethod(It.IsAny<string>(), 0), Times.Once());
        }

        [Test]
        public void Method_invoker_does_not_check_parameter_compatibility_and_let_IndexOutOfRangeException_or_InvalidCastException_to_be_thrown()
        {
            try
            {
                InvokerFor<OptimizedClass>("OneParameterVoidMethod").Invoke(target);
                Assert.Fail("exception not thrown");
            }
            catch (IndexOutOfRangeException) { }

            try
            {
                InvokerFor<OptimizedClass>("OneParameterVoidMethod").Invoke(target, 0);
                Assert.Fail("exception not thrown");
            }
            catch (InvalidCastException) { }
        }

        [Test]
        public void Test_generic_type_parameter()
        {
            InvokerFor<OptimizedClass>("ListParameterVoidMethod").Invoke(target, new List<string>());
            InvokerFor<OptimizedClass>("ListListParameterVoidMethod").Invoke(target, new List<List<string>>());
            InvokerFor<OptimizedClass>("DictionaryParameterVoidMethod").Invoke(target, new Dictionary<string, object>());
        }

        [Test]
        public void Test_generic_method()
        {
            InvokerFor<IOptimizedInterface<string>>("GenericParameterMethod").Invoke(target, "param");

            mock.Verify(o => o.GenericParameterMethod("param"));

            mock.Setup(o => o.GenericReturnMethod()).Returns("result");

            var actual = InvokerFor<IOptimizedInterface<string>>("GenericReturnMethod").Invoke(target) as string;

            Assert.AreEqual("result", actual);
        }

        [Test]
        public void Test_static_method()
        {
            InvokerFor<OptimizedClass>("StaticVoidMethod").Invoke(null);
        }

        [Test]
        public void Test_inner_class()
        {
            InvokerFor<OptimizedClass.InnerClass>("VoidMethod").Invoke(OptimizedClass.InnerClass.New());
        }

        [Test]
        public void Test_property_get()
        {
            mock.Setup(o => o.StringProperty).Returns("result");

            var actual = InvokerFor<OptimizedClass>("get:StringProperty").Invoke(target);

            Assert.AreEqual("result", actual);
        }

        [Test]
        public void Test_property_set()
        {
            InvokerFor<OptimizedClass>("set:StringProperty").Invoke(target, "result");

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

            var actual = InvokerFor<OptimizedClass>("get:Item").Invoke(target, "key", 0);

            Assert.AreEqual("result", actual);
        }

        [Test]
        public void Test_property_set_by_index()
        {
            InvokerFor<OptimizedClass>("set:Item").Invoke(target, "key", 0, "result");

            mock.VerifySet(o => o["key", 0] = "result", Times.Once());
        }

        [Test]
        public void Test_create_object()
        {
            var actual = InvokerFor<OptimizedClass>("new").Invoke(null, mock.Object);
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
            InvokerFor<OptimizedClass>("PrivateVoidMethod").Invoke(target);

            mock.Verify(o => o.PrivateVoidMethod(), Times.Once());

            InvokerFor<OptimizedClass.InnerClass>("new").Invoke(target);
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

            invokers[0].Invoke(target, new List<string>());
            invokers[1].Invoke(target);
            invokers[2].Invoke(target);
            invokers[3].Invoke(target, "test");
            invokers[4].Invoke(target);
            invokers[5].Invoke(target, 1);

            mock.Verify(o => o.ListParameterVoidMethod(It.IsAny<List<string>>()), Times.Once());
            mock.Verify(o => o.PrivateVoidMethod(), Times.Once());
            mock.VerifyGet(o => o.StringProperty, Times.Once());
            mock.VerifySet(o => o.StringProperty = It.IsAny<string>(), Times.Once());
            mock.Verify(o => o.Overload(), Times.Once());
            mock.Verify(o => o.Overload(It.IsAny<int>()), Times.Once());
        }

        [Test]
        [Ignore("")]
        public void Generic_method_invoker_interface_support()
        {
            Assert.Fail("not implemented");
        }

        [Test]
        [Ignore("")]
        public void Test_the_case_where_somehow_other_method_s_parameter_type_causes_an_extra_dll_reference()
        {
            Assert.Fail();
        }
    }
}

