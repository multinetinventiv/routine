using System.Collections.Generic;

namespace Routine.Core
{
	public interface IObjectItem
	{
		string Name { get; }
	}

	public interface IReturnItem : IObjectItem
	{
		TypeInfo Type { get; }
		TypeInfo ReturnType { get; }
	}
	
	public interface IMember : IReturnItem
	{
		bool CanFetchFrom(object target);
		object FetchFrom(object target);
	}
	
	public interface IOperation : IReturnItem
	{
		List<IParameter> Parameters { get; }

		object PerformOn(object target, params object[] parameters);
	}

	public interface IParameter : IObjectItem
	{
		IOperation Operation { get; }
		int Index { get; }
		TypeInfo ParameterType { get; }
	}
}

