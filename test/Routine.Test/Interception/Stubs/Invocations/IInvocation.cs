using Routine.Interception;
using System;

namespace Routine.Test.Interception.Stubs.Invocations
{
    public interface IInvocation
    {
        Context Context { get; }
        int Count { get; }
        string ExceptionStackTraceLookupText { get; }

        void Returns(object result);
        void FailsWith(Exception exception);
        object Intercept(IInterceptor<Context> testing);
    }
}
