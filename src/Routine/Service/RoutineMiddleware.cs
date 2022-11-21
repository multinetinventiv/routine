using Microsoft.AspNetCore.Http;
using Routine.Core.Rest;
using Routine.Service.RequestHandlers;
using System.Text.RegularExpressions;

namespace Routine.Service;

public class RoutineMiddleware
{
    private readonly RequestDelegate _next;

    private readonly string _rootPath;
    private readonly bool _enableTestApp;
    private readonly string _testAppPath;
    private readonly IndexRequestHandler _indexHandler;
    private readonly FileRequestHandler _fileHandler;
    private readonly FontsRequestHandler _fontsHandler;
    private readonly ConfigurationRequestHandler _configurationHandler;
    private readonly ApplicationModelRequestHandler _applicationModelHandler;
    private readonly HandleRequestHandler _handleHandler;

    public RoutineMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor, IJsonSerializer jsonSerializer, IServiceContext serviceContext)
    {
        _next = next;

        _rootPath = serviceContext.ServiceConfiguration.GetPath();
        _enableTestApp = serviceContext.ServiceConfiguration.GetEnableTestApp();
        _testAppPath = serviceContext.ServiceConfiguration.GetPath(serviceContext.ServiceConfiguration.GetTestAppPath());
        _indexHandler = new(serviceContext, jsonSerializer, httpContextAccessor);
        _fileHandler = new(serviceContext, jsonSerializer, httpContextAccessor);
        _fontsHandler = new(serviceContext, jsonSerializer, httpContextAccessor);
        _configurationHandler = new(serviceContext, jsonSerializer, httpContextAccessor);
        _applicationModelHandler = new(serviceContext, jsonSerializer, httpContextAccessor);
        _handleHandler = new(serviceContext, jsonSerializer, httpContextAccessor,
            actionFactory: resolution => resolution.HasOperation
                ? new DoRequestHandler(serviceContext, jsonSerializer, httpContextAccessor, resolution)
                : new GetRequestHandler(serviceContext, jsonSerializer, httpContextAccessor, resolution)
        );
    }

    public async Task Invoke(HttpContext context)
    {
        var loweredPath = $"{context.Request.Path}".ToLowerInvariant();
        var loweredRootPath = $"/{_rootPath.ToLowerInvariant()}";
        var loweredTestAppPath = $"/{_testAppPath.ToLowerInvariant()}/";

        if (_enableTestApp &&
            (loweredPath == "/" ||
             loweredPath == loweredRootPath ||
             loweredPath == loweredTestAppPath ||
             $"{loweredPath}/" == $"{loweredRootPath}" ||
             $"{loweredPath}/" == $"{loweredTestAppPath}"))
        {
            await _indexHandler.WriteResponse();
        }
        else if (loweredPath.StartsWith(loweredRootPath) &&
                 !loweredPath.StartsWith(loweredTestAppPath))
        {
            if (loweredPath == $"{loweredRootPath}configuration")
            {
                await _configurationHandler.WriteResponse();
            }
            else if (loweredPath == $"{loweredRootPath}applicationmodel")
            {
                await _applicationModelHandler.WriteResponse();
            }
            else
            {
                await _handleHandler.WriteResponse();
            }
        }
        else if (_enableTestApp &&
                 loweredPath == $"{loweredTestAppPath}file")
        {
            await _fileHandler.WriteResponse();
        }
        else if (_enableTestApp &&
                 Regex.IsMatch(loweredPath, $"{loweredTestAppPath}fonts/[^/]*/f"))
        {
            await _fontsHandler.WriteResponse();
        }
        else
        {
            await _next(context);
        }
    }
}
