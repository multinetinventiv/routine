using System;
using System.Collections.Generic;
using Routine.Core.Configuration;

namespace Routine.Service.Configuration
{
	public class ConventionalServiceClientConfiguration : LayeredBase<ConventionalServiceClientConfiguration>, IServiceClientConfiguration
	{
		public SingleConfiguration<ConventionalServiceClientConfiguration, string> ServiceUrlBase { get; private set; }

		public ListConfiguration<ConventionalServiceClientConfiguration, string> RequestHeaders { get; private set; }

		public ConventionalConfiguration<ConventionalServiceClientConfiguration, ExceptionResult, Exception> Exception { get; private set; }
		public ConventionalConfiguration<ConventionalServiceClientConfiguration, string, string> RequestHeaderValue { get; private set; }
		public ListConfiguration<ConventionalServiceClientConfiguration, IResponseHeaderProcessor> ResponseHeaderProcessors { get; private set; }

		public ConventionalServiceClientConfiguration()
		{
			ServiceUrlBase = new SingleConfiguration<ConventionalServiceClientConfiguration, string>(this, "ServiceUrlBase", true);

			RequestHeaders = new ListConfiguration<ConventionalServiceClientConfiguration, string>(this, "RequestHeaders");

			Exception = new ConventionalConfiguration<ConventionalServiceClientConfiguration, ExceptionResult, Exception>(this, "Exception");
			RequestHeaderValue = new ConventionalConfiguration<ConventionalServiceClientConfiguration, string, string>(this, "RequestHeaderValue");
			ResponseHeaderProcessors = new ListConfiguration<ConventionalServiceClientConfiguration, IResponseHeaderProcessor>(this, "ResponseHeaderProcessors");
		}

		public ConventionalServiceClientConfiguration Merge(ConventionalServiceClientConfiguration other)
		{
			RequestHeaders.Merge(other.RequestHeaders);

			Exception.Merge(other.Exception);
			RequestHeaderValue.Merge(other.RequestHeaderValue);
			ResponseHeaderProcessors.Merge(other.ResponseHeaderProcessors);

			return this;
		}

		#region IServiceClientConfiguration implementation

		string IServiceClientConfiguration.GetServiceUrlBase() { return ServiceUrlBase.Get(); }
		List<string> IServiceClientConfiguration.GetRequestHeaders() { return RequestHeaders.Get(); }
		Exception IServiceClientConfiguration.GetException(ExceptionResult exceptionResult) { return Exception.Get(exceptionResult); }
		string IServiceClientConfiguration.GetRequestHeaderValue(string requestHeader) { return RequestHeaderValue.Get(requestHeader); }
		List<IResponseHeaderProcessor> IServiceClientConfiguration.GetResponseHeaderProcessors() { return ResponseHeaderProcessors.Get(); }

		#endregion
	}
}

