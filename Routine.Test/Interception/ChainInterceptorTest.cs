using System;
using NUnit.Framework;
using Routine.Interception;

namespace Routine.Test.Interception
{
	[TestFixture]
	public class ChainInterceptorTest : InterceptorTestBase
	{
		private IInterceptor<TestContext<string>> testing;

		[Test]
		public void First_added_interceptor_wraps_the_second_one_and_invocation_happens_last()
		{
			var interceptor = new ChainInterceptor<TestContext<string>>();

			interceptor.Add(BuildRoutine.Interceptor<TestContext<string>>().Do()
				.Before(ctx => ctx.Value += " - before1")
				.Success(ctx => ctx.Value += " - success1")
				.After(ctx => ctx.Value += " - after1"));
			interceptor.Add(BuildRoutine.Interceptor<TestContext<string>>().Do()
				.Before(ctx => ctx.Value += " - before2")
				.Success(ctx => ctx.Value += " - success2")
				.After(ctx => ctx.Value += " - after2"));

			testing = interceptor;

			context.Value = "begin";

			InvocationReturns("actual");
			
			var actual = testing.Intercept(context, invocation);

			Assert.AreEqual("actual", actual);

			Assert.AreEqual("begin - before1 - before2 - success2 - after2 - success1 - after1", context.Value);
			AssertInvocationWasCalledOnlyOnce();
		}

		[Test]
		public void When_invocation_fails_all_interceptors_work_in_the_reverse_order__since_first_wraps_second()
		{
			var interceptor = new ChainInterceptor<TestContext<string>>();

			interceptor.Add(BuildRoutine.Interceptor<TestContext<string>>().Do()
				.Before(ctx => ctx.Value += " - before1")
				.Fail(ctx => ctx.Value += " - fail1")
				.After(ctx => ctx.Value += " - after1"));
			interceptor.Add(BuildRoutine.Interceptor<TestContext<string>>().Do()
				.Before(ctx => ctx.Value += " - before2")
				.Fail(ctx => ctx.Value += " - fail2")
				.After(ctx => ctx.Value += " - after2"));

			testing = interceptor;

			context.Value = "begin";

			InvocationFailsWith(new Exception());

			Assert.Throws<Exception>(() => testing.Intercept(context, invocation));

			Assert.AreEqual("begin - before1 - before2 - fail2 - after2 - fail1 - after1", context.Value);
			AssertInvocationWasCalledOnlyOnce();
		}

		[Test]
		public void When_invocation_fails_and_second_interceptor_handles_the_exception__first_interceptor_does_not_know_about_the_exception()
		{
			var interceptor = new ChainInterceptor<TestContext<string>>();

			interceptor.Add(BuildRoutine.Interceptor<TestContext<string>>().Do()
				.Before(ctx => ctx.Value += " - before1")
				.Success(ctx => ctx.Value += " - success1")
				.After(ctx => ctx.Value += " - after1"));
			interceptor.Add(BuildRoutine.Interceptor<TestContext<string>>().Do()
				.Before(ctx => ctx.Value += " - before2")
				.Fail(ctx =>
				{
					ctx.Value += " - fail2 (handled)";
					ctx.ExceptionHandled = true;
					ctx.Result = "result2";
				})
				.After(ctx => ctx.Value += " - after2"));

			testing = interceptor;
			
			context.Value = "begin";

			InvocationFailsWith(new Exception());

			var actual = testing.Intercept(context, invocation);

			Assert.AreEqual("result2", actual);

			Assert.AreEqual("begin - before1 - before2 - fail2 (handled) - after2 - success1 - after1", context.Value);
			AssertInvocationWasCalledOnlyOnce();
		}

		[Test]
		public void When_first_interceptor_fails_on_success__second_interceptor_does_not_know_about_the_exception()
		{
			var interceptor = new ChainInterceptor<TestContext<string>>();

			interceptor.Add(BuildRoutine.Interceptor<TestContext<string>>().Do()
				.Before(ctx => ctx.Value += " - before1")
				.Success(ctx =>
				{
					ctx.Value += " - success1";
					throw new Exception();
				})
				.Fail(ctx => ctx.Value += " - fail1")
				.After(ctx => ctx.Value += " - after1"));

			interceptor.Add(BuildRoutine.Interceptor<TestContext<string>>().Do()
				.Before(ctx => ctx.Value += " - before2")
				.Success(ctx => ctx.Value += " - success2")
				.After(ctx => ctx.Value += " - after2"));

			testing = interceptor;

			context.Value = "begin";

			InvocationReturns("actual");

			Assert.Throws<Exception>(() => testing.Intercept(context, invocation));

			Assert.AreEqual("begin - before1 - before2 - success2 - after2 - success1 - fail1 - after1", context.Value);
			AssertInvocationWasCalledOnlyOnce();
		}

