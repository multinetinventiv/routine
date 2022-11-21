using Routine.Engine.Virtual;
using System.Reflection;

namespace Routine.Engine.Configuration;

public class ParameterBuilder
{
    private readonly IParametric _owner;

    public ParameterBuilder(IParametric owner)
    {
        _owner = owner;
    }

    public IParametric Owner => _owner;

    public VirtualParameter Virtual() => new(_owner);

    public VirtualParameter Virtual(ParameterInfo parameterInfo) =>
        Virtual()
            .Name.Set(parameterInfo.Name)
            .Index.Set(parameterInfo.Position)
            .ParameterType.Set(parameterInfo.ParameterType.ToTypeInfo());
}
