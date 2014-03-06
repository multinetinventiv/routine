using System.Collections.Generic;

namespace Routine.Core
{
	public interface IOperation : IObjectItem
	{
		List<IParameter> Parameters { get; }

		object PerformOn(object target, params object[] parameters);
	}
}
