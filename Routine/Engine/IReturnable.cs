namespace Routine.Engine
{
	public interface IReturnable : ITypeComponent
	{
		IType ReturnType { get; }
		object[] GetReturnTypeCustomAttributes();
	}
}