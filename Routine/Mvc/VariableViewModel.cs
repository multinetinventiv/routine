using System.Linq;
using Routine.Core.Api;
using System.Collections.Generic;

namespace Routine.Mvc
{
	public class VariableViewModel : ViewModelBase
	{
        public VariableViewModel(IMvcContext mvcContext)
			: base(mvcContext) {}

		private Rvariable rvar;

		internal VariableViewModel With(Rvariable rvar)
		{
			this.rvar = rvar;

			return this;
		}

		public bool IsVoid {get{return rvar.IsVoid;}}
		public bool IsList{get{return rvar.IsList;}}

		public ObjectViewModel Object{get{return CreateObject().With(rvar.Object);}}
		public List<ObjectViewModel> List{get{return rvar.List.Select(o => CreateObject().With(o)).ToList();}}
	}
}
