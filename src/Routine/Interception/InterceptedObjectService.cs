using System.Collections.Generic;
using Routine.Core;
using Routine.Interception.Context;
using System.Threading.Tasks;

namespace Routine.Interception
{
    public class InterceptedObjectService : IObjectService
    {
        private readonly IInterceptionConfiguration configuration;
        private readonly IObjectService real;

        private readonly IInterceptor<InterceptionContext> applicationModel;
        private readonly IInterceptor<InterceptionContext> get;
        private readonly IInterceptor<InterceptionContext> @do;

        public InterceptedObjectService(IObjectService real, IInterceptionConfiguration configuration)
        {
            this.real = real;
            this.configuration = configuration;

            applicationModel = configuration.GetInterceptor(InterceptionTarget.ApplicationModel);
            get = configuration.GetInterceptor(InterceptionTarget.Get);
            @do = configuration.GetInterceptor(InterceptionTarget.Do);
        }

        public ApplicationModel ApplicationModel => applicationModel.Intercept(NewContext(), () => real.ApplicationModel) as ApplicationModel;
        public ObjectData Get(ReferenceData reference) => get.Intercept(NewContext(reference), () => real.Get(reference)) as ObjectData;

        public VariableData Do(ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters)
        {
            var context = NewContext(target, operation, parameters);
            var service = GetService(context);

            return @do.Intercept(context,
                () => service.Intercept(context,
                    () => real.Do(context.TargetReference, context.OperationName, context.ParameterValues)
                )
            ) as VariableData;
        }

        public async Task<VariableData> DoAsync(ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters)
        {
            var context = NewContext(target, operation, parameters);
            var service = GetService(context);

            return await @do.InterceptAsync(context,
                async () => await service.InterceptAsync(context,
                    async () => await real.DoAsync(context.TargetReference, context.OperationName, context.ParameterValues)
                )
            ) as VariableData;
        }

        private InterceptionContext NewContext() => new($"{InterceptionTarget.ApplicationModel}");
        private ObjectReferenceInterceptionContext NewContext(ReferenceData reference) => new($"{InterceptionTarget.Get}", real, reference);
        private ServiceInterceptionContext NewContext(ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters) => new($"{InterceptionTarget.Do}", real, target, operation, parameters);

        private IInterceptor<ServiceInterceptionContext> GetService(ServiceInterceptionContext context) => configuration.GetServiceInterceptor(context.TargetModel, context.OperationModel);
    }
}
