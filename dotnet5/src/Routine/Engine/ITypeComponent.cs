namespace Routine.Engine
{
	public interface ITypeComponent
	{
		string Name { get; }
		IType ParentType { get; }

		object[] GetCustomAttributes();
	}
}