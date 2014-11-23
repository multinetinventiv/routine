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
		public IApiGenerationContext AsApiGenerationLocal(IApiGenerationConfiguration apiGenerationConfiguration, ICodingStyle codingStyle)
		{
			return ApiGenerationContext(apiGenerationConfiguration, ApiContext(ObjectService(codingStyle)));
		}

		public IApiGenerationContext AsApiGenerationRemote(IApiGenerationConfiguration apiGenerationConfiguration, ISoaClientConfiguration soaClientConfiguration)
		{
			return ApiGenerationContext(apiGenerationConfiguration, ApiContext(ObjectServiceClient(soaClientConfiguration)));
		}

		public IMvcContext AsMvcApplication(IMvcConfiguration mvcConfiguration, ICodingStyle codingStyle)
		{
			return MvcContext(mvcConfiguration, ApiContext(ObjectService(codingStyle)));
		}

		public IMvcContext AsMvcSoaClient(IMvcConfiguration mvcConfiguration, ISoaClientConfiguration soaClientConfiguration)
		{
			return MvcContext(mvcConfiguration, ApiContext(ObjectServiceClient(soaClientConfiguration)));
		}

		public IApiContext AsSoaClient(ISoaClientConfiguration soaClientConfiguration)
		{
			return ApiContext(ObjectServiceClient(soaClientConfiguration));
		}

		public IApiContext AsClientApplication(ICodingStyle codingStyle)
		{
			return ApiContext(ObjectService(codingStyle));
		}

		public ISoaContext AsSoaApplication(ISoaConfiguration soaConfiguration, ICodingStyle codingStyle)
		{
			return SoaContext(soaConfiguration, codingStyle);
		}

		private IApiGenerationContext ApiGenerationContext(IApiGenerationConfiguration apiGenerationConfiguration, IApiContext apiContext)
		{
			return new DefaultApiGenerationContext(apiGenerationConfiguration, new ApplicationCodeModel(apiContext.Application, apiGenerationConfiguration));
		}

		private IMvcContext MvcContext(IMvcConfiguration mvcConfiguration, IApiContext apiContext)
		{
			return new DefaultMvcContext(mvcConfiguration, new ApplicationViewModel(apiContext.Application, mvcConfiguration));
		}

		private IApiContext ApiContext(IObjectService objectService)
		{
			return new DefaultApiContext(objectService, new Rapplication(objectService));
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
