using System;
using NUnit.Framework;
using Routine.Core.Reflection;

namespace Routine.Test.Core.Reflection
{
    [TestFixture]
    public class ReflectionMethodInvokerTest
    {
        public void Throw(CustomException ex) => throw ex;

        [Test]
        public void When_exception_occurs_invoker_should_throw_the_actual_exception()
        {
            var expected = new CustomException("message");

            var testing = new ReflectionMethodInvoker(GetType().GetMethod("Throw"));

            try
            {
                testing.Invoke(this, expected);
                Assert.Fail("exception not thrown");
            }
            catch (Exception actual)
            {
                Assert.AreSame(expected, actual);
            }
        }

        [Test]
        public void When_throwing_actual_exception_it_preserves_stack_trace()
        {
            var expected = new CustomException("message");

            var testing = new ReflectionMethodInvoker(GetType().GetMethod("Throw"));

            try
            {
                testing.Invoke(this, expected);
                Assert.Fail("exception not thrown");
            }
            catch (CustomException actual)
            {
                Console.WriteLine(actual.StackTrace);

                Assert.IsTrue(actual.StackTrace.Contains("ReflectionMethodInvokerTest.Throw"), actual.StackTrace);
            }
        }

        public class CustomException : Exception
        {
            public CustomException(string message) : base(message) { }
        }
    }
}
