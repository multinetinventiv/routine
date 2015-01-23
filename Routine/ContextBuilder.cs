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
using Routine.Soa;
using Routine.Soa.Context;
using Routine.Ui;
using Routine.Ui.Context;

namespace Routine
{
	public class ContextBuilder
	{
		public IApiContext AsApiGenerationLocal(IApiConfiguration apiConfiguration, ICodingStyle codingStyle)
		{
			return ApiContext(apiConfiguration, ClientContext(ObjectService(codingStyle)));
		}

		public IApiContext AsApiGenerationRemote(IApiConfiguration apiConfiguration, ISoaClientConfiguration soaClientConfiguration)
		{
			return ApiContext(apiConfiguration, ClientContext(ObjectServiceClient(soaClientConfiguration)));
		}

		public IMvcContext AsMvcApplication(IMvcConfiguration mvcConfiguration, ICodingStyle codingStyle)
		{
			return MvcContext(mvcConfiguration, ClientContext(ObjectService(codingStyle)));
		}

		public IMvcContext AsMvcSoaClient(IMvcConfiguration mvcConfiguration, ISoaClientConfiguration soaClientConfiguration)
		{
			return MvcContext(mvcConfiguration, ClientContext(ObjectServiceClient(soaClientConfiguration)));
		}

		public IClientContext AsSoaClient(ISoaClientConfiguration soaClientConfiguration)
		{
			return ClientContext(ObjectServiceClient(soaClientConfiguration));
		}

		public IClientContext AsClientApplication(ICodingStyle codingStyle)
		{
			return ClientContext(ObjectService(codingStyle));
		}

		public ISoaContext AsSoaApplication(ISoaConfiguration soaConfiguration, ICodingStyle codingStyle)
		{
			return SoaContext(soaConfiguration, codingStyle);
		}

		private IApiContext ApiContext(IApiConfiguration apiConfiguration, IClientContext clientContext)
		{
			return new DefaultApiContext(apiConfiguration, new ApplicationCodeModel(clientContext.Application, apiConfiguration));
		}

		private IMvcContext MvcContext(IMvcConfiguration mvcConfiguration, IClientContext clientContext)
		{
			return new DefaultMvcContext(mvcConfiguration, new ApplicationViewModel(clientContext.Application, mvcConfiguration));
		}

		private IClientContext ClientContext(IObjectService objectService)
		{
			return new DefaultClientContext(objectService, new Rapplication(objectService));
		}

		private ISoaContext SoaContext(ISoaConfiguration soaConfiguration, ICodingStyle codingStyle)
		{
			return new DefaultSoaContext(CoreContext(codingStyle), soaConfiguration, ObjectService(codingStyle));
		}

		private IObjectService ObjectService(ICodingStyle codingStyle)
		{
			return InterceptIfConfigured(new ObjectService(CoreContext(codingStyle), Cache()));
		}

		private IObjectService ObjectServiceClient(ISoaClientConfiguration soaClientConfiguration)
		{
			return InterceptIfConfigured(new RestClientObjectService(soaClientConfiguration, RestClient(), Serializer()));
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

		private IRestClient restClient = new WebRequestRestClient();
		public ContextBuilder UsingRestClient(IRestClient restClient) { this.restClient = restClient; return this; }
		private IRestClient RestClient() { return restClient; }

		private IRestSerializer serializer = new JsonRestSerializer();
		public ContextBuilder UsingSerializer(IRestSerializer serializer) { this.serializer = serializer; return this; }
		private IRestSerializer Serializer() { return serializer; }

		private ICache cache = new WebCache();
		public ContextBuilder UsingCache(ICache cache) { this.cache = cache; return this; }
		private ICache Cache() { return cache; }

		private IInterceptionConfiguration interceptionConfiguration;
		public ContextBuilder UsingInterception(IInterceptionConfiguration interceptionConfiguration) { this.interceptionConfiguration = interceptionConfiguration; return this; }
		public ContextBuilder UsingNoInterception() { return UsingInterception(null); }
		private IInterceptionConfiguration InterceptionConfiguration() { return interceptionConfiguration; }
	}
}
