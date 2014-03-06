using System.Web.Mvc;
using System.Web.Mvc.Html;
using Routine.Api;

namespace Routine.Mvc
{
	public abstract class ViewModelBase
	{
		private readonly IMvcConfiguration mvcConfig;
		private readonly IFactory factory;

		protected ViewModelBase(IMvcConfiguration mvcConfig, IFactory factory)
		{
			this.mvcConfig = mvcConfig;
			this.factory = factory;
		}

		protected IMvcConfiguration MvcConfig{get{return mvcConfig;}}
		public ApplicationViewModel Application{get{return Create<ApplicationViewModel>();}}

		protected MenuViewModel CreateMenu() {return Create<MenuViewModel>();}
		protected ObjectViewModel CreateObject() {return Create<ObjectViewModel>();}
		protected MemberViewModel CreateMember() {return Create<MemberViewModel>();}
		protected OperationViewModel CreateOperation() {return Create<OperationViewModel>();}
		protected ParameterViewModel CreateParameter() {return Create<ParameterViewModel>();}
		protected VariableViewModel CreateVariable() {return Create<VariableViewModel>();}
		protected T Create<T>() { return factory.Create<T>(); }

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

		protected Robject Robj(string id, string modelId)
		{
			return factory.Create<Robject>().With(id, modelId);
		}
	}
}
