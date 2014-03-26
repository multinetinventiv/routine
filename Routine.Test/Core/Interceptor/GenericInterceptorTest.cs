using NUnit.Framework;
using Routine.Core;
using Routine.Core.Interceptor;
using Routine.Test.Core.Interceptor.Domain;

namespace Routine.Test.Core.Interceptor
{
	[TestFixture]
	public class GenericInterceptorTest : InterceptorTestBase
	{
		public override string[] DomainTypeRootNamespaces { get { return new[] { "Routine.Test.Core.Interceptor.Domain" }; } }

		private IInterceptor<TestContext<string>> testing;

		[Test]
		public void BeforeAfterErrorActionsCanBeDefinedByDelegates()
		{
			var context = String();

			testing = BuildRoutine.Interceptor<TestContext<string>>().Do()
				.Before(() => context.Value = "before")
				.After(() => context.Value = "after")
				.Error(() => context.Value = "error");

			testing.OnBefore(context);
			Assert.AreEqual("before", context.Value);

			testing.OnAfter(context);
			Assert.AreEqual("after", context.Value);

			testing.OnError(context);
			Assert.AreEqual("error", context.Value);
		}

		[Test]
		public void NothingHappensWhenNothingIsDefined()
		{
			testing = BuildRoutine.Interceptor<TestContext<string>>().Do();

			var context = String();

			testing.OnBefore(context);
			testing.OnAfter(context);
			testing.OnError(context);
		}

		[Test]
		public void ContextCanBeUsedDuringInterception()
		{
			testing = BuildRoutine.Interceptor<TestContext<string>>().Do()
				.Before(ctx => ctx.Value = "before")
				.After(ctx => ctx.Value = "after")
				.Error(ctx => ctx.Value = "error");

			var context = String();

			testing.OnBefore(context);
			Assert.AreEqual("before", context.Value);

			testing.OnAfter(context);
			Assert.AreEqual("after", context.Value);

			testing.OnError(context);
			Assert.AreEqual("error", context.Value);
		}

		[Test]
		public void Facade_Before()
		{
			testing = BuildRoutine.Interceptor<TestContext<string>>().Before(ctx => ctx.Value = "before");

			var context = String();

			testing.OnBefore(context);

			Assert.AreEqual("before", context.Value);
		}

		[Test]
		public void Facade_After()
		{
			testing = BuildRoutine.Interceptor<TestContext<string>>().After(ctx => ctx.Value = "after");

			var context = String();

			testing.OnAfter(context);

			Assert.AreEqual("after", context.Value);
		}

		[Test]
		public void Facade_Error()
		{
			testing = BuildRoutine.Interceptor<TestContext<string>>().Error(ctx => ctx.Value = "error");

			var context = String();

			testing.OnError(context);

			Assert.AreEqual("error", context.Value);
		}
	}
}
