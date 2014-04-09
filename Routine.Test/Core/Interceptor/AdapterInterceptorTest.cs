using NUnit.Framework;
using Routine.Core;
using Routine.Test.Core.Interceptor.Domain;

namespace Routine.Test.Core.Interceptor
{
	[TestFixture]
	public class AdapterInterceptorTest : InterceptorTestBase
	{
		[Test]
		public void Adapts_an_interceptor_with_a_generic_context_to_an_interceptor_with_a_specific_context()
		{
			var interceptorWithGenericContext = BuildRoutine.Interceptor<InterceptionContext>()
				.Before(ctx => { ctx.Canceled = true; ctx.Result = "from generic context"; });

			var testing = BuildRoutine.Interceptor<TestContext<string>>()
				.Adapt(interceptorWithGenericContext);

			var actual = testing.Intercept(context, invocation);

			Assert.AreEqual("from generic context", actual);
		}
	}
}
