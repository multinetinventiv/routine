
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Routine;

public static class RoutineApplicationBuilderExtensions
{
    public static void UseRoutine(this IApplicationBuilder source, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, Action<ContextBuilder> action)
    {
        action(BuildRoutine.Context(source, httpContextAccessor, memoryCache));
    }
}