using Routine.Api;
using System.Collections.Generic;
using System.Linq;

namespace Routine.Mvc
{
	public class MemberViewModel : ViewModelBase
	{
        public MemberViewModel(IMvcContext mvcContext)
			: base(mvcContext) {}

		private Rmember rmem;

		internal MemberViewModel With(Rmember rmem)
		{
			this.rmem = rmem;

			return this;
		}

		public string Id{get{return rmem.Id;}}
		public string Text {get{return MvcConfig.DisplayNameExtractor.Extract(rmem.Id);}}

		public bool IsList{get{return rmem.IsList;}}

		public ObjectViewModel Object
		{
			get
			{
				return CreateObject().With(rmem.GetValue().Object);
			}
		}

		public List<ObjectViewModel> List
		{
			get
			{
				return rmem
						.GetValue().List
						.Select(robj => CreateObject().With(robj))
						.ToList();
			}
		}
	}
}
