using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Routine.Ui
{
	public abstract class ViewModelBase
	{
		private readonly IMvcConfiguration configuration;

		protected ViewModelBase(IMvcConfiguration configuration)
		{
			this.configuration = configuration;
		}

		protected IMvcConfiguration Configuration { get { return configuration; } }

		public void Render(HtmlHelper html, params object[] viewData) { RenderAs(html, null, viewData); }
		public void RenderAs(HtmlHelper html, string type, params object[] viewData)
		{
			var viewName = Configuration.GetViewName(this);

			if(!string.IsNullOrEmpty(type)) { viewName += Configuration.GetViewNameSeparator() + type; }

			var viewDataDict = new ViewDataDictionary();
			for(int i = 0; i<viewData.Length; i+=2)
			{
				viewDataDict.Add(viewData[i] as string, viewData[i + 1]);
			}
			html.RenderPartial(viewName, this, viewDataDict);
		}
	}
}
