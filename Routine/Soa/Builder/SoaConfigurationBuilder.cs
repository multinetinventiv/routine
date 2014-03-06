using System;
using Routine.Soa.SoaConfiguration;

namespace Routine.Soa.Builder
{
	public class SoaConfigurationBuilder
	{
		public GenericSoaConfiguration FromScratch()
		{
			return new GenericSoaConfiguration();
		}

		public GenericSoaConfiguration FromBasic()
		{
			return FromScratch();
		}
	}
}

