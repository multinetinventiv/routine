namespace Routine.Engine
{
	public interface IMember : IReturnable
	{
		bool IsPublic { get; }

		IType GetDeclaringType(bool firstDeclaringType);

		object FetchFrom(object target);
	}
}