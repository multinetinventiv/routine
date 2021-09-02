using System;
using Routine.Core.Runtime;

namespace Routine.Core.Reflection
{
    public class ReflectionMethodInvoker : IMethodInvoker
    {
        private readonly System.Reflection.MethodBase method;

        public ReflectionMethodInvoker(System.Reflection.MethodBase method)
        {
            this.method = method;
        }

        public object Invoke(object target, params object[] args)
        {
            try
            {
                if (method.IsConstructor)
                {
                    var ctor = (System.Reflection.ConstructorInfo)method;

                    return ctor.Invoke(args);
                }

                if (!method.IsStatic && target == null) { throw new NullReferenceException(); }

                return method.Invoke(target, args);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                ex.InnerException.PreserveStackTrace();

                throw ex.InnerException;
            }
        }
    }
}
