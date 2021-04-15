
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Routine;

public static class RoutineApplicationBuilderExtensions
    {
        public static void UseRoutine(this IApplicationBuilder source, IHttpContextAccessor httpContextAccessor, Action<ContextBuilder> action)
        {
            action(BuildRoutine.Context(source, httpContextAccessor));
        }
    }