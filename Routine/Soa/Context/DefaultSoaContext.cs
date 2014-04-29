using System.Collections.Generic;
using Routine.Core;
using Routine.Core.Service;

namespace Routine.Soa.Context
{
	public class DefaultSoaContext : ISoaContext
	{
		private readonly ICoreContext coreContext;

		public ISoaConfiguration SoaConfiguration { get; private set; }
		public IObjectService ObjectService { get; private set; }

		public DefaultSoaContext(ICoreContext coreContext, ISoaConfiguration soaConfiguration, IObjectService objectService)
		{
			this.coreContext = coreContext;

			SoaConfiguration = soaConfiguration;
			ObjectService = objectService;
		}

		public InterceptionContext CreateInterceptionContext()
		{
			return new InterceptionContext();
		}

		public ObjectModelInterceptionContext CreateObjectModelInterceptionContext(string objectModelId)
		{
			return new ObjectModelInterceptionContext(coreContext, objectModelId);
		}

		public ObjectReferenceInterceptionContext CreateObjectReferenceInterceptionContext(ObjectReferenceData targetReference)
		{
			return new ObjectReferenceInterceptionContext(coreContext, targetReference);
		}

		public MemberInterceptionContext CreateMemberInterceptionContext(ObjectReferenceData targetReference, string memberModelId)
		{
			return new MemberInterceptionContext(coreContext, targetReference, memberModelId);
		}

		public OperationInterceptionContext CreateOperationInterceptionContext(ObjectReferenceData targetReference, string operationModelId)
		{
			return new OperationInterceptionContext(coreContext, targetReference, operationModelId);
		}

		public PerformOperationInterceptionContext CreatePerformOperationInterceptionContext(ObjectReferenceData targetReference, string operationModelId, Dictionary<string, ReferenceData> parameterValues)
		{
			return new PerformOperationInterceptionContext(coreContext, targetReference, operationModelId, parameterValues);
		}

		public ObjectReferenceData GetObjectReference(object @object)
		{
			return coreContext.CreateReferenceData(@object);
		}

		public object GetObject(ObjectReferenceData reference)
		{
			return coreContext.Locate(reference);
		}
	}
}
