using System.Collections.Generic;
using Routine.Core;
using Routine.Interception.Context;

namespace Routine.Interception
{
	public class InterceptedObjectService : IObjectService
	{
		private readonly IInterceptionConfiguration interceptionConfiguration;
		private readonly IObjectService objectService;

		private readonly IInterceptor<InterceptionContext> getApplicationModelInterceptor;
		private readonly IInterceptor<InterceptionContext> getInterceptor;
		private readonly IInterceptor<InterceptionContext> doInterceptor;

		public InterceptedObjectService(IObjectService objectService, IInterceptionConfiguration interceptionConfiguration)
		{
			this.objectService = objectService;
			this.interceptionConfiguration = interceptionConfiguration;

			getApplicationModelInterceptor = interceptionConfiguration.GetInterceptor(InterceptionTarget.ApplicationModel);
			getInterceptor = interceptionConfiguration.GetInterceptor(InterceptionTarget.Get);
			doInterceptor = interceptionConfiguration.GetInterceptor(InterceptionTarget.Do);
		}

		public ApplicationModel ApplicationModel
		{
			get
			{
				var context = new InterceptionContext(InterceptionTarget.ApplicationModel.ToString());

                return getApplicationModelInterceptor.Intercept(
                    context,
                    () => objectService.ApplicationModel
                ) as ApplicationModel;	
			}
			
		}

		public ObjectData Get(ReferenceData reference)
		{
			var context = new ObjectReferenceInterceptionContext(InterceptionTarget.Get.ToString(), objectService, reference);

            return getInterceptor.Intercept(
                context,
                () => objectService.Get(reference)
            ) as ObjectData;
		}

		public VariableData Do(ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameterValues)
		{
			var context = new ServiceInterceptionContext(InterceptionTarget.Do.ToString(), objectService, target, operation, parameterValues);
			var objectModel = target.ViewModelId == null ? context.Model : context.ViewModel;

			var serviceInterceptor = interceptionConfiguration.GetServiceInterceptor(objectModel, context.OperationModel);

            return doInterceptor.Intercept(
                context,
                () => serviceInterceptor.Intercept(
                    context,
                    () => objectService.Do(context.TargetReference, context.OperationName, context.ParameterValues)
                )
            ) as VariableData;
		}
	}
}
