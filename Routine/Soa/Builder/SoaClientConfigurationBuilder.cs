using System;
using Routine.Soa.Configuration;

namespace Routine.Soa.Builder
{
	public class SoaClientConfigurationBuilder
	{
		public ConventionalSoaClientConfiguration FromScratch()
		{
			return new ConventionalSoaClientConfiguration();
		}

		public ConventionalSoaClientConfiguration FromBasic()
		{
			return FromScratch()
				.Exception.OnFailReturn(new Exception())
				.HeaderValue.OnFailReturn(string.Empty)
				;
		}
	}
}

