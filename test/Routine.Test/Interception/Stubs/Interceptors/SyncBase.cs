using Routine.Interception;
using System;

namespace Routine.Test.Interception.Stubs.Interceptors
{
    class SyncBase : InterceptorBase<Context>
    {
        protected override object Intercept(Context context, Func<object> invocation) => invocation();
    }
}