using System;
using System.Collections.Generic;
using Routine.Core.Configuration;

namespace Routine.Soa.Configuration
{
	public class ConventionalSoaClientConfiguration : LayeredBase<ConventionalSoaClientConfiguration>, ISoaClientConfiguration
	{
		public SingleConfiguration<ConventionalSoaClientConfiguration, string> ServiceUrlBase { get; private set; }

		public ListConfiguration<ConventionalSoaClientConfiguration, string> Headers { get; private set; }

		public ConventionalConfiguration<ConventionalSoaClientConfiguration, SoaExceptionResult, Exception> Exception { get; private set; }
		public ConventionalConfiguration<ConventionalSoaClientConfiguration, string, string> HeaderValue { get; private set; }

		public ConventionalSoaClientConfiguration()
		{
			ServiceUrlBase = new SingleConfiguration<ConventionalSoaClientConfiguration, string>(this, "ServiceUrlBase", true);
			Headers = new ListConfiguration<ConventionalSoaClientConfiguration, string>(this, "Headers");
			Exception = new ConventionalConfiguration<ConventionalSoaClientConfiguration, SoaExceptionResult, Exception>(this, "Exception");
			HeaderValue = new ConventionalConfiguration<ConventionalSoaClientConfiguration, string, string>(this, "HeaderValue");
		}

		public ConventionalSoaClientConfiguration Merge(ConventionalSoaClientConfiguration other)
		{
			Headers.Merge(other.Headers);
			Exception.Merge(other.Exception);
			HeaderValue.Merge(other.HeaderValue);

			return this;
		}

		#region ISoaClientConfiguration implementation

		string ISoaClientConfiguration.GetServiceUrlBase() { return ServiceUrlBase.Get(); }
		List<string> ISoaClientConfiguration.GetHeaders() { return Headers.Get(); }
		Exception ISoaClientConfiguration.GetException(SoaExceptionResult exceptionResult) { return Exception.Get(exceptionResult); }
		string ISoaClientConfiguration.GetHeaderValue(string header) { return HeaderValue.Get(header); }

		#endregion
	}
}

