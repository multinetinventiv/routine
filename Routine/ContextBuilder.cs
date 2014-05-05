using Routine.Api;
using Routine.Api.Context;
using Routine.Core;
using Routine.Core.Api;
using Routine.Core.Cache;
using Routine.Core.Context;
using Routine.Core.Rest;
using Routine.Mvc;
using Routine.Mvc.Context;
using Routine.Soa;
using Routine.Soa.Context;

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
			var result = new DefaultApiGenerationContext(apiGenerationConfiguration);

			result.Application = new ApplicationCodeModel(apiContext.Rapplication, result);

			return result;
		}

		private IMvcContext MvcContext(IMvcConfiguration mvcConfiguration, IApiContext apiContext)
		{
			var result = new DefaultMvcContext(mvcConfiguration);

			result.Application = new ApplicationViewModel(apiContext.Rapplication, result);

			return result;
		}

		private IApiContext ApiContext(IObjectService objectService)
		{
			var result = new DefaultApiContext(objectService);

			result.Rapplication = new Rapplication(result);

			return result;
		}

		private ISoaContext SoaContext(ISoaConfiguration soaConfiguration, ICodingStyle codingStyle)
		{
			return new DefaultSoaContext(CoreContext(codingStyle), soaConfiguration, ObjectService(codingStyle));
		}

		private IObjectService ObjectService(ICodingStyle codingStyle)
		{
			return new ObjectService(CoreContext(codingStyle), Cache());
		}

		private IObjectService ObjectServiceClient(ISoaClientConfiguration soaClientConfiguration)
		{
			return new RestClientObjectService(soaClientConfiguration, RestClient(), Serializer());
		}

		private ICoreContext coreContext;
		private ICoreContext CoreContext(ICodingStyle codingStyle)
		{
			if (coreContext == null)
			{
				coreContext = new CachedCoreContext(codingStyle, Cache());
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
	}
}
