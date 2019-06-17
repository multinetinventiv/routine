namespace Routine.Engine
{
	public interface IProperty : IReturnable
	{
		bool IsPublic { get; }

		IType GetDeclaringType(bool firstDeclaringType);

		object FetchFrom(object target);
	}
}