using Routine.Engine.Virtual;

namespace Routine.Engine.Configuration;

public class VirtualTypeBuilder
{
    public VirtualType FromBasic() =>
        new VirtualType()
            .ToStringMethod.Set(o => $"{o.Id} ({o.Type})");
}
