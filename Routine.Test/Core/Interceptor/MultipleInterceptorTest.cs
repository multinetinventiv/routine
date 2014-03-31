using Moq;
using NUnit.Framework;
using Routine.Core;
using Routine.Core.Interceptor;
using Routine.Test.Core.Interceptor.Domain;

namespace Routine.Test.Core.Interceptor
{
	[TestFixture]
	public class MultipleInterceptorTest : InterceptorTestBase
	{
		public override string[] DomainTypeRootNamespaces { get { return new[] { "Routine.Test.Core.Interceptor.Domain" }; } }

		private Mock<IInterceptor<TestContext<string>>> interceptor1Mock;
		private Mock<IInterceptor<TestContext<string>>> interceptor2Mock;
		private Mock<IInterceptor<TestContext<string>>> interceptor3Mock;

		private MultipleInterceptor<TestConfiguration, TestContext<string>> testing;
		private IInterceptor<TestContext<string>> testingInterface;
		
		private MultipleInterceptor<TestConfiguration, TestContext<string>> testingOther;

		[SetUp]
		public override void SetUp()
		{
			interceptor1Mock = new Mock<IInterceptor<TestContext<string>>>();
			interceptor2Mock = new Mock<IInterceptor<TestContext<string>>>();
			interceptor3Mock = new Mock<IInterceptor<TestContext<string>>>();

			testingInterface = testing = new MultipleInterceptor<TestConfiguration, TestContext<string>>(DummyConfiguration());
			testingOther = new MultipleInterceptor<TestConfiguration, TestContext<string>>(DummyConfiguration());

			testing.Add(interceptor1Mock.Object).Done(interceptor2Mock.Object);
			testingOther.Done(interceptor3Mock.Object);


			SetUpInterceptor(interceptor1Mock, "first");
			SetUpInterceptor(interceptor2Mock, "second");
			SetUpInterceptor(interceptor3Mock, "third");
		}

		private void SetUpInterceptor(Mock<IInterceptor<TestContext<string>>> interceptorMock, string contextValue)
		{
			interceptorMock
				.Setup(o => o.OnBefore(It.IsAny<TestContext<string>>()))
				.Callback((TestContext<string> ctx) => { ctx.Value = contextValue; });
			interceptorMock
				.Setup(o => o.OnSuccess(It.IsAny<TestContext<string>>()))
				.Callback((TestContext<string> ctx) => { ctx.Value = contextValue; });
			interceptorMock
				.Setup(o => o.OnFail(It.IsAny<TestContext<string>>()))
				.Callback((TestContext<string> ctx) => { ctx.Value = contextValue; });
			interceptorMock
				.Setup(o => o.OnAfter(It.IsAny<TestContext<string>>()))
				.Callback((TestContext<string> ctx) => { ctx.Value = contextValue; });
		}

		[Test]
		public void OnBeforeCallsChildInterceptorsInTheGivenOrder()
		{
			var ctx = String();

			testingInterface.OnBefore(ctx);

			Assert.AreEqual("second", ctx.Value);

			interceptor1Mock.Verify(o => o.OnBefore(ctx), Times.Once());
			interceptor2Mock.Verify(o => o.OnBefore(ctx), Times.Once());
		}

		[Test]
		public void OnSuccessCallsChildInterceptorsInTheReverseOrder()
		{
			var ctx = String();

			testingInterface.OnSuccess(ctx);

			Assert.AreEqual("first", ctx.Value);

			interceptor1Mock.Verify(o => o.OnSuccess(ctx), Times.Once());
			interceptor2Mock.Verify(o => o.OnSuccess(ctx), Times.Once());
		}

		[Test]
		public void OnFailCallsChildInterceptorsInTheReverseOrder()
		{
			var ctx = String();

			testingInterface.OnFail(ctx);

			Assert.AreEqual("first", ctx.Value);

			interceptor1Mock.Verify(o => o.OnFail(ctx), Times.Once());
			interceptor2Mock.Verify(o => o.OnFail(ctx), Times.Once());
		}

		[Test]
		public void OnAfterCallsChildInterceptorsInTheReverseOrder()
		{
			var ctx = String();

			testingInterface.OnAfter(ctx);

			Assert.AreEqual("first", ctx.Value);

			interceptor1Mock.Verify(o => o.OnAfter(ctx), Times.Once());
			interceptor2Mock.Verify(o => o.OnAfter(ctx), Times.Once());
		}

		[Test]
		public void MergeAppendsOthersInterceptors()
		{
			testing.Merge(testingOther);

			var ctx = String();

			testingInterface.OnBefore(ctx);

			Assert.AreEqual("third", ctx.Value);
		}
	}
}
