using Microsoft.Extensions.DependencyInjection;
using Routine.Core.Cache;
using Routine.Core.Reflection;
using Routine.Core.Rest;
using Routine.Engine.Configuration;
using Routine.Engine;
using Routine.Interception.Configuration;
using Routine.Interception;
using Routine.Service.Configuration;
using Routine.Service;
using Routine;
using System;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder;

public static class AspNetCoreExtensions
{
    public static IServiceCollection AddRoutine(this IServiceCollection source) => source.AddRoutine<JsonSerializerAdapter>();
    public static IServiceCollection AddRoutine<TJsonSerializer>(this IServiceCollection source) where TJsonSerializer : class, IJsonSerializer =>
        source
            .AddHttpContextAccessor()
            .AddSingleton<IJsonSerializer, TJsonSerializer>();

    public static IApplicationBuilder UseRoutine(this IApplicationBuilder source,
        Func<CodingStyleBuilder, ICodingStyle> codingStyle,
        Func<ServiceConfigurationBuilder, IServiceConfiguration> serviceConfiguration = null,
        IRestClient restClient = null,
        IJsonSerializer serializer = null,
        ICache cache = null,
        Func<InterceptionConfigurationBuilder, IInterceptionConfiguration> interceptionConfiguration = null
    )
    {
        serviceConfiguration ??= s => s.FromBasic();
        interceptionConfiguration ??= i => i.FromBasic();

        return source.UseMiddleware<RoutineMiddleware>(
            BuildRoutine.Context()
                .Using(
                    restClient: restClient,
                    serializer: serializer,
                    cache: cache,
                    interceptionConfiguration: interceptionConfiguration(BuildRoutine.InterceptionConfig())
                )
                .AsServiceApplication(serviceConfiguration, codingStyle)
        );
    }

    public static IApplicationBuilder UseRoutineInDevelopmentMode(this IApplicationBuilder source)
    {
        ReflectionOptimizer.Disable();

        return source;
    }

    public static IApplicationBuilder UseRoutineInProductionMode(this IApplicationBuilder source)
    {
        ReflectionOptimizer.Enable();

        return source;
    }
}
