using Routine.Client.Context;
using Routine.Client;
using Routine.Core.Cache;
using Routine.Core.Rest;
using Routine.Engine.Context;
using Routine.Engine;
using Routine.Interception;
using Routine.Service.Context;
using Routine.Service;

namespace Routine;

public class ContextBuilder
{
    public IRestClient RestClient { get; private set; }
    public IJsonSerializer Serializer { get; private set; }
    public ICache Cache { get; private set; }
    public IInterceptionConfiguration InterceptionConfiguration { get; private set; }

    public ContextBuilder()
    {
        RestClient = new HttpClientRestClient();
        Cache = new DictionaryCache();
        Serializer = new JsonSerializerAdapter();

        InterceptionConfiguration = null;
    }

    public ContextBuilder Using(
        IRestClient restClient = null,
        IJsonSerializer serializer = null,
        ICache cache = null,
        IInterceptionConfiguration interceptionConfiguration = null
    )
    {
        RestClient = restClient ?? RestClient;
        Serializer = serializer ?? Serializer;
        Cache = cache ?? Cache;
        InterceptionConfiguration = interceptionConfiguration ?? InterceptionConfiguration;

        return this;
    }

    public IClientContext AsServiceClient(IServiceClientConfiguration serviceClientConfiguration)
    {
        var service = new RestClientObjectService(serviceClientConfiguration, RestClient, Serializer)
            .Intercept(InterceptionConfiguration);

        return new DefaultClientContext(service, new Rapplication(service));
    }

    public IClientContext AsClientApplication(ICodingStyle codingStyle)
    {
        var coreContext = new DefaultCoreContext(codingStyle);
        var service = new ObjectService(coreContext, Cache).Intercept(InterceptionConfiguration);

        return new DefaultClientContext(service, new Rapplication(service));
    }

    public IServiceContext AsServiceApplication(IServiceConfiguration serviceConfiguration, ICodingStyle codingStyle)
    {
        var coreContext = new DefaultCoreContext(codingStyle);
        var service = new ObjectService(coreContext, Cache).Intercept(InterceptionConfiguration);

        return new DefaultServiceContext(coreContext, serviceConfiguration, service);
    }
}
