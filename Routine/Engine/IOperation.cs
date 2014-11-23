namespace Routine.Engine
{
	public interface IOperation : IReturnable, IParametric
	{
		bool IsPublic { get; }

		IType GetDeclaringType(bool firstDeclaringType);

		object PerformOn(object target, params object[] parameters);
	}
}