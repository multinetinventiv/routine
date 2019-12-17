using System;
using NUnit.Framework;
using Routine.Interception;

namespace Routine.Test.Interception
{
	[TestFixture]
	public class AroundInterceptorTest : InterceptorTestBase
	{
		private IInterceptor<TestContext<string>> testing;

		[Test]
		public void Before_success_fail_actions_can_be_defined_by_delegates()
		{
			var context = String();
			context.Value = "begin";

			testing = BuildRoutine.Interceptor<TestContext<string>>().Do()
				.Before(() => context.Value += " - before")
				.Success(() => context.Value += " - success")
				.Fail(() => context.Value += " - fail")
				.After(() => context.Value += " - after");

			testing.Intercept(context, invocation);

			Assert.AreEqual("begin - before - success - after", context.Value);

			context.Value = "begin";
			InvocationFailsWith(new Exception());

			Assert.Throws<Exception>(() => testing.Intercept(context, invocation));

			Assert.AreEqual("begin - before - fail - after", context.Value);
		}

		[Test]
		public void Nothing_happens_when_nothing_is_defined()
		{
			testing = BuildRoutine.Interceptor<TestContext<string>>().Do();

			var context = String();

			testing.Intercept(context, invocation);
		}

		[Test]
		public void Context_can_be_used_during_interception()
		{
			testing = BuildRoutine.Interceptor<TestContext<string>>().Do()
				.Before(ctx => ctx.Value += " - before")
				.Success(ctx => ctx.Value += " - success")
				.Fail(ctx => ctx.Value += " - fail")
				.After(ctx => ctx.Value += " - after");

			var context = String();
			context.Value = "begin";

			testing.Intercept(context, invocation);

			Assert.AreEqual("begin - before - success - after", context.Value);

			context.Value = "begin";
			InvocationFailsWith(new Exception());

			Assert.Throws<Exception>(() => testing.Intercept(context, invocation));

			Assert.AreEqual("begin - before - fail - after", context.Value);
		}

		[Test]
		public void Facade_Before()
		{
			testing = BuildRoutine.Interceptor<TestContext<string>>().Before(ctx => ctx.Value = "before");

			var context = String();

			testing.Intercept(context, invocation);

			Assert.AreEqual("before", context.Value);
		}

		[Test]
		public void Facade_Success()
		{
			testing = BuildRoutine.Interceptor<TestContext<string>>().Success(ctx => ctx.Value = "success");

			var context = String();

			testing.Intercept(context, invocation);

			Assert.AreEqual("success", context.Value);
		}

		[Test]
		public void Facade_Fail()
		{
			testing = BuildRoutine.Interceptor<TestContext<string>>().Fail(ctx => ctx.Value = "fail");

			var context = String();

			InvocationFailsWith(new Exception());

			Assert.Throws<Exception>(() => testing.Intercept(context, invocation));

			Assert.AreEqual("fail", context.Value);
		}

		[Test]
		public void Facade_After()
		{
			testing = BuildRoutine.Interceptor<TestContext<string>>().After(ctx => ctx.Value = "after");

			var context = String();

			testing.Intercept(context, invocation);

			Assert.AreEqual("after", context.Value);
		}
	}
}
