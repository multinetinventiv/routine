using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Routine.Mvc;

namespace Routine.Mvc
{
	public class MvcController : Controller
    {
		private readonly IMvcContext context;

		public MvcController(IMvcContext context) 
		{
			this.context = context; 
		}

		protected ActionResult ListPage(VariableViewModel vvm) { View("ListPage", vvm).ExecuteResult(ControllerContext); return new EmptyResult(); }
		protected ActionResult Page(ObjectViewModel ovm) { View("Page", ovm).ExecuteResult(ControllerContext); return new EmptyResult(); }

		protected ActionResult RedirectToPage(ObjectViewModel ovm) { return RedirectToRoute(ovm.ViewRouteName, ovm.RouteValues); }

		public ActionResult Index() { return RedirectToPage(context.Application.Index); }

		public ActionResult Perform(string id, string modelId, string operationModelId, Dictionary<string, string> parameters)  
		{ 
			return Perform_Inner(context.Application.Get(id, modelId), operationModelId, parameters); 
		}
		public ActionResult PerformAs(string id, string actualModelId, string viewModelId, string operationModelId, Dictionary<string, string> parameters) 
		{
			return Perform_Inner(context.Application.Get(id, actualModelId, viewModelId), operationModelId, parameters); 
		}
		private ActionResult Perform_Inner(ObjectViewModel target, string operationModelId, Dictionary<string, string> parameters)
		{
			var result = context.MvcConfiguration.PerformInterceptor.Intercept(
					context.CreatePerformInterceptionContext(target, operationModelId, parameters), 
					() => target.Perform(operationModelId, parameters)) as VariableViewModel;

			if(result.IsVoid)
			{
				return Redirect(Request.UrlReferrer.ToString());
			}

			if(result.IsList)
			{
				return ListPage(result);
			}

			return RedirectToPage(result.Object);
		}

		public ActionResult Get(string id, string modelId) 
		{
			return context.MvcConfiguration.GetInterceptor.Intercept(
				context.CreateGetInterceptionContext(id, modelId),
				() => Page(context.Application.Get(id, modelId))) as ActionResult;
		}

		public ActionResult GetAs(string id, string actualModelId, string viewModelId)
		{
			return context.MvcConfiguration.GetAsInterceptor.Intercept(
				context.CreateGetAsInterceptionContext(id, actualModelId, viewModelId),
				() => Page(context.Application.Get(id, actualModelId, viewModelId))) as ActionResult;
		}

		//TODO JSON actions
		//do	--> client redirects if there is a single non-value type object
		//		--> client refreshes object state and shows popup (or something else) when result is list or value type
		//		--> client refreshes object state if result is void
		//get member
		//get member as table
		//keep track of the user navigation so that it can be shown as like breadcrumb
    }
}
