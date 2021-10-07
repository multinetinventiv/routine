using System;
using Routine.Core.Runtime;
using System.Reflection;
using System.Threading.Tasks;

namespace Routine.Core.Reflection
{
    public class ReflectionMethodInvoker : IMethodInvoker
    {
        private readonly MethodBase method;

        public ReflectionMethodInvoker(MethodBase method)
        {
            this.method = method;
        }

        public object Invoke(object target, params object[] args)
        {
            var result = InvokeInner(target, args);

            if (result is not Task task) { return result; }

            try
            {
                Task.WaitAll(task);
            }
            catch (AggregateException ex)
            {
                TryThrowingInnerExceptionOf(ex);

                throw;
            }

            return ResultOf(task);
        }

        public async Task<object> InvokeAsync(object target, params object[] args)
        {
            var result = InvokeInner(target, args);

            if (result is not Task task) { return result; }

            await task;

            return ResultOf(task);
        }

        private object InvokeInner(object target, object[] args)
        {
            try
            {
                if (method.IsConstructor)
                {
                    var ctor = (ConstructorInfo)method;

                    return ctor.Invoke(args);
                }

                if (!method.IsStatic && target == null) { throw new NullReferenceException(); }

                return method.Invoke(target, args);
            }
            catch (TargetInvocationException ex)
            {
                TryThrowingInnerExceptionOf(ex);

                throw;
            }
        }

        private static void TryThrowingInnerExceptionOf(Exception ex)
        {
            if (ex.InnerException == null) { return; }

            ex.InnerException.PreserveStackTrace();

            throw ex.InnerException;
        }

        private static object ResultOf(Task task) =>
            task.GetType().IsGenericType
                ? task.GetType().GetProperty("Result")?.GetValue(task)
                : null;
    }
}
