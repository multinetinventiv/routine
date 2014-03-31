using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Routine.Core;
using Routine.Core.Interceptor;
using Routine.Test.Core.Interceptor.Domain;

namespace Routine.Test.Core.Interceptor
{
	[TestFixture]
	public class DecoratorInterceptorTest : InterceptorTestBase
	{
		public override string[] DomainTypeRootNamespaces { get { return new[] { "Routine.Test.Core.Interceptor.Domain" }; } }

		private IInterceptor<TestContext<string>> testing;
		private IInterceptor<TestContext<string>> testingOther;

		[Test]
		public void DecoratesAnInvocationWithAVariableOfGivenTypeWhichIsCreatedOnBefore()
		{
			testing = BuildRoutine.Interceptor<TestContext<string>>()
				.ByDecorating(() => "test string")
				.Success(actual => Assert.AreEqual("test string", actual))
				.Fail(actual => Assert.AreEqual("test string", actual))
				.After(actual => Assert.AreEqual("test string", actual));

			var context = String();

			testing.OnBefore(context);
			testing.OnSuccess(context);
			testing.OnFail(context);
			testing.OnAfter(context);
		}

		[Test]
		public void VariablesDoesNotConflictWhenMultipleInstancesInterceptSameContext()
		{
			testing = BuildRoutine.Interceptor<TestContext<string>>()
				.ByDecorating(() => "interceptor1")
				.Success(actual => Assert.AreEqual("interceptor1", actual))
				.Fail(actual => Assert.AreEqual("interceptor1", actual))
				.After(actual => Assert.AreEqual("interceptor1", actual));

			testingOther = BuildRoutine.Interceptor<TestContext<string>>()
				.ByDecorating(() => "interceptor2")
				.Success(actual => Assert.AreEqual("interceptor2", actual))
				.Fail(actual => Assert.AreEqual("interceptor2", actual))
				.After(actual => Assert.AreEqual("interceptor2", actual));

			var context = String();

			testing.OnBefore(context);
			testingOther.OnBefore(context);

			testingOther.OnSuccess(context);
			testing.OnSuccess(context);

			testingOther.OnFail(context);
			testing.OnFail(context);
		}

		[Test]
		public void ByDefaultNothingHappensOnSuccessAndOnFail()
		{
			testing = BuildRoutine.Interceptor<TestContext<string>>()
				.ByDecorating(() => "dummy");

			var context = String();

			testing.OnBefore(context);
			testing.OnSuccess(context);
			testing.OnFail(context);
			testing.OnAfter(context);
		}

		[Test]
		public void ContextCanBeUsedDuringInterception()
		{
			testing = BuildRoutine.Interceptor<TestContext<string>>()
				.ByDecorating(ctx => ctx["value"] as string)
				.Success((ctx, actual) => Assert.AreSame(ctx["value"], actual))
				.Fail((ctx, actual) => Assert.AreSame(ctx["value"], actual))
				.After((ctx, actual) => Assert.AreSame(ctx["value"], actual));

			var context = String();
			context["value"] = "dummy";

			testing.OnBefore(context);
			testing.OnSuccess(context);
			testing.OnFail(context);
			testing.OnAfter(context);
		}

		private T Throws<T>(Exception ex)
		{
			throw ex;
		}

		[Test]
		public void FailAndAfterAreNotCalledWhenVariableCouldNotBeRetrievedDuringBeforeDelegate()
		{
			testing = BuildRoutine.Interceptor<TestContext<string>>()
				.ByDecorating(() => Throws<string>(new Exception()))
				.Success(actual => Assert.Fail("should not be called"))
				.Fail(actual => Assert.Fail("should not be called"))
				.After(actual => Assert.Fail("should not be called"));

			var context = String();
			context["value"] = "dummy";

			try { testing.OnBefore(context); } catch (Exception){}

			testing.OnSuccess(context);
			testing.OnFail(context);
			testing.OnAfter(context);
		}
	}
}
