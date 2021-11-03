using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Routine.Core.Runtime
{
    public static class SystemExtensions
    {
        public static void PreserveStackTrace(this Exception ex) => ExceptionDispatchInfo.Capture(ex).Throw();

        public static Exception GetInnerException(this Exception ex)
        {
            if (ex.InnerException == null) { return ex; }

            ex.InnerException.PreserveStackTrace();

            return ex.InnerException;
        }

        public static object GetResult(this Task task) =>
            task.GetType().IsGenericType
                ? task.GetType().GetProperty("Result")?.GetValue(task)
                : null;

        public static object WaitAndGetResult(this Task task)
        {
            try
            {
                Task.WaitAll(task);
            }
            catch (AggregateException ex)
            {
                throw ex.GetInnerException();
            }

            return task.GetResult();
        }

        public static T WaitAndGetResult<T>(this Task<T> task)
        {
            try
            {
                Task.WaitAll(task);
            }
            catch (AggregateException ex)
            {
                throw ex.GetInnerException();
            }

            return task.Result;
        }
    }
}