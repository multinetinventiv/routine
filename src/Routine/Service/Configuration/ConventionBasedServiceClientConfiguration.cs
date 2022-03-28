using Routine.Core.Configuration;
using System.Collections.Generic;
using System;

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
            ServiceUrlBase = new SingleConfiguration<ConventionBasedServiceClientConfiguration, string>(this, nameof(ServiceUrlBase), true);

            RequestHeaders = new ListConfiguration<ConventionBasedServiceClientConfiguration, string>(this, nameof(RequestHeaders));

            Exception = new ConventionBasedConfiguration<ConventionBasedServiceClientConfiguration, ExceptionResult, Exception>(this, nameof(Exception));
            RequestHeaderValue = new ConventionBasedConfiguration<ConventionBasedServiceClientConfiguration, string, string>(this, nameof(RequestHeaderValue));
            ResponseHeaderProcessors = new ListConfiguration<ConventionBasedServiceClientConfiguration, IHeaderProcessor>(this, nameof(ResponseHeaderProcessors));
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

        string IServiceClientConfiguration.GetServiceUrlBase() => ServiceUrlBase.Get();
        List<string> IServiceClientConfiguration.GetRequestHeaders() => RequestHeaders.Get();
        Exception IServiceClientConfiguration.GetException(ExceptionResult exceptionResult) => Exception.Get(exceptionResult);
        string IServiceClientConfiguration.GetRequestHeaderValue(string requestHeader) => RequestHeaderValue.Get(requestHeader);
        List<IHeaderProcessor> IServiceClientConfiguration.GetResponseHeaderProcessors() => ResponseHeaderProcessors.Get();

        #endregion
    }
}
