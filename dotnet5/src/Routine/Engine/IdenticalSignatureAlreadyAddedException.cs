using System;
using System.Linq;

namespace Routine.Engine
{
	public class IdenticalSignatureAlreadyAddedException : Exception
	{
		public IdenticalSignatureAlreadyAddedException(IParametric parametric)
			: base(
				string.Format("{0}({1}) already added",
					parametric.Name,
					string.Join(", ", parametric.Parameters.Select(p =>
						string.Format("{0} {1}",
							p.ParameterType.Name,
							p.Name))))) { }
	}
}