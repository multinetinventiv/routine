using System.Collections.Generic;
using System.Linq;

namespace Routine.Engine
{
	internal class DomainParameterGroup<T> where T : class, IParametric
	{
		public T Parametric { get; private set; }
		public List<DomainParameter> Parameters { get; private set; }
		public int GroupIndex { get; private set; }

		public DomainParameterGroup(T parametric, IEnumerable<DomainParameter> parameters, int groupIndex)
		{
			Parametric = parametric;
			Parameters = parameters.ToList();
			GroupIndex = groupIndex;
		}
	}
}