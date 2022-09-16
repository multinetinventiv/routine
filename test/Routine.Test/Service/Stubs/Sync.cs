﻿using Routine.Core.Rest;
using System.Linq.Expressions;
using System.Net;

namespace Routine.Test.Service.Stubs;

public class Sync : IRestClientStubber
{
    public void SetUpGet(
        Mock<IRestClient> mock,
        string url,
        Expression<Func<RestRequest, bool>> match,
        RestResponse response
    ) => mock.Setup(rc => rc.Get(url, It.Is(match))).Returns(response);

    public void SetUpGet(
        Mock<IRestClient> mock,
        string url,
        Expression<Func<RestRequest, bool>> match,
        WebException exception
    ) => mock.Setup(rc => rc.Get(url, It.Is(match))).Throws(exception);

    public void SetUpPost(Mock<IRestClient> mock,
        string url,
        Expression<Func<RestRequest, bool>> match,
        RestResponse response
    ) => mock.Setup(rc => rc.Post(url, It.Is(match))).Returns(response);

    public void SetUpPost(Mock<IRestClient> mock,
        string url,
        Expression<Func<RestRequest, bool>> match,
        WebException exception
    ) => mock.Setup(rc => rc.Post(url, It.Is(match))).Throws(exception);

    public void VerifyPost(
        Mock<IRestClient> mock,
        Expression<Func<RestRequest, bool>> match
    ) => mock.Verify(rc => rc.Post(It.IsAny<string>(), It.Is(match)));
}
