namespace Routine.Engine;

public interface IParametric : ITypeComponent
{
    List<IParameter> Parameters { get; }
}
