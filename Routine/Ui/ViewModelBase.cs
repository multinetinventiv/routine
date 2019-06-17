using System.Globalization;
using System.Web.Mvc;

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

		public string GetViewName(ControllerContext controllerContext) { return GetViewName(controllerContext, null); }
		public string GetViewName(ControllerContext controllerContext, string mode)
		{
			if (MayHaveSpecificView)
			{
				var specificViewName = Combine(SpecificViewName, mode);

				if (ViewEngines.Engines.FindPartialView(controllerContext, specificViewName).View != null)
				{
					return specificViewName;
				}
			}

			return Combine(configuration.GetViewName(this), mode);
		}

		private string Combine(string viewName, string mode)
		{
			if (string.IsNullOrEmpty(mode))
			{
				return viewName;
			}

			return string.Format("{0}{1}{2}", viewName, Configuration.GetViewNameSeparator(), mode.ToLower(CultureInfo.InvariantCulture));
		}

		private bool MayHaveSpecificView { get { return !string.IsNullOrEmpty(SpecificViewName); } }
		public virtual string SpecificViewName { get { return null; } }
	}
}
