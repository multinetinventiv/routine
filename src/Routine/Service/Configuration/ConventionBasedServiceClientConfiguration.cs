using System;
using System.Collections.Generic;
using Routine.Core.Configuration;

namespace Routine.Service.Configuration
{
	public class ConventionBasedServiceClientConfiguration : LayeredBase<ConventionBasedServiceClientConfiguration>, IServiceClientConfiguration
	{
		public SingleConfiguration<ConventionBasedServiceClientConfiguration, string> ServiceUrlBase { get; }

		public ListConfiguration<ConventionBasedServiceClientConfiguration, string> RequestHeaders { get; }

		public ConventionBasedConfiguration<ConventionBasedServiceClientConfiguration, ExceptionResult, Exception> Exception { get; }
		public ConventionBasedConfiguration<ConventionBasedServiceClientConfiguration, string, string> RequestHeaderValue { get; }
		public ListConfiguration<ConventionBasedServiceClientConfiguration, IHeaderProcessor> ResponseHeaderProcessors { get; }

		public ConventionBasedServiceClientConfiguration()
		{
			ServiceUrlBase = new SingleConfiguration<ConventionBasedServiceClientConfiguration, string>(this, "ServiceUrlBase", true);

			RequestHeaders = new ListConfiguration<ConventionBasedServiceClientConfiguration, string>(this, "RequestHeaders");

			Exception = new ConventionBasedConfiguration<ConventionBasedServiceClientConfiguration, ExceptionResult, Exception>(this, "Exception");
			RequestHeaderValue = new ConventionBasedConfiguration<ConventionBasedServiceClientConfiguration, string, string>(this, "RequestHeaderValue");
			ResponseHeaderProcessors = new ListConfiguration<ConventionBasedServiceClientConfiguration, IHeaderProcessor>(this, "ResponseHeaderProcessors");
		}

		public ConventionBasedServiceClientConfiguration Merge(ConventionBasedServiceClientConfiguration other)
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
		List<IHeaderProcessor> IServiceClientConfiguration.GetResponseHeaderProcessors() { return ResponseHeaderProcessors.Get(); }

		#endregion
	}
}

