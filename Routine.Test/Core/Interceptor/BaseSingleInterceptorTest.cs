using NUnit.Framework;
using Routine.Core;
using Routine.Core.Interceptor;
using Routine.Test.Core.Interceptor.Domain;

namespace Routine.Test.Core.Interceptor
{
	[TestFixture]
	public class BaseSingleInterceptorTest : InterceptorTestBase
	{
		public override string[] DomainTypeRootNamespaces { get { return new[] { "Routine.Test.Core.Interceptor.Domain" }; } }

		private class TestInterceptor : BaseSingleInterceptor<TestInterceptor, TestContext<string>>
		{
			protected override void OnBefore(TestContext<string> context) { context["before"] = true; }
			protected override void OnAfter(TestContext<string> context) { context["after"] = true; }
			protected override void OnError(TestContext<string> context) { context["error"] = true; }

			protected override bool CanIntercept(TestContext<string> context)
			{
				return (key != null && context[key] != null) || base.CanIntercept(context);
			}

			private string key;
			public TestInterceptor WhenContextHas(string key)
			{
				this.key = key;

				return this;
			}
		}

		private TestInterceptor testing;
		private IInterceptor<TestContext<string>> testingInterface;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			testingInterface = testing = new TestInterceptor();
		}

		[Test]
		public void InterceptsOnlyWhenClauseReturnsTrue()
		{
			var context = Ctx<string>();

			testing.When(ctx => false);

			testingInterface.OnBefore(context);
			testingInterface.OnAfter(context);
			testingInterface.OnError(context);

			Assert.IsNull(context["before"]);
			Assert.IsNull(context["after"]);
			Assert.IsNull(context["error"]);

			testing.When(ctx => true);

			testingInterface.OnBefore(context);
			testingInterface.OnAfter(context);
			testingInterface.OnError(context);

			Assert.IsTrue((bool)context["before"]);
			Assert.IsTrue((bool)context["after"]);
			Assert.IsTrue((bool)context["error"]);
		}

		[Test]
		public void ByDefaultInterceptsAnyGivenContext()
		{
			var context = Ctx<string>();

			testingInterface.OnBefore(context);
			testingInterface.OnAfter(context);
			testingInterface.OnError(context);

			Assert.IsTrue((bool)context["before"]);
			Assert.IsTrue((bool)context["after"]);
			Assert.IsTrue((bool)context["error"]);
		}

		[Test]
		public void CustomWhenClausesCanBeDefinedBySubClasses()
		{
			var context = Ctx<string>();

			context["override-base"] = true;

			testing.When(ctx => false).WhenContextHas("override-base");

			testingInterface.OnBefore(context);
			testingInterface.OnAfter(context);
			testingInterface.OnError(context);

			Assert.IsTrue((bool)context["before"]);
			Assert.IsTrue((bool)context["after"]);
			Assert.IsTrue((bool)context["error"]);

			testing.When(ctx => true);
		}
	}
}
