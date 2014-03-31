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
		public void BeforeSuccessFailActionsCanBeDefinedByDelegates()
		{
			var context = String();

			testing = BuildRoutine.Interceptor<TestContext<string>>().Do()
				.Before(() => context.Value = "before")
				.Success(() => context.Value = "success")
				.Fail(() => context.Value = "fail")
				.After(() => context.Value = "after");

			testing.OnBefore(context);
			Assert.AreEqual("before", context.Value);

			testing.OnSuccess(context);
			Assert.AreEqual("success", context.Value);

			testing.OnFail(context);
			Assert.AreEqual("fail", context.Value);

			testing.OnAfter(context);
			Assert.AreEqual("after", context.Value);
		}

		[Test]
		public void NothingHappensWhenNothingIsDefined()
		{
			testing = BuildRoutine.Interceptor<TestContext<string>>().Do();

			var context = String();

			testing.OnBefore(context);
			testing.OnSuccess(context);
			testing.OnFail(context);
			testing.OnAfter(context);
		}

		[Test]
		public void ContextCanBeUsedDuringInterception()
		{
			testing = BuildRoutine.Interceptor<TestContext<string>>().Do()
				.Before(ctx => ctx.Value = "before")
				.Success(ctx => ctx.Value = "success")
				.Fail(ctx => ctx.Value = "fail")
				.After(ctx => ctx.Value = "after");

			var context = String();

			testing.OnBefore(context);
			Assert.AreEqual("before", context.Value);

			testing.OnSuccess(context);
			Assert.AreEqual("success", context.Value);

			testing.OnFail(context);
			Assert.AreEqual("fail", context.Value);

			testing.OnAfter(context);
			Assert.AreEqual("after", context.Value);
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
		public void Facade_Success()
		{
			testing = BuildRoutine.Interceptor<TestContext<string>>().Success(ctx => ctx.Value = "success");

			var context = String();

			testing.OnSuccess(context);

			Assert.AreEqual("success", context.Value);
		}

		[Test]
		public void Facade_Fail()
		{
			testing = BuildRoutine.Interceptor<TestContext<string>>().Fail(ctx => ctx.Value = "fail");

			var context = String();

			testing.OnFail(context);

			Assert.AreEqual("fail", context.Value);
		}

		[Test]
		public void Facade_After()
		{
			testing = BuildRoutine.Interceptor<TestContext<string>>().After(ctx => ctx.Value = "after");

			var context = String();

			testing.OnAfter(context);

			Assert.AreEqual("after", context.Value);
		}
	}
}
