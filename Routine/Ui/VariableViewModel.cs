using System.Collections.Generic;
using System.Linq;
using Routine.Client;

namespace Routine.Ui
{
	public class VariableViewModel : ViewModelBase
	{
		public Rvariable Variable { get; private set; }

		public VariableViewModel(IMvcConfiguration configuration, Rvariable rvar)
			: base(configuration)
		{
			Variable = rvar;
		}

		public string Name { get { return Variable.Name; } }
		public bool IsVoid { get { return Variable.IsVoid; } }
		public bool IsList { get { return Variable.IsList; } }

		public ObjectViewModel Object { get { return new ObjectViewModel(Configuration, Variable.Object); } }
		public List<ObjectViewModel> List { get { return Variable.List.Select(o => new ObjectViewModel(Configuration, o)).ToList(); } }
	}
}
