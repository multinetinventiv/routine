using System.Collections.Generic;
using System.Web.Mvc;
using Routine.Interception;
using Routine.Ui.Context;

namespace Routine.Ui
{
	public class RoutineController : Controller
	{
		public static string ControllerName { get { return typeof(RoutineController).Name.BeforeLast("Controller"); } }
		public static string DefaultAction { get { return "Index"; } }
		public static string PerformAction { get { return "Perform"; } }
		public static string PerformAsAction { get { return "PerformAs"; } }
		public static string GetAction { get { return "Get"; } }
		public static string GetAsAction { get { return "GetAs"; } }

		private readonly IMvcConfiguration configuration;
		private readonly ApplicationViewModel application;

		public RoutineController(IMvcContext context)
		{
			configuration = context.Configuration;
			application = context.Application;
		}

		protected void RenderListPage(VariableViewModel vvm) { View("ListPage", vvm).ExecuteResult(ControllerContext); }
		protected ActionResult Page(ObjectViewModel ovm) { View("Page", ovm).ExecuteResult(ControllerContext); return new EmptyResult(); }
		protected ActionResult ModalPage(ObjectViewModel ovm) { View("ModalPage", ovm).ExecuteResult(ControllerContext); return new EmptyResult(); }

		protected ActionResult RedirectToPage(ObjectViewModel ovm) { return RedirectToRoute(ovm.GetRouteName(), ovm.GetRouteValues()); }

		public ActionResult Index()
		{
			var context = new InterceptionContext(InterceptionTarget.Index.ToString());
			return configuration.GetInterceptor(InterceptionTarget.Index).Intercept(
				context,
				() =>
				{
					return RedirectToPage(application.Index);
				}
			) as ActionResult;
		}

		public ActionResult Perform(string id, string modelId, string operationModelId, Dictionary<string, string> parameters)
		{
			return Perform_Inner(application.Get(id, modelId), operationModelId, parameters);
		}

		public ActionResult PerformAs(string id, string actualModelId, string viewModelId, string operationModelId, Dictionary<string, string> parameters)
		{
			return Perform_Inner(application.Get(id, actualModelId, viewModelId), operationModelId, parameters);
		}

		private ActionResult Perform_Inner(ObjectViewModel target, string operationModelId, Dictionary<string, string> parameters)
		{
			var context = new PerformInterceptionContext(InterceptionTarget.Perform.ToString(), target, operationModelId, parameters);
			var result = configuration.GetInterceptor(InterceptionTarget.Perform).Intercept(
					context,
				() =>
				{
					var innerResult = target.Perform(context.OperationModelId, context.Parameters);

					if (innerResult.IsList)
					{
						RenderListPage(innerResult);
					}

					return innerResult;
				}) as VariableViewModel;

			if (result.IsVoid)
			{
				return Redirect(Request.UrlReferrer.ToString());
			}

			if (result.IsList)
			{
				return new EmptyResult();
			}

			return RedirectToPage(result.Object);
		}

		public ActionResult Get(string id, string modelId)
		{
			var context = new GetInterceptionContext(InterceptionTarget.Get.ToString(), id, modelId);
			return configuration.GetInterceptor(InterceptionTarget.Get).Intercept(
				context,
				() =>
				{
					var ovm = application.Get(context.Id, context.ActualModelId);

					if (Request["modal"] == "true")
					{
						return ModalPage(ovm);
					}

					return Page(ovm);
				}
			) as ActionResult;
		}

		public ActionResult GetAs(string id, string actualModelId, string viewModelId)
		{
			var context = new GetAsInterceptionContext(InterceptionTarget.GetAs.ToString(), id, actualModelId, viewModelId);
			return configuration.GetInterceptor(InterceptionTarget.GetAs).Intercept(
				context,
				() =>
				{
					var ovm = application.Get(context.Id, context.ActualModelId, context.ViewModelId);

					if (Request.QueryString["modal"] == "true")
					{
						return ModalPage(ovm);
					}

					return Page(ovm);
				}
			) as ActionResult;
		}
	}
}
