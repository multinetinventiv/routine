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

		protected void RenderListPage(VariableViewModel vvm) { View("ListPage", vvm).ExecuteResult(ControllerContext); }
		protected ActionResult Page(ObjectViewModel ovm) { View("Page", ovm).ExecuteResult(ControllerContext); return new EmptyResult(); }
		protected ActionResult ModalPage(ObjectViewModel ovm) { View("ModalPage", ovm).ExecuteResult(ControllerContext); return new EmptyResult(); }

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
				() => 
				{
					var innerResult = target.Perform(operationModelId, parameters);

					if(innerResult.IsList)
					{
						RenderListPage(innerResult);
					}

					return innerResult;
				}) as VariableViewModel;

			if(result.IsVoid)
			{
				return Redirect(Request.UrlReferrer.ToString());
			}

			if(result.IsList)
			{
				return new EmptyResult();
			}

			return RedirectToPage(result.Object);
		}

		public ActionResult Get(string id, string modelId) 
		{
			return context.MvcConfiguration.GetInterceptor.Intercept(
				context.CreateGetInterceptionContext(id, modelId),
				() => 
				{
					var ovm = context.Application.Get(id, modelId);

					if(Request["modal"] == "true")
					{
						return ModalPage(ovm);
					}

					return Page(ovm);
				}
			) as ActionResult;
		}

		public ActionResult GetAs(string id, string actualModelId, string viewModelId)
		{
			return context.MvcConfiguration.GetAsInterceptor.Intercept(
				context.CreateGetAsInterceptionContext(id, actualModelId, viewModelId),
				() => 
				{
					var ovm = context.Application.Get(id, actualModelId, viewModelId);
					
					if(Request.QueryString["modal"] == "true")
					{
						return ModalPage(ovm);
					}

					return Page(ovm);
				}
			) as ActionResult;
		}
    }
}
