using System;
using System.Collections.Generic;
using Routine.Core;
using Routine.Core.Configuration;

namespace Routine.Service.Configuration
{
	public class ConventionBasedServiceConfiguration : LayeredBase<ConventionBasedServiceConfiguration>, IServiceConfiguration
	{
		public SingleConfiguration<ConventionBasedServiceConfiguration, string> RootPath { get; }

		public ConventionBasedConfiguration<ConventionBasedServiceConfiguration, OperationWithObjectModel, bool> AllowGet { get; }

		public ListConfiguration<ConventionBasedServiceConfiguration, string> RequestHeaders { get; }
		public ListConfiguration<ConventionBasedServiceConfiguration, IHeaderProcessor> RequestHeaderProcessors { get; }

		public ListConfiguration<ConventionBasedServiceConfiguration, string> ResponseHeaders { get; }
		public ConventionBasedConfiguration<ConventionBasedServiceConfiguration, string, string> ResponseHeaderValue { get; }

		public ConventionBasedConfiguration<ConventionBasedServiceConfiguration, Exception, ExceptionResult> ExceptionResult { get; }

		public ConventionBasedServiceConfiguration()
		{
			RootPath = new SingleConfiguration<ConventionBasedServiceConfiguration, string>(this, "RootPath", true);

			AllowGet = new ConventionBasedConfiguration<ConventionBasedServiceConfiguration, OperationWithObjectModel, bool>(this, "AllowGet", true);

			RequestHeaders = new ListConfiguration<ConventionBasedServiceConfiguration, string>(this, "RequestHeaders");
			RequestHeaderProcessors = new ListConfiguration<ConventionBasedServiceConfiguration, IHeaderProcessor>(this, "RequestHeaderProcessors");

			ResponseHeaders = new ListConfiguration<ConventionBasedServiceConfiguration, string>(this, "ResponseHeaders");
			ResponseHeaderValue = new ConventionBasedConfiguration<ConventionBasedServiceConfiguration, string, string>(this, "ResponseHeaderValue");

			ExceptionResult = new ConventionBasedConfiguration<ConventionBasedServiceConfiguration, Exception, ExceptionResult>(this, "ExceptionResult");
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

		string IServiceConfiguration.GetRootPath() { return RootPath.Get(); }
		bool IServiceConfiguration.GetAllowGet(ObjectModel objectModel, OperationModel operationModel) { return AllowGet.Get(new OperationWithObjectModel(objectModel, operationModel)); }
		List<string> IServiceConfiguration.GetRequestHeaders() { return RequestHeaders.Get(); }
		List<IHeaderProcessor> IServiceConfiguration.GetRequestHeaderProcessors() { return RequestHeaderProcessors.Get(); }
		List<string> IServiceConfiguration.GetResponseHeaders() { return ResponseHeaders.Get(); }
		string IServiceConfiguration.GetResponseHeaderValue(string responseHeader) { return ResponseHeaderValue.Get(responseHeader); }
		ExceptionResult IServiceConfiguration.GetExceptionResult(Exception exception) { return ExceptionResult.Get(exception); }

		#endregion
	}
}

