using System;
using NUnit.Framework;
using Routine.Interception;
using Routine.Test.Core;

namespace Routine.Test.Interception
{
	public class TestContext<T> : InterceptionContext
	{
		public TestContext(string target) : base(target) { }

		public T Value { get; set; }
	}

	public abstract class InterceptorTestBase : CoreTestBase
	{
		protected class TestConfiguration { }

		#region TestAroundInterceptor

		protected class TestAroundInterceptor : BaseAroundInterceptor<TestAroundInterceptor, TestContext<string>>
		{
			private Exception exceptionBefore;
			public void FailOnBeforeWith(Exception exceptionBefore) { this.exceptionBefore = exceptionBefore; }

			private object result;
			public void CancelAndReturn(object result) { this.result = result; }

			private object resultOverride;
			public void OverrideActualResultWith(object resultOverride) { this.resultOverride = resultOverride; }

			private Exception exceptionSuccess;
			public void FailOnSuccessWith(Exception exceptionSuccess) { this.exceptionSuccess = exceptionSuccess; }

			private object resultOnFail;
			public void HideFailAndReturn(object resultOnFail) { this.resultOnFail = resultOnFail; }

			private Exception exception;
			public void OverrideExceptionWith(Exception exception) { this.exception = exception; }

			protected override void OnBefore(TestContext<string> context)
			{
				if (exceptionBefore != null)
				{
					throw exceptionBefore;
				}

				context["before"] = true;

				if (result != null)
				{
					context.Canceled = true;
					context.Result = result;
				}
			}

			protected override void OnSuccess(TestContext<string> context)
			{
				if (exceptionSuccess != null)
				{
					throw exceptionSuccess;
				}

				context["success"] = true;

				if (resultOverride != null)
				{
					context.Result = resultOverride;
				}
			}

			protected override void OnFail(TestContext<string> context)
			{
				context["fail"] = true;

				if (resultOnFail != null)
				{
					context.ExceptionHandled = true;
					context.Result = resultOnFail;
				}

				if (exception != null)
				{
					context.Exception = exception;
				}
			}

			protected override void OnAfter(TestContext<string> context) { context["after"] = true; }

			protected override bool CanIntercept(TestContext<string> context)
			{
				return (key != null && context[key] != null) || base.CanIntercept(context);
			}

			private string key;
			public TestAroundInterceptor WhenContextHas(string key)
			{
				this.key = key;

				return this;
			}
		}

		#endregion

		protected TestContext<string> String() { return String(null); }
		protected TestContext<string> String(string value) { return Ctx(value); }
		protected TestContext<T> Ctx<T>() { return Ctx(default(T)); }
		protected TestContext<T> Ctx<T>(T value) { return new TestContext<T>("test") { Value = value }; }
		protected TestConfiguration DummyConfiguration() { return new TestConfiguration(); }

		protected TestContext<string> context;
		protected Func<object> invocation;

		private int invocationCount;

		private object result;
		protected void InvocationReturns(object result) { this.result = result; }

		private Exception exception;
		protected void InvocationFailsWith(Exception exception) { this.exception = exception; }
		protected string ExceptionStackTraceLookupText => "InterceptorTestBase.<SetUp>";

        protected T Throw<T>(Exception ex)
		{
			throw ex;
		}

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			context = String();
			invocationCount = 0;
			result = null;
			exception = null;
			invocation = () =>
			{
				invocationCount++;

				if (exception != null) { throw exception; }

				context["invocation"] = true;

				return result;
			};
		}

		protected void AssertInvocationWasCalledOnlyOnce()
		{
			Assert.AreEqual(1, invocationCount);
		}
	}
}