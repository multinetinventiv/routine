using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Routine.Mvc;

namespace Routine.Mvc
{
	public class MvcController : Controller
    {
		private readonly ApplicationViewModel avm;

		public MvcController(ApplicationViewModel avm) 
		{ 
			this.avm = avm; 
		}

		protected ActionResult Page(ObjectViewModel ovm) { return View("Page", ovm); }

		protected ActionResult RedirectToPage(ObjectViewModel ovm) { return RedirectToRoute(ovm.ViewRouteName, ovm.RouteValues); }

		public ActionResult Index () { return RedirectToPage(avm.Index); }

		public ActionResult Perform(string id, string modelId, string operationModelId, Dictionary<string, string> parameters)  
		{ 
			return Perform_Inner(avm.Get(id, modelId), operationModelId, parameters); 
		}
		public ActionResult PerformAs(string id, string actualModelId, string viewModelId, string operationModelId, Dictionary<string, string> parameters) 
		{
			return Perform_Inner(avm.Get(id, actualModelId, viewModelId), operationModelId, parameters); 
		}
		private ActionResult Perform_Inner(ObjectViewModel target, string operationModelId, Dictionary<string, string> parameters)
		{
			var result = target.Perform(operationModelId, parameters);

			if(result.IsVoid)
			{
				return Redirect(Request.UrlReferrer.ToString());
			}

			if(result.IsList)
			{
				throw new NotImplementedException();
			}

			return RedirectToPage(result.Object);
		}

		public ActionResult Get(string id, string modelId) { return Page(avm.Get(id, modelId)); }
		public ActionResult GetAs(string id, string actualModelId, string viewModelId) { return Page(avm.Get(id, actualModelId, viewModelId)); }

		//TODO JSON actions
		//do	--> client redirects if there is a single non-value type object
		//		--> client refreshes object state and shows popup (or something else) when result is list or value type
		//		--> client refreshes object state if result is void
		//get member
		//get member as table
		//keep track of the user navigation so that it can be shown as like breadcrumb
    }
}
