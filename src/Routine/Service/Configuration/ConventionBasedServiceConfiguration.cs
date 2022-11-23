using Routine.Core.Configuration;
using Routine.Core;

namespace Routine.Service.Configuration;

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
        RootPath = new(this, nameof(RootPath), true);

        EnableTestApp = new(this, nameof(EnableTestApp), true);
        TestAppPath = new(this, nameof(TestAppPath), true);

        AllowGet = new(this, nameof(AllowGet), true);

        RequestHeaders = new(this, nameof(RequestHeaders));
        RequestHeaderProcessors = new(this, nameof(RequestHeaderProcessors));

        ResponseHeaders = new(this, nameof(ResponseHeaders));
        ResponseHeaderValue = new(this, nameof(ResponseHeaderValue));

        ExceptionResult = new(this, nameof(ExceptionResult));
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
