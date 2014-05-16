using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Routine.Core.Configuration;

namespace Routine.Core.Builder
{
	public class InterceptionConfigurationBuilder
	{
		public GenericInterceptionConfiguration FromScratch() { return new GenericInterceptionConfiguration(); }

		public GenericInterceptionConfiguration FromBasic()
		{
			return FromScratch()
				;
		}
	}
}
