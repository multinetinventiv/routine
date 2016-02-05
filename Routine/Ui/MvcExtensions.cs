using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Routine.Ui
{
	public static class MvcExtensions
	{
		public static string Route(this UrlHelper source, ObjectViewModel model) { return source.Route(model, false); }
		public static string Route(this UrlHelper source, ObjectViewModel model, bool ignoreGetAsRoute)
		{
			if (model == null) { return null; }

			return source.RouteUrl(model.GetRouteName(ignoreGetAsRoute), model.GetRouteValues(ignoreGetAsRoute));
		}

		public static string Route(this UrlHelper source, OperationViewModel model)
		{
			return source.RouteUrl(
				model.GetRouteName(),
				model.GetRouteValues());
		}

		public static string GetRouteName(this ObjectViewModel source) { return source.GetRouteName(false); }
		public static string GetRouteName(this ObjectViewModel source, bool ignoreGetAsRoute)
		{
			return source.Object.IsNaked || ignoreGetAsRoute ? Constants.MVC_GET_ROUTE_NAME : Constants.MVC_GET_AS_ROUTE_NAME;
		}

		private static string GetRouteName(this OperationViewModel source)
		{
			return source.Object.IsNaked ? Constants.MVC_PERFORM_ROUTE_NAME : Constants.MVC_PERFORM_AS_ROUTE_NAME;
		}

		public static RouteValueDictionary GetRouteValues(this ObjectViewModel source) { return source.GetRouteValues(false); }
		public static RouteValueDictionary GetRouteValues(this ObjectViewModel source, bool ignoreViewModelId)
		{
			if (source.Object.IsNaked || ignoreViewModelId)
			{
				return new RouteValueDictionary(new { id = source.Object.Id, modelId = source.Object.ActualType.Id });
			}

			return new RouteValueDictionary(new { id = source.Object.Id, actualModelId = source.Object.ActualType.Id, viewModelId = source.Object.ViewType.Id });
		}

		public static RouteValueDictionary GetRouteValues(this OperationViewModel source)
		{
			var result = source.Target.GetRouteValues(false);
			result.Add("operationName", source.Operation.Name);
			return result;
		}

		public static MvcHtmlString Partial(this HtmlHelper htmlHelper, ViewModelBase model) { return htmlHelper.Partial(model, VDD()); }
		public static MvcHtmlString Partial(this HtmlHelper htmlHelper, ViewModelBase model, object viewData) { return htmlHelper.Partial(model, VDD(viewData)); }
		public static MvcHtmlString Partial(this HtmlHelper htmlHelper, ViewModelBase model, ViewDataDictionary viewData)
		{
			return htmlHelper.Partial(model.GetViewName(htmlHelper.ViewContext.Controller.ControllerContext), model, viewData);
		}

		public static MvcHtmlString Partial(this HtmlHelper htmlHelper, ViewModelBase model, string mode) { return htmlHelper.Partial(model, mode, VDD()); }
		public static MvcHtmlString Partial(this HtmlHelper htmlHelper, ViewModelBase model, string mode, object viewData) { return htmlHelper.Partial(model, mode, VDD(viewData)); }
		public static MvcHtmlString Partial(this HtmlHelper htmlHelper, ViewModelBase model, string mode, ViewDataDictionary viewData)
		{
			return htmlHelper.Partial(model.GetViewName(htmlHelper.ViewContext.Controller.ControllerContext, mode), model, viewData);
		}

		private static ViewDataDictionary VDD() { return VDD(null); }
		private static ViewDataDictionary VDD(object viewData)
		{
			var result = new ViewDataDictionary();

			if (viewData == null) { return result; }

			var rvd = new RouteValueDictionary(viewData);

			foreach (var key in rvd.Keys)
			{
				result[key] = rvd[key];
			}

			return result;
		}
	}
}