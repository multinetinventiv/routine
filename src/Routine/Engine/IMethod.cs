using System.Threading.Tasks;

namespace Routine.Engine
{
	public interface IMethod : IReturnable, IParametric
	{
		bool IsPublic { get; }

		IType GetDeclaringType(bool firstDeclaringType);

		object PerformOn(object target, params object[] parameters);
        Task<object> PerformOnAsync(object target, params object[] parameters);
	}
}