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
        private IInterceptionConfiguration interceptionConfiguration;
        private ICache cache;
        private IJsonSerializer serializer;
        private IRestClient restClient;

        public ContextBuilder Using(
            IRestClient restClient = null,
            IJsonSerializer serializer = null,
            ICache cache = null,
            IInterceptionConfiguration interceptionConfiguration = null
        )
        {
            this.restClient = restClient ?? new WebRequestRestClient();
            this.serializer = serializer ?? new JsonSerializerAdapter();
            this.cache = cache ?? new DictionaryCache();

            this.interceptionConfiguration = interceptionConfiguration;

            return this;
        }

        public IClientContext AsServiceClient(IServiceClientConfiguration serviceClientConfiguration)
        {
            var service = InterceptIfConfigured(new RestClientObjectService(serviceClientConfiguration, restClient, serializer));

            return new DefaultClientContext(service, new Rapplication(service));
        }

        public IClientContext AsClientApplication(ICodingStyle codingStyle)
        {
            var coreContext = new DefaultCoreContext(codingStyle, cache);
            var service = InterceptIfConfigured(new ObjectService(coreContext, cache));

            return new DefaultClientContext(service, new Rapplication(service));
        }

        public IServiceContext AsServiceApplication(IServiceConfiguration serviceConfiguration, ICodingStyle codingStyle)
        {
            var coreContext = new DefaultCoreContext(codingStyle, cache);
            var service = InterceptIfConfigured(new ObjectService(coreContext, cache));

            return new DefaultServiceContext(coreContext, serviceConfiguration, service);
        }

        private IObjectService InterceptIfConfigured(IObjectService real) => interceptionConfiguration == null ? real : new InterceptedObjectService(real, interceptionConfiguration);
    }
}
