using Routine.Core;
using Routine.Interception.Context;

namespace Routine.Interception;

public class InterceptedObjectService : IObjectService
{
    private readonly IInterceptionConfiguration _configuration;
    private readonly IObjectService _real;

    private readonly IInterceptor<InterceptionContext> _applicationModel;
    private readonly IInterceptor<InterceptionContext> _get;
    private readonly IInterceptor<InterceptionContext> @_do;

    public InterceptedObjectService(IObjectService real, IInterceptionConfiguration configuration)
    {
        _real = real;
        _configuration = configuration;

        _applicationModel = configuration.GetInterceptor(InterceptionTarget.ApplicationModel);
        _get = configuration.GetInterceptor(InterceptionTarget.Get);
        @_do = configuration.GetInterceptor(InterceptionTarget.Do);
    }

    public ApplicationModel ApplicationModel => _applicationModel.Intercept(NewContext(), () => _real.ApplicationModel) as ApplicationModel;

    public ObjectData Get(ReferenceData reference) => _get.Intercept(NewContext(reference), () => _real.Get(reference)) as ObjectData;
    public async Task<ObjectData> GetAsync(ReferenceData reference) => await _get.InterceptAsync(NewContext(reference), async () => await _real.GetAsync(reference)) as ObjectData;

    public VariableData Do(ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters)
    {
        var context = NewContext(target, operation, parameters);
        var service = GetService(context);

        return @_do.Intercept(context,
            () => service.Intercept(context,
                () => _real.Do(context.TargetReference, context.OperationName, context.ParameterValues)
            )
        ) as VariableData;
    }

    public async Task<VariableData> DoAsync(ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters)
    {
        var context = NewContext(target, operation, parameters);
        var service = GetService(context);

        return await @_do.InterceptAsync(context,
            async () => await service.InterceptAsync(context,
                async () => await _real.DoAsync(context.TargetReference, context.OperationName, context.ParameterValues)
            )
        ) as VariableData;
    }

    private InterceptionContext NewContext() => new($"{InterceptionTarget.ApplicationModel}");
    private ObjectReferenceInterceptionContext NewContext(ReferenceData reference) => new($"{InterceptionTarget.Get}", _real, reference);
    private ServiceInterceptionContext NewContext(ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters) => new($"{InterceptionTarget.Do}", _real, target, operation, parameters);

    private IInterceptor<ServiceInterceptionContext> GetService(ServiceInterceptionContext context) => _configuration.GetServiceInterceptor(context.TargetModel, context.OperationModel);
}
