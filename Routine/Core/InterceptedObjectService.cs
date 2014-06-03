using System.Collections.Generic;

namespace Routine.Core
{
	public class InterceptedObjectService : IObjectService
	{
		private readonly IInterceptionContext context;

		public InterceptedObjectService(IInterceptionContext context)
		{
			this.context = context;
		}

		public ApplicationModel GetApplicationModel()
		{
			return context.InterceptionConfiguration.GetApplicationModelInterceptor.Intercept(
					context.CreateInterceptionContext(),
					() => context.ObjectService.GetApplicationModel()) as ApplicationModel;
		}

		public ObjectModel GetObjectModel(string objectModelId)
		{
			return context.InterceptionConfiguration.GetObjectModelInterceptor.Intercept(
					context.CreateObjectModelInterceptionContext(objectModelId),
					() => context.ObjectService.GetObjectModel(objectModelId)) as ObjectModel;
		}

		public List<ObjectData> GetAvailableObjects(string objectModelId)
		{
			return context.InterceptionConfiguration.GetAvailableObjectsInterceptor.Intercept(
					context.CreateObjectModelInterceptionContext(objectModelId),
					() => context.ObjectService.GetAvailableObjects(objectModelId)) as List<ObjectData>;
		}

		public string GetValue(ObjectReferenceData reference)
		{
			return context.InterceptionConfiguration.GetValueInterceptor.Intercept(
					context.CreateObjectReferenceInterceptionContext(reference),
					() => context.ObjectService.GetValue(reference)) as string;
		}

		public ObjectData Get(ObjectReferenceData reference)
		{
			return context.InterceptionConfiguration.GetInterceptor.Intercept(
					context.CreateObjectReferenceInterceptionContext(reference),
					() => context.ObjectService.Get(reference)) as ObjectData;
		}

		public ValueData PerformOperation(ObjectReferenceData targetReference, string operationModelId, Dictionary<string, ParameterValueData> parameterValues)
		{
			return context.InterceptionConfiguration.PerformOperationInterceptor.Intercept(
					context.CreatePerformOperationInterceptionContext(targetReference, operationModelId, parameterValues),
					() => context.ObjectService.PerformOperation(targetReference, operationModelId, parameterValues)) as ValueData;
		}
	}
}
