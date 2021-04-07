namespace Routine.Engine
{
	public interface IMethod : IReturnable, IParametric
	{
		bool IsPublic { get; }

		IType GetDeclaringType(bool firstDeclaringType);

		object PerformOn(object target, params object[] parameters);
	}
}