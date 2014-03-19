using System;
using Routine.Soa.Configuration;

namespace Routine.Soa.Builder
{
	public class SoaClientConfigurationBuilder
	{
		public GenericSoaClientConfiguration FromScratch()
		{
			return new GenericSoaClientConfiguration();
		}

		public GenericSoaClientConfiguration FromBasic()
		{
			return FromScratch()
				.ExtractException.OnFailReturn(new Exception());
		}
	}
}

