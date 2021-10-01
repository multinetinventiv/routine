namespace Routine.Engine
{
	public interface IParameter : ITypeComponent
	{
		IParametric Owner { get; }
		int Index { get; }
		IType ParameterType { get; }
		bool IsOptional { get; }
		bool HasDefaultValue { get; }
		object DefaultValue { get; }
	}
}

