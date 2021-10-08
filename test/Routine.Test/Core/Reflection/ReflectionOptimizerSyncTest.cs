using NUnit.Framework;
using Routine.Core.Reflection;

namespace Routine.Test.Core.Reflection
{
    [TestFixture]
    public class ReflectionOptimizerSyncTest : ReflectionOptimizerContract
    {
        protected override object Invoke(IMethodInvoker invoker, object target, params object[] args) => invoker.Invoke(target, args);
    }
}