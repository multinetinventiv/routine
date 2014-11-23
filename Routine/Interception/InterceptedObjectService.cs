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
		private readonly IInterceptor<InterceptionContext> getObjectModelInterceptor;
		private readonly IInterceptor<InterceptionContext> getValueInterceptor;
		private readonly IInterceptor<InterceptionContext> getInterceptor;
		private readonly IInterceptor<InterceptionContext> performOperationInterceptor;

		public InterceptedObjectService(IObjectService objectService, IInterceptionConfiguration interceptionConfiguration)
		{
			this.objectService = objectService;
			this.interceptionConfiguration = interceptionConfiguration;

			getApplicationModelInterceptor = interceptionConfiguration.GetInterceptor(InterceptionTarget.GetApplicationModel);
			getObjectModelInterceptor = interceptionConfiguration.GetInterceptor(InterceptionTarget.GetObjectModel);
			getValueInterceptor = interceptionConfiguration.GetInterceptor(InterceptionTarget.GetValue);
			getInterceptor = interceptionConfiguration.GetInterceptor(InterceptionTarget.Get);
			performOperationInterceptor = interceptionConfiguration.GetInterceptor(InterceptionTarget.PerformOperation);
		}

		public ApplicationModel GetApplicationModel()
		{
			var context = new InterceptionContext(InterceptionTarget.GetApplicationModel.ToString());

			var result = getApplicationModelInterceptor.Intercept(
				context, 
				() => objectService.GetApplicationModel()
			) as ApplicationModel;

			return result;
		}

		public ObjectModel GetObjectModel(string objectModelId)
		{
			var context = new ObjectModelInterceptionContext(InterceptionTarget.GetObjectModel.ToString(), objectService, objectModelId);

			var result = getObjectModelInterceptor.Intercept(
				context, 
				() => objectService.GetObjectModel(context.ObjectModelId)
			) as ObjectModel;

			return result;
		}

		public string GetValue(ObjectReferenceData reference)
		{
			var context = new ObjectReferenceInterceptionContext(InterceptionTarget.GetValue.ToString(), objectService, reference);
			
			var result = getValueInterceptor.Intercept(
				context, 
				() => objectService.GetValue(context.TargetReference)
			) as string;

			return result;
		}

		public ObjectData Get(ObjectReferenceData reference)
		{
			var context = new ObjectReferenceInterceptionContext(InterceptionTarget.Get.ToString(), objectService, reference);
			
			var result = getInterceptor.Intercept(
				context, 
				() => objectService.Get(reference)
			) as ObjectData;

			return result;
		}

		public ValueData PerformOperation(ObjectReferenceData targetReference, string operationModelId, Dictionary<string, ParameterValueData> parameterValues)
		{
			var context = new ServiceInterceptionContext(InterceptionTarget.PerformOperation.ToString(), objectService, targetReference, operationModelId, parameterValues);
			var objectModel = targetReference.ViewModelId ==null ? context.GetActualModel() : context.GetViewModel();

			var serviceInterceptor = interceptionConfiguration.GetServiceInterceptor(objectModel, context.GetOperationModel());

			var result = performOperationInterceptor.Intercept(
				context, 
				() => serviceInterceptor.Intercept(
					context, 
					() => objectService.PerformOperation(context.TargetReference, context.OperationModelId, context.ParameterValues)
				)
			) as ValueData;

			return result;
		}
	}
}
