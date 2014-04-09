using System;
using System.Web.Mvc;
using System.Web.Routing;
using Routine.Core;
using Routine.Core.Extractor;
using Routine.Core.Interceptor;
using Routine.Soa.Context;

namespace Routine.Soa.Configuration
{
	public class GenericSoaConfiguration : ISoaConfiguration
	{
		private const string ACTION = "action";

		public MultipleExtractor<GenericSoaConfiguration, Exception, SoaExceptionResult> ExtractExceptionResult { get; private set; }

		public ChainInterceptor<GenericSoaConfiguration, InterceptionContext> InterceptGetApplicationModel { get; private set; }
		public ChainInterceptor<GenericSoaConfiguration, ObjectModelInterceptionContext> InterceptGetObjectModel { get; private set; }
		public ChainInterceptor<GenericSoaConfiguration, ObjectModelInterceptionContext> InterceptGetAvailableObjects { get; private set; }
		public ChainInterceptor<GenericSoaConfiguration, ObjectReferenceInterceptionContext> InterceptGetValue { get; private set; }
		public ChainInterceptor<GenericSoaConfiguration, ObjectReferenceInterceptionContext> InterceptGet { get; private set; }
		public ChainInterceptor<GenericSoaConfiguration, MemberInterceptionContext> InterceptGetMember { get; private set; }
		public ChainInterceptor<GenericSoaConfiguration, OperationInterceptionContext> InterceptGetOperation { get; private set; }
		public ChainInterceptor<GenericSoaConfiguration, PerformOperationInterceptionContext> InterceptPerformOperation { get; private set; }

		public GenericSoaConfiguration() : this(true) { }

		internal GenericSoaConfiguration(bool rootConfig)
		{
			ExtractExceptionResult = new MultipleExtractor<GenericSoaConfiguration, Exception, SoaExceptionResult>(this, "ExceptionResult");

			InterceptGetApplicationModel = new ChainInterceptor<GenericSoaConfiguration, InterceptionContext>(this);
			InterceptGetObjectModel = new ChainInterceptor<GenericSoaConfiguration, ObjectModelInterceptionContext>(this);
			InterceptGetAvailableObjects = new ChainInterceptor<GenericSoaConfiguration, ObjectModelInterceptionContext>(this);
			InterceptGetValue = new ChainInterceptor<GenericSoaConfiguration, ObjectReferenceInterceptionContext>(this);
			InterceptGet = new ChainInterceptor<GenericSoaConfiguration, ObjectReferenceInterceptionContext>(this);
			InterceptGetMember = new ChainInterceptor<GenericSoaConfiguration, MemberInterceptionContext>(this);
			InterceptGetOperation = new ChainInterceptor<GenericSoaConfiguration, OperationInterceptionContext>(this);
			InterceptPerformOperation = new ChainInterceptor<GenericSoaConfiguration, PerformOperationInterceptionContext>(this);

			if (rootConfig)
			{
				RegisterRoutes();
			}
		}

		public GenericSoaConfiguration Merge(GenericSoaConfiguration other)
		{
			ExtractExceptionResult.Merge(other.ExtractExceptionResult);

			InterceptGetApplicationModel.Merge(other.InterceptGetApplicationModel);
			InterceptGetObjectModel.Merge(other.InterceptGetObjectModel);
			InterceptGetAvailableObjects.Merge(other.InterceptGetAvailableObjects);
			InterceptGetValue.Merge(other.InterceptGetValue);
			InterceptGet.Merge(other.InterceptGet);
			InterceptGetMember.Merge(other.InterceptGetMember);
			InterceptGetOperation.Merge(other.InterceptGetOperation);
			InterceptPerformOperation.Merge(other.InterceptPerformOperation);

			return this;
		}

		private void RegisterRoutes()
		{
			RouteTable.Routes.MapRoute(
				"Soa",
				"Soa/{"+ACTION+"}/{id}",
				new {controller="Soa", action="Index", id=""}
			);
		}

		#region ISoaConfiguration implementation

		string ISoaConfiguration.ActionRouteName { get { return ACTION; } }

		IExtractor<Exception, SoaExceptionResult> ISoaConfiguration.ExceptionResultExtractor { get { return ExtractExceptionResult; } }

		IInterceptor<InterceptionContext> ISoaConfiguration.GetApplicationModelInterceptor { get { return InterceptGetApplicationModel; } }
		IInterceptor<ObjectModelInterceptionContext> ISoaConfiguration.GetObjectModelInterceptor { get { return InterceptGetObjectModel; } }
		IInterceptor<ObjectModelInterceptionContext> ISoaConfiguration.GetAvailableObjectsInterceptor { get { return InterceptGetAvailableObjects; } }
		IInterceptor<ObjectReferenceInterceptionContext> ISoaConfiguration.GetValueInterceptor { get { return InterceptGetValue; } }
		IInterceptor<ObjectReferenceInterceptionContext> ISoaConfiguration.GetInterceptor { get { return InterceptGet; } }
		IInterceptor<MemberInterceptionContext> ISoaConfiguration.GetMemberInterceptor { get { return InterceptGetMember; } }
		IInterceptor<OperationInterceptionContext> ISoaConfiguration.GetOperationInterceptor { get { return InterceptGetOperation; } }
		IInterceptor<PerformOperationInterceptionContext> ISoaConfiguration.PerformOperationInterceptor { get { return InterceptPerformOperation; } }

		#endregion
	}
}

