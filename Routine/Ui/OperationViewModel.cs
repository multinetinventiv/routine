using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
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

		public ObjectViewModel Target { get { return new ObjectViewModel(Configuration, Object); } }
		public bool IsAvailable { get { return Configuration.IsAvailable(this); } }
		public bool IsSimple { get { return Configuration.IsSimple(this); } }
		public string Id { get { return Operation.Id; } }
		public string Text { get { return Configuration.GetDisplayName(Operation.Id); } }
		public bool HasParameter { get { return Operation.Parameters.Any(); } }
		public bool ReturnsList { get { return Operation.ResultIsList; } }

		public List<ParameterViewModel> Parameters
		{
			get
			{
				return Operation.Parameters.Select(p => new ParameterViewModel(Configuration, p)).ToList();
			}
		}

		public string PerformRouteName { get { return Target.PerformRouteName; } }
		public RouteValueDictionary RouteValues
		{
			get
			{
				var result = Target.RouteValuesIncludingViewModelId;
				result.Add("operationModelId", Operation.Id);
				return result;
			}
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

	public static class UrlHelper_OperationViewModelExtensions
	{
		public static string Route(this UrlHelper source, OperationViewModel model)
		{
			return source.RouteUrl(model.PerformRouteName, model.RouteValues);
		}
	}
}
