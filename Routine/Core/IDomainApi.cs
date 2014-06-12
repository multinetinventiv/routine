using System.Collections.Generic;

namespace Routine.Core
{
	public interface ITypeComponent
	{
		string Name { get; }
		TypeInfo Type { get; }
		object[] GetCustomAttributes();
	}

	public interface IReturnable : ITypeComponent
	{
		TypeInfo ReturnType { get; }
	}

	public interface IParametric : ITypeComponent
	{
		List<IParameter> Parameters { get; }
	}

	public interface IInitializer : IParametric
	{
		TypeInfo InitializedType { get; }

		object Initialize(params object[] parameters);
	}

	public interface IMember : IReturnable
	{
		object FetchFrom(object target);
	}

	public interface IOperation : IReturnable, IParametric
	{
		object PerformOn(object target, params object[] parameters);
	}

	public interface IParameter : ITypeComponent
	{
		IParametric Owner { get; }
		int Index { get; }
		TypeInfo ParameterType { get; }
	}
}

