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
        public IClientContext AsServiceClient(IServiceClientConfiguration serviceClientConfiguration) => ClientContext(ObjectServiceClient(serviceClientConfiguration));
        public IClientContext AsClientApplication(ICodingStyle codingStyle) => ClientContext(ObjectService(codingStyle));
        public IServiceContext AsServiceApplication(IServiceConfiguration serviceConfiguration, ICodingStyle codingStyle) => ServiceContext(serviceConfiguration, codingStyle);
        
        private IClientContext ClientContext(IObjectService objectService) => new DefaultClientContext(objectService, new Rapplication(objectService));
        private IServiceContext ServiceContext(IServiceConfiguration serviceConfiguration, ICodingStyle codingStyle) => new DefaultServiceContext(CoreContext(codingStyle), serviceConfiguration, ObjectService(codingStyle));
        
        private IObjectService ObjectService(ICodingStyle codingStyle) => InterceptIfConfigured(new ObjectService(CoreContext(codingStyle), Cache()));
        private IObjectService ObjectServiceClient(IServiceClientConfiguration serviceClientConfiguration) => InterceptIfConfigured(new RestClientObjectService(serviceClientConfiguration, RestClient(), Serializer()));
        private IObjectService InterceptIfConfigured(IObjectService real) => InterceptionConfiguration() == null ? real : new InterceptedObjectService(real, InterceptionConfiguration());

        private ICoreContext coreContext;
        private ICoreContext CoreContext(ICodingStyle codingStyle) => coreContext ??= new DefaultCoreContext(codingStyle, Cache());

        private IRestClient restClient = new WebRequestRestClient();
        public ContextBuilder UsingRestClient(IRestClient restClient) { this.restClient = restClient; return this; }
        private IRestClient RestClient() => restClient;

        private IJsonSerializer serializer = new JsonSerializerAdapter();
        public ContextBuilder UsingSerializer(IJsonSerializer serializer) { this.serializer = serializer; return this; }
        private IJsonSerializer Serializer() => serializer;

        private ICache cache = new DictionaryCache();
        public ContextBuilder UsingCache(ICache cache) { this.cache = cache; return this; }
        private ICache Cache() => cache;

        private IInterceptionConfiguration interceptionConfiguration;
        public ContextBuilder UsingNoInterception() => UsingInterception(null);
        public ContextBuilder UsingInterception(IInterceptionConfiguration interceptionConfiguration) { this.interceptionConfiguration = interceptionConfiguration; return this; }
        private IInterceptionConfiguration InterceptionConfiguration() => interceptionConfiguration;
    }
}
