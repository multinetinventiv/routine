using System.Collections.Generic;
using System.Linq;
using Routine.Client;
using Routine.Core.Rest;

namespace Routine.Ui
{
	public class ParameterViewModel : ViewModelBase
	{
		public Rparameter Parameter { get; private set; }

		public ParameterViewModel(IMvcConfiguration configuration, Rparameter rpar)
			: base(configuration)
		{
			Parameter = rpar;
		}

		public string DataType { get { return Parameter.ParameterType.Id; } }
		public string Id { get { return Parameter.Id; } }
		public string Text { get { return Configuration.GetDisplayName(Parameter.Id); } }
		public bool IsList { get { return Parameter.IsList; } }
		public bool IsValue { get { return Parameter.ParameterType.IsValueType; } }

		public string DefaultValue
		{
			get
			{
				var result = Configuration.GetDefault(Parameter);

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
					options = Configuration.GetOptions(Parameter)
						.Select(o => new OptionViewModel(o))
						.ToList();
				}

				return options;
			}
		}

		public class OptionViewModel
		{
			public string Id { get; private set; }
			public string Value { get; private set; }

			public OptionViewModel(Robject robj)
			{
				Id = robj.ObjectReferenceData.ToSerializable().ToString();
				Value = robj.Value;
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
				result += robj.Value + separator;
			}
			return result.BeforeLast(separator);
		}
	}
}
