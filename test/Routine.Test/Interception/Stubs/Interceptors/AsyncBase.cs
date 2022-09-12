using Routine.Interception;
using System.Threading.Tasks;
using System;

namespace Routine.Test.Interception.Stubs.Interceptors;

public class AsyncBase : AsyncInterceptorBase<Context>
{
    protected override async Task<object> InterceptAsync(Context context, Func<Task<object>> invocation) => await invocation();
}
