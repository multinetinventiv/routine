using System;

namespace Routine.Service.Configuration
{
	public class ServiceClientConfigurationBuilder
	{
		public ConventionalServiceClientConfiguration FromBasic()
		{
			return new ConventionalServiceClientConfiguration()
				.Exception.Set(new Exception())
				.RequestHeaderValue.Set(string.Empty)

				.NextLayer()
				;
		}
	}
}

