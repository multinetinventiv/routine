using System.Web.Mvc;
using System.Web.Mvc.Html;
using Routine.Core.Api;

namespace Routine.Mvc
{
	public abstract class ViewModelBase
	{
		private readonly IMvcContext context;

		protected ViewModelBase(IMvcContext context)
		{
            this.context = context;
		}

        protected IMvcConfiguration MvcConfig { get { return context.MvcConfiguration; } }
		public ApplicationViewModel Application{get{return context.Application;}}

        protected MenuViewModel CreateMenu() { return context.CreateMenu(); }
        protected ObjectViewModel CreateObject() { return context.CreateObject(); }
        protected MemberViewModel CreateMember() { return context.CreateMember(); }
        protected OperationViewModel CreateOperation() { return context.CreateOperation(); }
        protected ParameterViewModel CreateParameter() { return context.CreateParameter(); }
        protected VariableViewModel CreateVariable() { return context.CreateVariable(); }

		public void Render(HtmlHelper html, params object[] viewData) { RenderAs(html, null, viewData); }
		public void RenderAs(HtmlHelper html, string type, params object[] viewData)
		{
			var viewName = MvcConfig.ViewNameExtractor.Extract(this);

			if(!string.IsNullOrEmpty(type)) { viewName += MvcConfig.ViewNameSeparator + type; }

			var viewDataDict = new ViewDataDictionary();
			for(int i = 0; i<viewData.Length; i+=2)
			{
				viewDataDict.Add(viewData[i] as string, viewData[i + 1]);
			}
			html.RenderPartial(viewName, this, viewDataDict);
		}

		internal virtual Robject Robj(string id, string modelId)
		{
			return context.Application.Robj(id, modelId);
		}
	}
}
