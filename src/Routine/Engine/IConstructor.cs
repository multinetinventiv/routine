namespace Routine.Engine;

public interface IConstructor : IParametric
{
    bool IsPublic { get; }
    IType InitializedType { get; }

    object Initialize(params object[] parameters);
}
