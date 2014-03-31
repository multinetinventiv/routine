using System;
using Moq;
using NUnit.Framework;
using Routine.Core;
using Routine.Core.Interceptor;
using Routine.Test.Core.Interceptor.Domain;

namespace Routine.Test.Core.Interceptor
{
	[TestFixture]
	public class InterceptionTest : InterceptorTestBase
	{
		public override string[] DomainTypeRootNamespaces { get { return new[] { "Routine.Test.Core.Interceptor.Domain" }; } }

		private Mock<IInterceptor<TestContext<string>>> interceptorMock;
		private TestContext<string> context;
		private Interception<TestContext<string>> testing;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			interceptorMock = new Mock<IInterceptor<TestContext<string>>>();
			context = new TestContext<string>();

			testing = interceptorMock.Object.Intercept(context);
		}

		[Test]
		public void BeforeInvocationOnBeforeIsCalled()
		{
			testing.Do(() => null);

			interceptorMock.Verify(obj => obj.OnBefore(context), Times.Once());
		}

		[Test]
		public void CanCancelActualInvocationAndReturnResultBeforeInvocation()
		{
			bool called = false;
			interceptorMock
				.Setup(obj => obj.OnBefore(context))
				.Callback((TestContext<string> ctx) => { 
					ctx.Canceled = true;
					ctx.Result = "actual";
				});
			var actual = testing.Do(() => {
				called = true;
				return "overridden";
			});

			Assert.AreEqual("actual", actual);
			Assert.IsFalse(called);
			interceptorMock.Verify(obj => obj.OnSuccess(context), Times.Once());
		}

		[Test]
		public void AfterASuccessfulInvocationOnSuccessIsCalled()
		{
			testing.Do(() => null);

			interceptorMock.Verify(obj => obj.OnSuccess(context), Times.Once());
			interceptorMock.Verify(obj => obj.OnFail(It.IsAny<TestContext<string>>()), Times.Never());
		}

		[Test]
		public void CanAlterActualInvocationResultAfterASuccessfulInvocation()
		{
			interceptorMock
				.Setup(obj => obj.OnSuccess(context))
				.Callback((TestContext<string> ctx) => {
					ctx.Result = "actual";
				});

			var actual = testing.Do(() => "overridden");

			Assert.AreEqual("actual", actual);
		}

		private object Throw(Exception exception) { throw exception; }

		[Test]
		public void WhenInvocationThrowsExceptionOnFailIsCalled()
		{
			var exception = new Exception();
			try
			{
				testing.Do(() => Throw(exception));
				Assert.Fail("exception not thrown");
			}
			catch (Exception){}

			interceptorMock.Verify(obj => obj.OnSuccess(It.IsAny<TestContext<string>>()), Times.Never());
			interceptorMock.Verify(obj => obj.OnFail(context), Times.Once());
			Assert.AreEqual(exception, context.Exception);
		}

		[Test]
		public void CanHideTheExceptionAndReturnResult()
		{
			interceptorMock
				.Setup(obj => obj.OnFail(context))
				.Callback((TestContext<string> ctx) => {
					ctx.ExceptionHandled = true;
					ctx.Result = "result";
				});
			
			var actual = testing.Do(() => Throw(new Exception()));

			Assert.AreEqual("result", actual);
		}

		[Test]
		public void CanAlterExceptionWhenInvocationThrowsException()
		{
			var actualException = new Exception();
			var alteredException = new Exception();

			interceptorMock
				.Setup(obj => obj.OnFail(context))
				.Callback((TestContext<string> ctx) =>
				{
					ctx.Exception = alteredException;
				});

			try
			{
				testing.Do(() => Throw(actualException));
				Assert.Fail("exception not thrown");
			}
			catch (Exception actual)
			{
				Assert.AreEqual(alteredException, actual);
			}
		}

		[Test]
		public void OnFailIsCalledWhenAnExceptionIsThrownOnBefore()
		{
			interceptorMock
				.Setup(obj => obj.OnBefore(context))
				.Callback((TestContext<string> ctx) =>
				{
					throw new Exception();
				});

			try
			{
				testing.Do(() => null);
				Assert.Fail("exception not thrown");
			}
			catch (Exception) { }

			interceptorMock.Verify(obj => obj.OnFail(context), Times.Once());
		}

		[Test]
		public void OnFailIsCalledWhenAnExceptionIsThrownOnSuccess()
		{
			interceptorMock
				.Setup(obj => obj.OnSuccess(context))
				.Callback((TestContext<string> ctx) =>
				{
					throw new Exception();
				});

			try
			{
				testing.Do(() => null);
				Assert.Fail("exception not thrown");
			}
			catch (Exception) { }

			interceptorMock.Verify(obj => obj.OnFail(context), Times.Once());
		}

		[Test]
		public void AfterAnyInvocationOnAfterIsCalled()
		{
			var exception = new Exception();
			try
			{
				testing.Do(() => Throw(exception));
				Assert.Fail("exception not thrown");
			}
			catch (Exception) { }

			testing.Do(() => null);

			interceptorMock.Verify(obj => obj.OnAfter(context), Times.Exactly(2));
		}
	}
}
