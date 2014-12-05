using System;

namespace Routine.Soa.Configuration
{
	public class SoaClientConfigurationBuilder
	{
		public ConventionalSoaClientConfiguration FromBasic()
		{
			return new ConventionalSoaClientConfiguration()
				.Exception.Set(new Exception())
				.HeaderValue.Set(string.Empty)

				.NextLayer()
				;
		}
	}
}

