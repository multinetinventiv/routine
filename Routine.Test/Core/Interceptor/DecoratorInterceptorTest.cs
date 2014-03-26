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
				.After(actual => Assert.AreEqual("test string", actual))
				.Error(actual => Assert.AreEqual("test string", actual));

			var context = String();

			testing.OnBefore(context);
			testing.OnAfter(context);
			testing.OnError(context);
		}

		[Test]
		public void VariablesDoesNotConflictWhenMultipleInstancesInterceptSameContext()
		{
			testing = BuildRoutine.Interceptor<TestContext<string>>()
				.ByDecorating(() => "interceptor1")
				.After(actual => Assert.AreEqual("interceptor1", actual))
				.Error(actual => Assert.AreEqual("interceptor1", actual));

			testingOther = BuildRoutine.Interceptor<TestContext<string>>()
				.ByDecorating(() => "interceptor2")
				.After(actual => Assert.AreEqual("interceptor2", actual))
				.Error(actual => Assert.AreEqual("interceptor2", actual));

			var context = String();

			testing.OnBefore(context);
			testingOther.OnBefore(context);

			testingOther.OnAfter(context);
			testing.OnAfter(context);

			testingOther.OnError(context);
			testing.OnError(context);
		}

		[Test]
		public void ByDefaultNothingHappensOnAfterAndOnError()
		{
			testing = BuildRoutine.Interceptor<TestContext<string>>()
				.ByDecorating(() => "dummy");

			var context = String();

			testing.OnBefore(context);
			testing.OnAfter(context);
			testing.OnError(context);
		}

		[Test]
		public void ContextCanBeUsedDuringInterception()
		{
			testing = BuildRoutine.Interceptor<TestContext<string>>()
				.ByDecorating(ctx => ctx["value"] as string)
				.After((ctx, actual) => Assert.AreSame(ctx["value"], actual))
				.Error((ctx, actual) => Assert.AreSame(ctx["value"], actual));

			var context = String();
			context["value"] = "dummy";

			testing.OnBefore(context);
			testing.OnAfter(context);
			testing.OnError(context);
		}
	}
}
