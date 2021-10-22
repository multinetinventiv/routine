using Routine.Interception;

namespace Routine.Test.Interception.Stubs
{
    public class TestContext<T> : InterceptionContext
    {
        public TestContext(string target) : base(target) { }

        public T Value { get; set; }
    }
}