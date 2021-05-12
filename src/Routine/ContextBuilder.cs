using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Routine.Api;
using Routine.Api.Context;
using Routine.Client;
using Routine.Client.Context;
using Routine.Core;
using Routine.Core.Cache;
using Routine.Core.Rest;
using Routine.Engine;
using Routine.Engine.Context;
using Routine.Interception;
using Routine.Service;
using Routine.Service.Context;

namespace Routine
{
    public class ContextBuilder
    {
        public ContextBuilder(IApplicationBuilder applicationBuilder, IHttpContextAccessor httpContextAccessor)
        {
            this.applicationBuilder = applicationBuilder;
            this.httpContextAccessor = httpContextAccessor;
        }
        public IApiContext AsApiGenerationLocal(IApiConfiguration apiConfiguration, ICodingStyle codingStyle)
        {
            return ApiContext(apiConfiguration, ClientContext(ObjectService(codingStyle)));
        }

        public IApiContext AsApiGenerationRemote(IApiConfiguration apiConfiguration, IServiceClientConfiguration serviceClientConfiguration)
        {
            return ApiContext(apiConfiguration, ClientContext(ObjectServiceClient(serviceClientConfiguration)));
        }

        public IClientContext AsServiceClient(IServiceClientConfiguration serviceClientConfiguration)
        {
            return ClientContext(ObjectServiceClient(serviceClientConfiguration));
        }

        public IClientContext AsClientApplication(ICodingStyle codingStyle)
        {
            return ClientContext(ObjectService(codingStyle));
        }

        public IServiceContext AsServiceApplication(IServiceConfiguration serviceConfiguration, ICodingStyle codingStyle)
        {
            return ServiceContext(serviceConfiguration, codingStyle);
        }

        private IApiContext ApiContext(IApiConfiguration apiConfiguration, IClientContext clientContext)
        {
            return new DefaultApiContext(apiConfiguration, new ApplicationCodeModel(clientContext.Application, apiConfiguration));
        }

        private IClientContext ClientContext(IObjectService objectService)
        {
            return new DefaultClientContext(objectService, new Rapplication(objectService));
        }

        private IServiceContext ServiceContext(IServiceConfiguration serviceConfiguration, ICodingStyle codingStyle)
        {
            return new DefaultServiceContext(CoreContext(codingStyle), serviceConfiguration, ObjectService(codingStyle), HandlerFactory(), applicationBuilder);
        }

        private IObjectService ObjectService(ICodingStyle codingStyle)
        {
            return InterceptIfConfigured(new ObjectService(CoreContext(codingStyle), Cache()));
        }

        private IObjectService ObjectServiceClient(IServiceClientConfiguration serviceClientConfiguration)
        {
            return InterceptIfConfigured(new RestClientObjectService(serviceClientConfiguration, RestClient(), Serializer()));
        }

        private IObjectService InterceptIfConfigured(IObjectService real)
        {
            if (InterceptionConfiguration() == null) { return real; }

            return new InterceptedObjectService(real, InterceptionConfiguration());
        }

        private ICoreContext coreContext;
        private ICoreContext CoreContext(ICodingStyle codingStyle)
        {
            if (coreContext == null)
            {
                coreContext = new DefaultCoreContext(codingStyle, Cache());
            }

            return coreContext;
        }


        private Func<IServiceContext, IRoutineRouteHandler> handlerFactory;
        public ContextBuilder UsingHandlerFactory(Func<IServiceContext, IRoutineRouteHandler> handlerFactory) { this.handlerFactory = handlerFactory; return this; }
        private Func<IServiceContext, IRoutineRouteHandler> HandlerFactory() => handlerFactory ?? (sc => new RoutineRouteHandler(sc, Serializer(), httpContextAccessor));

        private IRestClient restClient = new WebRequestRestClient();
        public ContextBuilder UsingRestClient(IRestClient restClient) { this.restClient = restClient; return this; }
        private IRestClient RestClient() { return restClient; }


        private readonly IApplicationBuilder applicationBuilder;
        private readonly IHttpContextAccessor httpContextAccessor;
        private IJsonSerializer serializer = new JsonSerializerAdapter();
        public ContextBuilder UsingSerializer(IJsonSerializer serializer) { this.serializer = serializer; return this; }
        private IJsonSerializer Serializer() { return serializer; }

        private ICache cache = new WebCache();
        public ContextBuilder UsingCache(ICache cache) { this.cache = cache; return this; }
        private ICache Cache() { return cache; }

        private IInterceptionConfiguration interceptionConfiguration;
        public ContextBuilder UsingNoInterception() { return UsingInterception(null); }
        public ContextBuilder UsingInterception(IInterceptionConfiguration interceptionConfiguration) { this.interceptionConfiguration = interceptionConfiguration; return this; }
        private IInterceptionConfiguration InterceptionConfiguration() { return interceptionConfiguration; }
    }
}
