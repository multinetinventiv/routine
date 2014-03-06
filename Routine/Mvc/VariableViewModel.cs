using Routine.Api;

namespace Routine.Mvc
{
	public class VariableViewModel : ViewModelBase
	{
		public VariableViewModel(IMvcConfiguration mvcConfig, IFactory factory)
			: base(mvcConfig, factory) {}

		private Rvariable rvar;

		internal VariableViewModel With(Rvariable rvar)
		{
			this.rvar = rvar;

			return this;
		}

		public bool IsVoid {get{return rvar.IsVoid;}}
		public bool IsList{get{return rvar.IsList;}}

		public ObjectViewModel Object{get{return Create<ObjectViewModel>().With(rvar.Object);}}
	}
}
