using Routine.Core.Configuration;
using Routine.Core;
using System.Collections.Generic;
using System;

namespace Routine.Service.Configuration
{
    public class ConventionBasedServiceConfiguration : LayeredBase<ConventionBasedServiceConfiguration>, IServiceConfiguration
    {
        public SingleConfiguration<ConventionBasedServiceConfiguration, string> RootPath { get; }

        public SingleConfiguration<ConventionBasedServiceConfiguration, bool> EnableTestApp { get; }
        public SingleConfiguration<ConventionBasedServiceConfiguration, string> TestAppPath { get; }

        public ConventionBasedConfiguration<ConventionBasedServiceConfiguration, OperationWithObjectModel, bool> AllowGet { get; }

        public ListConfiguration<ConventionBasedServiceConfiguration, string> RequestHeaders { get; }
        public ListConfiguration<ConventionBasedServiceConfiguration, IHeaderProcessor> RequestHeaderProcessors { get; }

        public ListConfiguration<ConventionBasedServiceConfiguration, string> ResponseHeaders { get; }
        public ConventionBasedConfiguration<ConventionBasedServiceConfiguration, string, string> ResponseHeaderValue { get; }

        public ConventionBasedConfiguration<ConventionBasedServiceConfiguration, Exception, ExceptionResult> ExceptionResult { get; }

        public ConventionBasedServiceConfiguration()
        {
            RootPath = new SingleConfiguration<ConventionBasedServiceConfiguration, string>(this, nameof(RootPath), true);

            EnableTestApp = new SingleConfiguration<ConventionBasedServiceConfiguration, bool>(this, nameof(EnableTestApp), true);
            TestAppPath = new SingleConfiguration<ConventionBasedServiceConfiguration, string>(this, nameof(TestAppPath), true);

            AllowGet = new ConventionBasedConfiguration<ConventionBasedServiceConfiguration, OperationWithObjectModel, bool>(this, nameof(AllowGet), true);

            RequestHeaders = new ListConfiguration<ConventionBasedServiceConfiguration, string>(this, nameof(RequestHeaders));
            RequestHeaderProcessors = new ListConfiguration<ConventionBasedServiceConfiguration, IHeaderProcessor>(this, nameof(RequestHeaderProcessors));

            ResponseHeaders = new ListConfiguration<ConventionBasedServiceConfiguration, string>(this, nameof(ResponseHeaders));
            ResponseHeaderValue = new ConventionBasedConfiguration<ConventionBasedServiceConfiguration, string, string>(this, nameof(ResponseHeaderValue));

            ExceptionResult = new ConventionBasedConfiguration<ConventionBasedServiceConfiguration, Exception, ExceptionResult>(this, nameof(ExceptionResult));
        }

        public ConventionBasedServiceConfiguration Merge(ConventionBasedServiceConfiguration other)
        {
            AllowGet.Merge(other.AllowGet);
            RequestHeaders.Merge(other.RequestHeaders);
            RequestHeaderProcessors.Merge(other.RequestHeaderProcessors);
            ResponseHeaders.Merge(other.ResponseHeaders);
            ResponseHeaderValue.Merge(other.ResponseHeaderValue);
            ExceptionResult.Merge(other.ExceptionResult);

            return this;
        }

        #region IServiceConfiguration implementation

        string IServiceConfiguration.GetRootPath() => RootPath.Get();
        bool IServiceConfiguration.GetEnableTestApp() => EnableTestApp.Get();
        string IServiceConfiguration.GetTestAppPath() => TestAppPath.Get();
        bool IServiceConfiguration.GetAllowGet(ObjectModel objectModel, OperationModel operationModel) => AllowGet.Get(new OperationWithObjectModel(objectModel, operationModel));
        List<string> IServiceConfiguration.GetRequestHeaders() => RequestHeaders.Get();
        List<IHeaderProcessor> IServiceConfiguration.GetRequestHeaderProcessors() => RequestHeaderProcessors.Get();
        List<string> IServiceConfiguration.GetResponseHeaders() => ResponseHeaders.Get();
        string IServiceConfiguration.GetResponseHeaderValue(string responseHeader) => ResponseHeaderValue.Get(responseHeader);
        ExceptionResult IServiceConfiguration.GetExceptionResult(Exception exception) => ExceptionResult.Get(exception);

        #endregion
    }
}
