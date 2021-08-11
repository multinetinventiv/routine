﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Routine.Core.Rest;
using System.Threading.Tasks;

namespace Routine.Service.RequestHandlers
{
    public class ApplicationModelRequestHandler : RequestHandlerBase
	{
		public ApplicationModelRequestHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache)
			: base(serviceContext, jsonSerializer, httpContextAccessor, memoryCache) { }

		public override async Task WriteResponse()
		{
			await WriteJsonResponse(ServiceContext.ObjectService.ApplicationModel);
		}
	}
}