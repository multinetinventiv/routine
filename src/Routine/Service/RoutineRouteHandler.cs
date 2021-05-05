using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Routine.Core.Rest;
using Routine.Service.RequestHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.Service
{
    public class RoutineRouteHandler : IRoutineRouteHandler
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IServiceContext serviceContext;
        private readonly IJsonSerializer jsonSerializer;


        public Dictionary<string, Func<IHttpContextAccessor, IRequestHandler>> RequestHandlers { get; }

        public List<string> RequestHandlersList => RequestHandlers.Keys.Select(q => q.ToLowerInvariant()).ToList();

        public RoutineRouteHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, IHttpContextAccessor httpContextAccessor)
        {
            this.serviceContext = serviceContext;
            this.jsonSerializer = jsonSerializer;
            this.httpContextAccessor = httpContextAccessor;
            RequestHandlers = new Dictionary<string, Func<IHttpContextAccessor, IRequestHandler>>();
        }

        public virtual void RegisterRoutes(IApplicationBuilder applicationBuilder)
        {
            Add(hcb => new IndexRequestHandler(serviceContext, jsonSerializer, hcb), applicationBuilder);
            Add(hcb => new FileRequestHandler(serviceContext, jsonSerializer, hcb), applicationBuilder);
            Add(applicationBuilder: applicationBuilder, path: "{action}/{fileName}/f", factory: hcb => new FontsRequestHandler(serviceContext, jsonSerializer, hcb, "fileName"));
            Add(hcb => new ConfigurationRequestHandler(serviceContext, jsonSerializer, hcb), applicationBuilder);
            Add(hcb => new ApplicationModelRequestHandler(serviceContext, jsonSerializer, hcb), applicationBuilder);
            Add(applicationBuilder: applicationBuilder,
                path: "{modelId}/{idOrViewModelIdOrOperation}/{viewModelIdOrOperation}/{operation}",
                defaults: new { idOrViewModelIdOrOperation = string.Empty, viewModelIdOrOperation = string.Empty, operation = string.Empty },
                factory: hcb =>
                    new HandleRequestHandler(serviceContext, jsonSerializer, hcb,
                        modelIdRouteKey: "modelId",
                        idOrViewModelIdOrOperationRouteKey: "idOrViewModelIdOrOperation",
                        viewModelIdOrOperationRouteKey: "viewModelIdOrOperation",
                        operationRouteKey: "operation",
                        actionFactory: resolution => resolution.HasOperation
                            ? new DoRequestHandler(serviceContext, jsonSerializer, hcb, resolution) as IRequestHandler
                            : new GetRequestHandler(serviceContext, jsonSerializer, hcb, resolution)
                    )
            );


            var rootBuilder = new RouteBuilder(applicationBuilder);

            rootBuilder.MapGet("", (context) =>
            {
                var requestHandler = RequestHandlers["index"](httpContextAccessor);
                return Task.Run(() => requestHandler.WriteResponse());
            }
            );

            rootBuilder.MapGet("api/{action}/{fileName}/f", (context) =>
            {
                var requestHandler = RequestHandlers["fonts"](httpContextAccessor);
                return Task.Run(() => requestHandler.WriteResponse());
            }
            );

            rootBuilder.MapPost("api/{modelId}/{idOrViewModelIdOrOperation}", (context) =>
            {
                var requestHandler = RequestHandlers["handle"](httpContextAccessor);
                return Task.Run(() => requestHandler.WriteResponse());
            }
            );

            rootBuilder.MapPost("api/{modelId}/{idOrViewModelIdOrOperation}/{viewModelIdOrOperation}", (context) =>
            {
                var requestHandler = RequestHandlers["handle"](httpContextAccessor);
                return Task.Run(() => requestHandler.WriteResponse());
            }
            );

            rootBuilder.MapPost("api/{modelId}/{idOrViewModelIdOrOperation}/{viewModelIdOrOperation}/{operation}", (context) =>
            {
                var requestHandler = RequestHandlers["handle"](httpContextAccessor);
                return Task.Run(() => requestHandler.WriteResponse());
            }
            );

            // rootBuilder.MapGet("api/{action}", (context) =>
            // {
            //     var routeData = context.GetRouteData();

            //     var requestHandler = RequestHandlers[routeData.Values["action"].ToString().ToLowerInvariant()](httpContextAccessor);
            //     return Task.Run(() => requestHandler.WriteResponse());
            // }
            // );

            rootBuilder.MapGet("api/index", (context) =>
            {
                var requestHandler = RequestHandlers["index"](httpContextAccessor);
                return Task.Run(() => requestHandler.WriteResponse());
            }
            );
            rootBuilder.MapGet("api/file", (context) =>
            {
                var requestHandler = RequestHandlers["file"](httpContextAccessor);
                return Task.Run(() => requestHandler.WriteResponse());
            }
            );
            rootBuilder.MapGet("api/configuration", (context) =>
            {
                var requestHandler = RequestHandlers["configuration"](httpContextAccessor);
                return Task.Run(() => requestHandler.WriteResponse());
            }
            );
            rootBuilder.MapGet("api/applicationmodel", (context) =>
            {
                var requestHandler = RequestHandlers["applicationmodel"](httpContextAccessor);
                return Task.Run(() => requestHandler.WriteResponse());
            }
            );

            applicationBuilder.UseRouter(rootBuilder.Build());
        }


        private void Add<T>(Func<IHttpContextAccessor, T> factory, IApplicationBuilder applicationBuilder, object defaults = null)
            where T : IRequestHandler
        {
            Add(
                factory: factory,
                applicationBuilder: applicationBuilder,
                path: ActionNameFor<T>(),
                defaults: defaults
            );
        }
        private void Add<T>(Func<IHttpContextAccessor, T> factory, IApplicationBuilder applicationBuilder, string path, object defaults = null) where T : IRequestHandler
        {
            Add(
                factory: hc => factory(hc),
                applicationBuilder: applicationBuilder,
                path: path,
                action: ActionNameFor<T>(),
                defaults: defaults,
                index: typeof(IIndexRequestHandler).IsAssignableFrom(typeof(T))
            );
        }
        private void Add(
            Func<IHttpContextAccessor, IRequestHandler> factory,
            IApplicationBuilder applicationBuilder,
            string path,
            string action,
            object defaults = null,
            bool index = false
        )
        {
            if (defaults == null) { defaults = new { }; }

            RequestHandlers[action] = factory;


        }

        private string ActionNameFor<T>() where T : IRequestHandler
        {
            return typeof(T).Name.BeforeLast("RequestHandler").ToLowerInvariant();
        }

        public RequestDelegate GetRequestHandler(HttpContext httpContext, RouteData routeData)
        {
            throw new NotImplementedException();
        }
    }
}
