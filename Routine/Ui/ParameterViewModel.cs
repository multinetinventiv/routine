using System.Collections.Generic;
using System.Linq;
using Routine.Client;

namespace Routine.Ui
{
	public class ParameterViewModel : ViewModelBase
	{
		public Rparameter Parameter { get; private set; }
		public Robject Target { get; private set; }
		public Rparametric Operation { get { return Parameter.Owner; } }

		public ParameterViewModel(IMvcConfiguration configuration, Rparameter rpar, Robject target)
			: base(configuration)
		{
			Parameter = rpar;
			Target = target;
		}

		public string DataType { get { return Parameter.ParameterType.Id; } }
		public string Id { get { return Parameter.Name; } }
		public string Text { get { return Configuration.GetDisplayName(Parameter.Name); } }
		public bool IsList { get { return Parameter.IsList; } }
		public bool IsValue { get { return Parameter.ParameterType.IsValueType; } }

		public string DefaultValue
		{
			get
			{
				var result = Configuration.GetDefault(this);

				if (result == null)
				{
					return null;
				}

				return result.ToValueString(Configuration.GetListValueSeparator());
			}
		}

		private List<OptionViewModel> options;
		public bool HasOptions { get { return Options.Any(); } }
		public List<OptionViewModel> Options
		{
			get
			{
				if (options == null)
				{
					options = Configuration.GetOptions(this)
						.Select(o => new OptionViewModel(Configuration, this, o))
						.OrderBy(o => o.Order)
						.ToList();
				}

				return options;
			}
		}

		public ObjectViewModel GetSearcher()
		{
			var searcher = Configuration.GetSearcher(Parameter);

			if (searcher == null)
			{
				return null;
			}

			return new ObjectViewModel(Configuration, searcher);
		}
	}

	public static class Rvariable_ParameterViewModelExtensions
	{
		public static string ToValueString(this Rvariable source, char separator)
		{
			var result = "";
			foreach (var robj in source.List)
			{
				result += robj.Display + separator;
			}
			return result.BeforeLast(separator);
		}
	}
}
