using System;
using System.Collections.Generic;
using Routine.Core;
using Routine.Core.Extractor;

namespace Routine.Soa.Configuration
{
	public class GenericSoaClientConfiguration : ISoaClientConfiguration
	{
		private string ServiceUrlBase { get; set; }
		public GenericSoaClientConfiguration ServiceUrlBaseIs(string serviceUrlBase){ServiceUrlBase = serviceUrlBase; return this;}

		private List<string> DefaultParameters { get; set; }
		public GenericSoaClientConfiguration DefaultParametersAre(params string[] parameters) { DefaultParameters.AddRange(parameters); return this; }

		public MultipleExtractor<GenericSoaClientConfiguration, SoaExceptionResult, Exception> ExtractException { get; private set; }
		public MultipleExtractor<GenericSoaClientConfiguration, string, string> ExtractDefaultParameterValue { get; private set; }

		public GenericSoaClientConfiguration()
		{
			DefaultParameters = new List<string>();

			ExtractException = new MultipleExtractor<GenericSoaClientConfiguration, SoaExceptionResult, Exception>(this, "Exception");
			ExtractDefaultParameterValue = new MultipleExtractor<GenericSoaClientConfiguration, string, string>(this, "DefaultParameterValue");
		}

		public GenericSoaClientConfiguration Merge(GenericSoaClientConfiguration other)
		{
			DefaultParameters.AddRange(other.DefaultParameters);

			ExtractException.Merge(other.ExtractException);
			ExtractDefaultParameterValue.Merge(other.ExtractDefaultParameterValue);

			return this;
		}

		#region ISoaClientConfiguration implementation

		string ISoaClientConfiguration.ServiceUrlBase { get { return ServiceUrlBase; } }
		List<string> ISoaClientConfiguration.DefaultParameters { get { return DefaultParameters; } }

		IExtractor<SoaExceptionResult, Exception> ISoaClientConfiguration.ExceptionExtractor { get { return ExtractException; } }
		IExtractor<string, string> ISoaClientConfiguration.DefaultParameterValueExtractor { get { return ExtractDefaultParameterValue; } }

		#endregion
	}
}

