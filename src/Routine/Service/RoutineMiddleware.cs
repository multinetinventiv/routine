using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Routine.Core.Rest;
using Routine.Service.RequestHandlers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Routine.Service
{
    public class RoutineMiddleware
	{
		private readonly RequestDelegate next;

		private readonly string rootPath;
		private readonly IndexRequestHandler indexHandler;
		private readonly FileRequestHandler fileHandler;
		private readonly FontsRequestHandler fontsHandler;
		private readonly ConfigurationRequestHandler configurationHandler;
		private readonly ApplicationModelRequestHandler applicationModelHandler;
		private readonly HandleRequestHandler handleHandler;

		public RoutineMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, IJsonSerializer jsonSerializer, IServiceContext serviceContext)
		{
			this.next = next;

			rootPath = serviceContext.ServiceConfiguration.GetPath();
			indexHandler = new IndexRequestHandler(serviceContext, jsonSerializer, httpContextAccessor, memoryCache);
			fileHandler = new FileRequestHandler(serviceContext, jsonSerializer, httpContextAccessor, memoryCache);
			fontsHandler = new FontsRequestHandler(serviceContext, jsonSerializer, httpContextAccessor, memoryCache);
			configurationHandler = new ConfigurationRequestHandler(serviceContext, jsonSerializer, httpContextAccessor, memoryCache);
			applicationModelHandler = new ApplicationModelRequestHandler(serviceContext, jsonSerializer, httpContextAccessor, memoryCache);
			handleHandler = new HandleRequestHandler(serviceContext, jsonSerializer, httpContextAccessor, memoryCache,
				actionFactory: resolution => resolution.HasOperation
					? new DoRequestHandler(serviceContext, jsonSerializer, httpContextAccessor, memoryCache, resolution)
					: new GetRequestHandler(serviceContext, jsonSerializer, httpContextAccessor, memoryCache, resolution)
			);
		}

		public async Task Invoke(HttpContext context)
		{
			var path = $"{context.Request.Path}".ToLowerInvariant();

			if (path == "/")
			{
				await indexHandler.WriteResponse();
			}
			else if (path == $"/{rootPath}file")
			{
				await fileHandler.WriteResponse();
			}
			else if (Regex.IsMatch(path, $"/{rootPath}fonts/[^/]*/f"))
			{
				await fontsHandler.WriteResponse();
			}
			else if (path == $"/{rootPath}configuration")
			{
				await configurationHandler.WriteResponse();
			}
			else if (path == $"/{rootPath}applicationmodel")
			{
				await applicationModelHandler.WriteResponse();
			}
			else if (path.StartsWith($"/{rootPath}"))
			{
				await handleHandler.WriteResponse();
			}
			else
			{
				await next(context);
			}
		}
	}
}