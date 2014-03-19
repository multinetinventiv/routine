using System;
using Routine.Soa.Configuration;

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
			return FromScratch()
				.ExtractExceptionResult.OnFailReturn(new SoaExceptionResult());
		}
	}
}

