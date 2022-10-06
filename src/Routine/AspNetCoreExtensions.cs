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

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder;

public static class AspNetCoreExtensions
{
    public static IServiceCollection AddRoutine(this IServiceCollection source, Action<RoutineOptions> options = default)
    {
        options ??= _ => { };
        var o = new RoutineOptions();
        options(o);

        if (o.DevelopmentMode)
        {
            ReflectionOptimizer.Disable();
        }
        else
        {
            ReflectionOptimizer.Enable();
        }

        return source;
    }

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

        var contextBuilder = BuildRoutine.Context().Using(
            restClient: restClient,
            serializer: serializer,
            cache: cache,
            interceptionConfiguration: interceptionConfiguration(BuildRoutine.InterceptionConfig())
        );

        return source.UseMiddleware<RoutineMiddleware>(
            contextBuilder.Serializer,
            contextBuilder.AsServiceApplication(serviceConfiguration, codingStyle)
        );
    }
}
