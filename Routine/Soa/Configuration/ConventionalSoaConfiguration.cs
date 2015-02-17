using System;
using System.Collections.Generic;
using Routine.Core.Configuration;

namespace Routine.Soa.Configuration
{
	public class ConventionalSoaConfiguration : LayeredBase<ConventionalSoaConfiguration>, ISoaConfiguration
	{
		public SingleConfiguration<ConventionalSoaConfiguration, string> RootPath { get; private set; }
		public SingleConfiguration<ConventionalSoaConfiguration, int> MaxResultLength { get; private set; }
		public ListConfiguration<ConventionalSoaConfiguration, string> Headers { get; private set; }
		public ConventionalConfiguration<ConventionalSoaConfiguration, Exception, SoaExceptionResult> ExceptionResult { get; private set; }

		public ConventionalSoaConfiguration()
		{
			RootPath = new SingleConfiguration<ConventionalSoaConfiguration, string>(this, "RootPath", true);
			MaxResultLength = new SingleConfiguration<ConventionalSoaConfiguration, int>(this, "MaxResultLength", true);
			Headers = new ListConfiguration<ConventionalSoaConfiguration, string>(this, "Headers");
			ExceptionResult = new ConventionalConfiguration<ConventionalSoaConfiguration, Exception, SoaExceptionResult>(this, "ExceptionResult");
		}

		public ConventionalSoaConfiguration Merge(ConventionalSoaConfiguration other)
		{
			Headers.Merge(other.Headers);
			ExceptionResult.Merge(other.ExceptionResult);

			return this;
		}

		#region ISoaConfiguration implementation

		string ISoaConfiguration.GetRootPath() { return RootPath.Get(); }
		List<string> ISoaConfiguration.GetHeaders() { return Headers.Get(); }
		int ISoaConfiguration.GetMaxResultLength() { return MaxResultLength.Get(); }
		SoaExceptionResult ISoaConfiguration.GetExceptionResult(Exception exception) { return ExceptionResult.Get(exception); }

		#endregion
	}
}

