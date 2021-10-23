using Routine.Interception;

namespace Routine.Test.Interception.Stubs
{
    public class TestContext : InterceptionContext
    {
        public TestContext(string target) : base(target) { }

        public string Value { get; set; }
    }
}