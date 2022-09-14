using Routine.Interception;

namespace Routine.Test.Interception.Stubs;

public class Context : InterceptionContext
{
    public Context() : base("test") { }

    public string Value { get; set; }
}
