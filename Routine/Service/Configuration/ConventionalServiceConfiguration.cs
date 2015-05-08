using System;
using System.Collections.Generic;
using Routine.Core.Configuration;

namespace Routine.Service.Configuration
{
	public class ConventionalServiceConfiguration : LayeredBase<ConventionalServiceConfiguration>, IServiceConfiguration
	{
		public SingleConfiguration<ConventionalServiceConfiguration, string> RootPath { get; private set; }
		public SingleConfiguration<ConventionalServiceConfiguration, int> MaxResultLength { get; private set; }
		public ListConfiguration<ConventionalServiceConfiguration, string> RequestHeaders { get; private set; }
		public ConventionalConfiguration<ConventionalServiceConfiguration, Exception, ExceptionResult> ExceptionResult { get; private set; }
		public ListConfiguration<ConventionalServiceConfiguration, string> ResponseHeaders { get; private set; }
		public ConventionalConfiguration<ConventionalServiceConfiguration, string, string> ResponseHeaderValue { get; private set; }

		public ConventionalServiceConfiguration()
		{
			RootPath = new SingleConfiguration<ConventionalServiceConfiguration, string>(this, "RootPath", true);
			MaxResultLength = new SingleConfiguration<ConventionalServiceConfiguration, int>(this, "MaxResultLength", true);
			RequestHeaders = new ListConfiguration<ConventionalServiceConfiguration, string>(this, "RequestHeaders");
			ExceptionResult = new ConventionalConfiguration<ConventionalServiceConfiguration, Exception, ExceptionResult>(this, "ExceptionResult");
			ResponseHeaders = new ListConfiguration<ConventionalServiceConfiguration, string>(this, "ResponseHeaders");
			ResponseHeaderValue = new ConventionalConfiguration<ConventionalServiceConfiguration, string, string>(this, "ResponseHeaderValue");
		}

		public ConventionalServiceConfiguration Merge(ConventionalServiceConfiguration other)
		{
			RequestHeaders.Merge(other.RequestHeaders);
			ExceptionResult.Merge(other.ExceptionResult);
			ResponseHeaders.Merge(other.ResponseHeaders);
			ResponseHeaderValue.Merge(other.ResponseHeaderValue);

			return this;
		}

		#region IServiceConfiguration implementation

		string IServiceConfiguration.GetRootPath() { return RootPath.Get(); }
		int IServiceConfiguration.GetMaxResultLength() { return MaxResultLength.Get(); }
		List<string> IServiceConfiguration.GetRequestHeaders() { return RequestHeaders.Get(); }
		ExceptionResult IServiceConfiguration.GetExceptionResult(Exception exception) { return ExceptionResult.Get(exception); }
		List<string> IServiceConfiguration.GetResponseHeaders() { return ResponseHeaders.Get(); }
		string IServiceConfiguration.GetResponseHeaderValue(string responseHeader) { return ResponseHeaderValue.Get(responseHeader); }

		#endregion
	}
}

