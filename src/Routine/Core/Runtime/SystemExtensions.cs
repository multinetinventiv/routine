using System;
using System.Runtime.ExceptionServices;

namespace Routine.Core.Runtime
{
    public static class SystemExtensions
    {
        //TODO couldn't find a better method, .net 4.5 has a fix for this
        public static void PreserveStackTrace(this Exception ex)
        {
            //todo: https://stackoverflow.com/questions/57383/how-to-rethrow-innerexception-without-losing-stack-trace-in-c#answer-17091351
            ExceptionDispatchInfo.Capture(ex).Throw();
        }
    }
}