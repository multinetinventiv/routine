using Moq;
using NUnit.Framework;
using Routine.Core.Reflection;
using System.Threading.Tasks;

namespace Routine.Test.Core.Reflection
{
    [TestFixture]
    public class ReflectionOptimizerSyncTest : ReflectionOptimizerContract
    {
        protected override object Invoke(IMethodInvoker invoker, object target, params object[] args) => invoker.Invoke(target, args);
        
        [Test]
        public void Waits_for_async_methods_to_run()
        {
            var testing = InvokerFor<OptimizedClass>(nameof(OptimizedClass.AsyncVoidMethod));

            var actual = testing.Invoke(target);

            Assert.IsNotInstanceOf<Task>(actual);
            Assert.IsNull(actual);
            mock.Verify(o => o.AsyncVoidMethod());
        }

        [Test]
        public void Returns_task_result_for_async_methods_with_return_value()
        {
            mock.Setup(o => o.AsyncStringMethod()).ReturnsAsync("test");

            var testing = InvokerFor<OptimizedClass>(nameof(OptimizedClass.AsyncStringMethod));

            var actual = testing.Invoke(target);

            Assert.AreEqual("test", actual);
        }
    }
}