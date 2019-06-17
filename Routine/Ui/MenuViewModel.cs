using System.Collections.Generic;
using System.Linq;
using Routine.Client;

namespace Routine.Ui
{
	public class MenuViewModel : ViewModelBase
	{
		//TODO support tree menu
		//TODO cache full menu in appcache
		public List<ObjectViewModel> Links { get; private set; }

		public MenuViewModel(IMvcConfiguration configuration, List<Robject> links)
			: base(configuration)
		{
			Links = links.Select(l => new ObjectViewModel(Configuration, l)).ToList();
		}
	}
}
