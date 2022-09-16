using Routine.Core.Rest;
using System.Linq.Expressions;
using System.Net;

namespace Routine.Test.Service.Stubs;

public interface IRestClientStubber
{
    public void SetUpGet(Mock<IRestClient> mock, string url, string response) => SetUpGet(mock, url, req => true, response);
    public void SetUpGet(Mock<IRestClient> mock, string url, RestResponse response) => SetUpGet(mock, url, req => true, response);
    public void SetUpGet(Mock<IRestClient> mock, string url, Expression<Func<RestRequest, bool>> match, string response) => SetUpGet(mock, url, match, new RestResponse(response));
    void SetUpGet(Mock<IRestClient> mock, string url, Expression<Func<RestRequest, bool>> match, RestResponse response);

    public void SetUpGet(Mock<IRestClient> mock, string url, WebException exception) => SetUpGet(mock, url, req => true, exception);
    void SetUpGet(Mock<IRestClient> mock, string url, Expression<Func<RestRequest, bool>> match, WebException exception);

    public void SetUpPost(Mock<IRestClient> mock, string url, string response) => SetUpPost(mock, url, req => true, response);
    public void SetUpPost(Mock<IRestClient> mock, string url, RestResponse response) => SetUpPost(mock, url, req => true, response);
    public void SetUpPost(Mock<IRestClient> mock, string url, string body, string response) => SetUpPost(mock, url, req => req.Body == body, response);
    public void SetUpPost(Mock<IRestClient> mock, string url, string body, RestResponse response) => SetUpPost(mock, url, req => req.Body == body, response);
    public void SetUpPost(Mock<IRestClient> mock, string url, Expression<Func<RestRequest, bool>> match, string response) => SetUpPost(mock, url, match, new RestResponse(response));
    void SetUpPost(Mock<IRestClient> mock, string url, Expression<Func<RestRequest, bool>> match, RestResponse response);

    public void SetUpPost(Mock<IRestClient> mock, string url, WebException exception) => SetUpPost(mock, url, req => true, exception);
    void SetUpPost(Mock<IRestClient> mock, string url, Expression<Func<RestRequest, bool>> match, WebException exception);

    void VerifyGet(Mock<IRestClient> mock, Expression<Func<RestRequest, bool>> match);
    void VerifyPost(Mock<IRestClient> mock, Expression<Func<RestRequest, bool>> match);
}
