using System.Collections.Generic;
using System.Linq;
using Routine.Core.Api;
using System.Web.Mvc;

namespace Routine.Mvc
{
	public class ParameterViewModel : ViewModelBase
	{
        public ParameterViewModel(IMvcContext mvcContext)
			: base(mvcContext) {}

		private Rparameter rpar;

		internal ParameterViewModel With(Rparameter rpar)
		{
			this.rpar = rpar;

			return this;
		}

		public string DataType{ get { return rpar.ViewModelId; } }
		public ObjectViewModel GetSearcher(string id)
		{
			var rapp = rpar.Operation.Object.Application;
			var vm = rapp.ObjectModel[rpar.ViewModelId];
			var som = rapp.ObjectModels.SingleOrDefault(om => om.Module == vm.Module && om.Name == vm.Name + "s");

			if(som == null)
			{
				return null;
			}
				
			return CreateObject().With(rapp.Get(id, som.Id));
		}
		public string Id{ get { return rpar.Id; } }
		public string Text{ get { return MvcConfig.DisplayNameExtractor.Extract(rpar.Id); } }
		public bool IsList{ get { return rpar.IsList; } }
		public bool IsValue{ get { return rpar.Operation.Object.Application.ObjectModel[rpar.ViewModelId].IsValueModel; } }

		public string DefaultValue
		{
			get
			{
				var result = MvcConfig.ParameterDefaultExtractor.Extract(rpar);

				if(result == null)
				{
					return null;
				}

				return result.ToValueString(MvcConfig.ListValueSeparator);
			}
		}

		private List<OptionViewModel> options;
		public bool HasOptions{get{return Options.Any();}}
		public List<OptionViewModel> Options
		{
			get
			{
				if(options == null)
				{
					options = MvcConfig.ParameterOptionsExtractor
						.Extract(rpar)
						.Select(o => new OptionViewModel(o))
						.ToList();
				}

				return options;
			}
		}

		public class OptionViewModel
		{
			public string Id{get;private set;}
			public string Value{get;private set;}

			public OptionViewModel(Robject robj)
			{
				Id = robj.Id + "|" + robj.ActualModelId;
				Value = robj.Value;
			}
		}
	}

	public static class Rvariable_ParameterViewModelExtensions
	{
		public static string ToValueString(this Rvariable source, char separator)
		{				
			var result = "";
			foreach(var robj in source.List)
			{
				result += robj.Value + separator;
			}
			return result.BeforeLast(separator);
		}
	}
}
