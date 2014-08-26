using NUnit.Framework;
using Moq;
using Routine.Core.Reflection.Optimization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Routine.Test.Common;

namespace Routine.Test.Core.Reflection.Optimization
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

		string this[string key, int index] { get; set; }

		void GenericParameterMethod(T param);
		T GenericReturnMethod();

		void PrivateVoidMethod();
	}

	public class OptimizedClass : IOptimizedInterface<string>
	{
		public class InnerClass
		{
			public static InnerClass New() { return new InnerClass(); }
			private InnerClass() { }
			public void VoidMethod() { }
		}

		public static void StaticVoidMethod() { }

		private readonly IOptimizedInterface<string> real;
		public OptimizedClass(IOptimizedInterface<string> real) { this.real = real; }

		public void VoidMethod() { real.VoidMethod(); }

		void IOptimizedInterface<string>.ExplicitVoidMethod() { real.ExplicitVoidMethod(); }

		public string StringMethod() { return real.StringMethod(); }
		public void OneParameterVoidMethod(string str) { real.OneParameterVoidMethod(str); }
		public void TwoParameterVoidMethod(string str, int i) { real.TwoParameterVoidMethod(str, i); }
		public string ThreeParameterStringMethod(string str, int i, decimal d) { return real.ThreeParameterStringMethod(str, i, d); }

		public List<string> ListMethod() { return real.ListMethod(); }
		public void ListParameterVoidMethod(List<string> list) { real.ListParameterVoidMethod(list); }
		public void ListListParameterVoidMethod(List<List<string>> listList) { real.ListListParameterVoidMethod(listList); }
		public void DictionaryParameterVoidMethod(Dictionary<string, object> dict) { real.DictionaryParameterVoidMethod(dict); }

		private void NonPublicVoidMethod() { }

		public string StringProperty { get { return real.StringProperty; } set { real.StringProperty = value; } }
		public string this[string key, int index] { get { return real[key, index]; } set { real[key, index] = value; } }

		public void GenericParameterMethod(string param) { real.GenericParameterMethod(param); }
		public string GenericReturnMethod() { return real.GenericReturnMethod(); }

		private void PrivateVoidMethod() { real.PrivateVoidMethod(); }
		void IOptimizedInterface<string>.PrivateVoidMethod() { PrivateVoidMethod(); }
	}

	#endregion

	[TestFixture]
	public class ReflectionOptimizerTest : CoreTestBase
	{
		#region Setup & Helpers

		public override string[] DomainTypeRootNamespaces { get { return new[] { "Routine.Test.Core.Reflection.Optimization" }; } }

		private Mock<IOptimizedInterface<string>> mock;
		private OptimizedClass target;
		private ReflectionOptimizer testing;

		public override void SetUp()
		{
			base.SetUp();

			mock = new Mock<IOptimizedInterface<string>>();

			target = new OptimizedClass(mock.Object);

			testing = new ReflectionOptimizer();
		}

		private IMethodInvoker InvokerFor<T>(string methodName)
		{
			if (methodName.StartsWith("get:"))
			{
				return testing
							.CreateInvoker(typeof(T).GetProperty(methodName.After("get:"), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
							.GetGetMethod());
			}

			if (methodName.StartsWith("set:"))
			{
				return testing
							.CreateInvoker(typeof(T).GetProperty(methodName.After("set:"), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
							.GetSetMethod());
			}

			if (methodName == "new")
			{
				return testing.CreateInvoker(typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).FirstOrDefault());
			}

			return testing.CreateInvoker(typeof(T).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static));
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
				Assert.AreEqual("method", ex.ParamName);
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
			Assert.IsInstanceOf<ReflectionMethodInvoker>(InvokerFor<OptimizedClass>("NonPublicVoidMethod"));
			Assert.IsNotInstanceOf<ReflectionMethodInvoker>(InvokerFor<OptimizedClass>("VoidMethod"));
		}

		[Test]
		public void ReflectionOptimizer_uses_reflection_when_given_method_is_not_public()
		{
			InvokerFor<OptimizedClass>("PrivateVoidMethod").Invoke(target);

			mock.Verify(o => o.PrivateVoidMethod(), Times.Once());

			InvokerFor<OptimizedClass.InnerClass>("new").Invoke(target);
		}

		[Test][Ignore]
		public void Generic_method_invoker_interface_support()
		{
			Assert.Fail("not implemented");
		}

		[Test][Ignore]
		public void Test_the_case_where_somehow_other_method_s_parameter_type_causes_an_extra_dll_reference()
		{
			Assert.Fail();
		}
	}
}

