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
			Parameters = parameters.OrderBy(p => parametric.Parameters.Single(p2 => p2.Name == p.Id).Index).ToList();
			GroupIndex = groupIndex;
		}

		public bool ContainsSameParameters(T parametric)
		{
			return Parametric.Parameters.Count == parametric.Parameters.Count &&
			       Parametric.Parameters.All(p1 => parametric.Parameters.Any(p2 => p1.Name == p2.Name));
		}
	}
}