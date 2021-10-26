using Moq;
using Routine.Core.Rest;
using System;
using System.Linq.Expressions;
using System.Net;

namespace Routine.Test.Service.Stubs
{
    public class Sync : IRestClientStubber
    {
        public void SetUp(Mock<IRestClient> mock,
            string url,
            Expression<Func<RestRequest, bool>> restRequestMatcher,
            RestResponse response
        ) => mock.Setup(rc => rc.Post(url, It.Is(restRequestMatcher))).Returns(response);

        public void SetUp(Mock<IRestClient> mock,
            string url,
            Expression<Func<RestRequest, bool>> restRequestMatcher,
            WebException exception
        ) => mock.Setup(rc => rc.Post(url, It.Is(restRequestMatcher))).Throws(exception);
    }
}