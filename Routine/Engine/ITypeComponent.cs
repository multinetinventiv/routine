namespace Routine.Engine
{
	public interface ITypeComponent
	{
		string Name { get; }
		object[] GetCustomAttributes();
		IType ParentType { get; }
	}
}