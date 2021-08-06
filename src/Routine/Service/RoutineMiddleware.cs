using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Routine.Core.Rest;

namespace Routine.Service
{
	public class RoutineMiddleware
	{
		private readonly RequestDelegate next;

		public RoutineMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, IJsonSerializer serializer, IServiceContext serviceContext, IApplicationBuilder applicationBuilder)
		{
			this.next = next;

			new RoutineRouteHandler(serviceContext, serializer, httpContextAccessor, memoryCache)
				.RegisterRoutes(applicationBuilder)
			;
		}

		public async Task Invoke(HttpContext context) => await next(context);
	}
}