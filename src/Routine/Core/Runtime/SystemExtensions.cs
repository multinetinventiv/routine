using System;
using System.Runtime.ExceptionServices;

namespace Routine.Core.Runtime
{
    public static class SystemExtensions
    {
        public static void PreserveStackTrace(this Exception ex)
        {
            ExceptionDispatchInfo.Capture(ex).Throw();
        }
    }
}