using System.Collections.Generic;
using System.Linq;
using Routine.Api;

namespace Routine.Mvc
{
	public class MenuViewModel : ViewModelBase
	{		
		public MenuViewModel(IMvcConfiguration mvcConfig, IFactory factory)
			: base(mvcConfig, factory) {}

		private List<ObjectViewModel> links;
		internal MenuViewModel With(List<Robject> links)
		{
			this.links = links.Select(l => CreateObject().With(l)).ToList();

			return this;
		}

		public List<ObjectViewModel> Links {get{return links;}}
	}
}
