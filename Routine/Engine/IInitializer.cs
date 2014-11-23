namespace Routine.Engine
{
	public interface IInitializer : IParametric
	{
		bool IsPublic { get; }
		IType InitializedType { get; }

		object Initialize(params object[] parameters);
	}
}