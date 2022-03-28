using NUnit.Framework;
using Routine.Interception;
using Routine.Test.Interception.Stubs.Interceptors;
using Routine.Test.Interception.Stubs;
using System.Threading.Tasks;

namespace Routine.Test.Interception
{
    [TestFixture(typeof(SyncBase))]
    [TestFixture(typeof(AsyncBase))]
    public class InterceptorBaseTest<TInterceptor>
        where TInterceptor : IInterceptor<Context>, new()
    {
        [Test]
        public void Delegates_Intercept_as_is()
        {
            var testing = new TInterceptor();
            var expected = new object();

            var actual = testing.Intercept(new Context(), () => expected);

            Assert.AreSame(expected, actual);
        }

        [Test]
        public async Task Delegates_InterceptAsync_as_is()
        {
            var testing = new TInterceptor();
            var expected = new object();

            var actual = await testing.InterceptAsync(new Context(), () => Task.FromResult(expected));

            Assert.AreSame(expected, actual);
        }
    }
}
