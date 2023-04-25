using Routine.Interception;
using Routine.Test.Interception.Stubs.Interceptors;
using Routine.Test.Interception.Stubs;

namespace Routine.Test.Interception;

public class InterceptorBaseTest
{
    [Test]
    public void Delegates_Intercept_as_is()
    {
        var testing = new Base() as IInterceptor<Context>;
        var expected = new object();

        var actual = testing.Intercept(new Context(), () => expected);

        Assert.That(actual, Is.SameAs(expected));
    }

    [Test]
    public async Task Delegates_InterceptAsync_as_is()
    {
        var testing = new Base() as IInterceptor<Context>;
        var expected = new object();

        var actual = await testing.InterceptAsync(new Context(), () => Task.FromResult(expected));

        Assert.That(actual, Is.SameAs(expected));
    }
}
