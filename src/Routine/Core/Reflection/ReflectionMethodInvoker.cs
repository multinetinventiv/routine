using Routine.Core.Runtime;
using System.Reflection;
using System.Threading.Tasks;
using System;

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

            return task.WaitAndGetResult();
        }

        public async Task<object> InvokeAsync(object target, params object[] args)
        {
            var result = InvokeInner(target, args);

            if (result is not Task task) { return result; }

            await task;

            return task.GetResult();
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
                throw ex.GetInnerException();
            }
        }
    }
}
