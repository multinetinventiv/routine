using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Routine.Core.Rest;
using Routine.Service.RequestHandlers;
using System.Threading.Tasks;

namespace Routine.Service
{
	public class RoutineRouteHandler
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMemoryCache memoryCache;
        private readonly IServiceContext serviceContext;
        private readonly IJsonSerializer jsonSerializer;
        private readonly IApplicationBuilder applicationBuilder;
        
        public RoutineRouteHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, IApplicationBuilder applicationBuilder)
        {
            this.serviceContext = serviceContext;
            this.jsonSerializer = jsonSerializer;
            this.httpContextAccessor = httpContextAccessor;
            this.memoryCache = memoryCache;
            this.applicationBuilder = applicationBuilder;
        }

        // TODO: gecici olarak unit test compile etsin diye koydum, kaldirilacak
        public HandleRequestHandler HandleRequestHandler { get; private set; }

        public virtual void RegisterRoutes()
        {
	        var routeBuilder = new RouteBuilder(applicationBuilder);
            var rootPath = serviceContext.ServiceConfiguration.GetPath();

            var indexHandler = new IndexRequestHandler(serviceContext, jsonSerializer, httpContextAccessor, memoryCache);
            routeBuilder.MapGet("", 
	            context => Task.Run(() => indexHandler.WriteResponse())
            );

            var fileHandler = new FileRequestHandler(serviceContext, jsonSerializer, httpContextAccessor, memoryCache);
            routeBuilder.MapGet($"{rootPath}{{action}}/{{fileName}}/f", 
	            context => Task.Run(() => fileHandler.WriteResponse())
            );

            var fontsHandler = new FontsRequestHandler(serviceContext, jsonSerializer, httpContextAccessor, memoryCache, "fileName");
            routeBuilder.MapGet($"{rootPath}fonts}}", 
	            context => Task.Run(() => fontsHandler.WriteResponse())
            );

            var configurationHandler = new ConfigurationRequestHandler(serviceContext, jsonSerializer, httpContextAccessor, memoryCache);
            routeBuilder.MapGet($"{rootPath}configuration", 
	            context => Task.Run(() => configurationHandler.WriteResponse())
            );

            var applicationModelHandler = new ApplicationModelRequestHandler(serviceContext, jsonSerializer, httpContextAccessor, memoryCache);
            routeBuilder.MapGet($"{rootPath}applicationmodel", 
	            context => Task.Run(() => applicationModelHandler.WriteResponse())
            );

            var handleHandler = HandleRequestHandler = new HandleRequestHandler(serviceContext, jsonSerializer, httpContextAccessor, memoryCache,
	            modelIdRouteKey: "modelId",
	            idOrViewModelIdOrOperationRouteKey: "idOrViewModelIdOrOperation",
	            viewModelIdOrOperationRouteKey: "viewModelIdOrOperation",
	            operationRouteKey: "operation",
	            actionFactory: resolution => resolution.HasOperation
		            ? new DoRequestHandler(serviceContext, jsonSerializer, httpContextAccessor, memoryCache, resolution)
		            : new GetRequestHandler(serviceContext, jsonSerializer, httpContextAccessor, memoryCache, resolution)
            );
            routeBuilder.MapGet($"{rootPath}{{modelId}}/{{idOrViewModelIdOrOperation}}/{{viewModelIdOrOperation ?}}/{{operation ?}}", 
	            context => Task.Run(() => handleHandler.WriteResponse())
            );
            routeBuilder.MapPost($"{rootPath}{{modelId}}/{{idOrViewModelIdOrOperation}}/{{viewModelIdOrOperation ?}}/{{operation ?}}", 
	            context => Task.Run(() => handleHandler.WriteResponse())
	        );

            applicationBuilder.UseRouter(routeBuilder.Build());
        }
    }
}