		[Test]
		public void Merging_two_chains_connects_given_chain_to_the_last_link_of_the_root_chain()
		{
			var interceptor1 = new ChainInterceptor<TestContext<string>>();
			interceptor1.Add(BuildRoutine.Interceptor<TestContext<string>>().Before(ctx => ctx.Value += " - before1"));
			interceptor1.Add(BuildRoutine.Interceptor<TestContext<string>>().Before(ctx => ctx.Value += " - before2"));
			
			var interceptor2 = new ChainInterceptor<TestContext<string>>();
			interceptor2.Add(BuildRoutine.Interceptor<TestContext<string>>().Before(ctx => ctx.Value += " - before3"));
			interceptor2.Add(BuildRoutine.Interceptor<TestContext<string>>().Before(ctx => ctx.Value += " - before4"));

			interceptor1.Merge(interceptor2);

			testing = interceptor1;

			context.Value = "begin";
			testing.Intercept(context, invocation);

			Assert.AreEqual("begin - before1 - before2 - before3 - before4", context.Value);
			AssertInvocationWasCalledOnlyOnce();
		}

		[Test]
		public void After_merging_two_chains__new_interceptor_is_added_to_the_very_last_chain_link()
		{
			var interceptor1 = new ChainInterceptor<TestContext<string>>();
			interceptor1.Add(BuildRoutine.Interceptor<TestContext<string>>().Before(ctx => ctx.Value += " - before1"));
			interceptor1.Add(BuildRoutine.Interceptor<TestContext<string>>().Before(ctx => ctx.Value += " - before2"));

			var interceptor2 = new ChainInterceptor<TestContext<string>>();
			interceptor2.Add(BuildRoutine.Interceptor<TestContext<string>>().Before(ctx => ctx.Value += " - before3"));
			interceptor2.Add(BuildRoutine.Interceptor<TestContext<string>>().Before(ctx => ctx.Value += " - before4"));

			interceptor1.Merge(interceptor2);

			interceptor1.Add(BuildRoutine.Interceptor<TestContext<string>>().Before(ctx => ctx.Value += " - before5"));

			testing = interceptor1;

			context.Value = "begin";
			testing.Intercept(context, invocation);

			Assert.AreEqual("begin - before1 - before2 - before3 - before4 - before5", context.Value);
			AssertInvocationWasCalledOnlyOnce();
		}

		[Test]
		public void When_a_chain_is_merged_with_an_empty_chain_nothing_changes()
		{
			var interceptor1 = new ChainInterceptor<TestContext<string>>();
			interceptor1.Add(BuildRoutine.Interceptor<TestContext<string>>().Before(ctx => ctx.Value += " - before1"));
			interceptor1.Add(BuildRoutine.Interceptor<TestContext<string>>().Before(ctx => ctx.Value += " - before2"));

			var interceptor2 = new ChainInterceptor<TestContext<string>>();

			interceptor1.Merge(interceptor2);

			testing = interceptor1;

			context.Value = "begin";
			testing.Intercept(context, invocation);

			Assert.AreEqual("begin - before1 - before2", context.Value);
			AssertInvocationWasCalledOnlyOnce();
		}

		[Test]
		public void When_an_empty_chain_is_merged_with_a_chain__given_chain_s_links_added_to_the_empty_one()
		{
			var interceptor1 = new ChainInterceptor<TestContext<string>>();

			var interceptor2 = new ChainInterceptor<TestContext<string>>();
			interceptor2.Add(BuildRoutine.Interceptor<TestContext<string>>().Before(ctx => ctx.Value += " - before1"));
			interceptor2.Add(BuildRoutine.Interceptor<TestContext<string>>().Before(ctx => ctx.Value += " - before2"));

			interceptor1.Merge(interceptor2);

			testing = interceptor1;

			context.Value = "begin";
			testing.Intercept(context, invocation);

			Assert.AreEqual("begin - before1 - before2", context.Value);
			AssertInvocationWasCalledOnlyOnce();
		}

		[Test]
		public void When_an_empty_chain_intercepts_some_invocation__invocation_is_directly_called()
		{
			testing = new ChainInterceptor<TestContext<string>>();

			InvocationReturns("actual");

			var actual = testing.Intercept(context, invocation);

			Assert.AreEqual("actual", actual);
			AssertInvocationWasCalledOnlyOnce();
		}
	}
}
