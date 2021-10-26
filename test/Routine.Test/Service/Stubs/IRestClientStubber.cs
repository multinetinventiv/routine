using Moq;
using Routine.Core.Rest;
using System;
using System.Linq.Expressions;
using System.Net;

namespace Routine.Test.Service.Stubs
{
    public interface IRestClientStubber
    {
        public void SetUp(Mock<IRestClient> mock, string url, string response) => SetUp(mock, url, new RestResponse(response));
        public void SetUp(Mock<IRestClient> mock, string url, RestResponse response) => SetUp(mock, url, req => true, response);
        public void SetUp(Mock<IRestClient> mock, string url, string body, string response) => SetUp(mock, url, body, new RestResponse(response));
        public void SetUp(Mock<IRestClient> mock, string url, string body, RestResponse response) => SetUp(mock, url, req => req.Body == body, response);
        public void SetUp(Mock<IRestClient> mock, string url, Expression<Func<RestRequest, bool>> restRequestMatcher, string response) => SetUp(mock, url, restRequestMatcher, new RestResponse(response));
        void SetUp(Mock<IRestClient> mock, string url, Expression<Func<RestRequest, bool>> restRequestMatcher, RestResponse response);

        public void SetUp(Mock<IRestClient> mock, string url, WebException exception) => SetUp(mock, url, req => true, exception);
        void SetUp(Mock<IRestClient> mock, string url, Expression<Func<RestRequest, bool>> restRequestMatcher, WebException exception);
    }
}