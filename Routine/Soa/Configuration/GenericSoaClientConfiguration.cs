using System;
using Routine.Core;
using Routine.Core.Extractor;
namespace Routine.Soa.Configuration
{
	public class GenericSoaClientConfiguration : ISoaClientConfiguration
	{
		private string ServiceUrlBase{get;set;}
		public GenericSoaClientConfiguration ServiceUrlBaseIs(string serviceUrlBase){ServiceUrlBase = serviceUrlBase; return this;}

		public MultipleExtractor<GenericSoaClientConfiguration, SoaExceptionResult, Exception> ExtractException { get; private set; }

		public GenericSoaClientConfiguration()
		{
			ExtractException = new MultipleExtractor<GenericSoaClientConfiguration, SoaExceptionResult, Exception>(this, "Exception");
		}

		public GenericSoaClientConfiguration Merge(GenericSoaClientConfiguration other)
		{
			ExtractException.Merge(other.ExtractException);

			return this;
		}

		#region ISoaClientConfiguration implementation
		string ISoaClientConfiguration.ServiceUrlBase {get{return ServiceUrlBase;}}

		IExtractor<SoaExceptionResult, Exception> ISoaClientConfiguration.ExceptionExtractor { get { return ExtractException; } }
		#endregion
	}
}

