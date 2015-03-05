using System.Collections.Generic;
using System.Linq;
using Routine.Client;

namespace Routine.Ui
{
	public class OperationViewModel : ViewModelBase
	{
		public Robject Object { get; set; }
		public Roperation Operation { get; private set; }

		public OperationViewModel(IMvcConfiguration configuration, Robject @object, Roperation operation)
			: base(configuration)
		{
			Object = @object;
			Operation = operation;
		}

		public override string SpecificViewName
		{
			get { return string.Format("{0}{1}{2}", Operation.Type.Name, Configuration.GetViewNameSeparator(), Id); }
		}

		public ObjectViewModel Target { get { return new ObjectViewModel(Configuration, Object); } }
		public string Id { get { return Operation.Id; } }
		public string Text { get { return Configuration.GetDisplayName(Operation.Id); } }

		public bool HasParameter { get { return Operation.Parameters.Any(); } }
		public bool ReturnsList { get { return Operation.ResultIsList; } }

		public bool IsRendered { get { return Configuration.IsRendered(this); } }
		public bool IsAvailable { get { return Configuration.IsAvailable(this); } }
		public bool ConfirmationRequired { get { return Configuration.GetConfirmationRequired(this); } }

		public List<ParameterViewModel> Parameters
		{
			get
			{
				return Operation.Parameters.Select(p => new ParameterViewModel(Configuration, p, Object)).ToList();
			}
		}

		public bool Is(OperationTypes types)
		{
			return Configuration.GetOperationTypes(this).HasFlag(types);
		}

		public int GetOrder() { return GetOrder(OperationTypes.None); }
		public int GetOrder(OperationTypes operationTypes)
		{
			return Configuration.GetOrder(this, operationTypes);
		}

		public bool MarkedAs(string mark)
		{
			return Operation.MarkedAs(mark);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(OperationViewModel))
				return false;

			var other = (OperationViewModel)obj;

			return Operation.Id == other.Operation.Id;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Text != null ? Text.GetHashCode() : 0);
			}
		}
	}
}
