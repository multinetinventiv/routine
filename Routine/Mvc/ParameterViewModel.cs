using Routine.Api;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Routine.Mvc
{
	public class ParameterViewModel : ViewModelBase
	{		
		public ParameterViewModel(IMvcConfiguration mvcConfig, IFactory factory)
			: base(mvcConfig, factory) {}

		private Rparameter rpar;

		internal ParameterViewModel With(Rparameter rpar)
		{
			this.rpar = rpar;

			return this;
		}

		public string DataType{get{return rpar.ViewModelId;}}
		public string Id{get{return rpar.Id;}}
		public string Text{get{return MvcConfig.DisplayNameExtractor.Extract(rpar.Id);}}
		public bool IsList{get{return rpar.IsList;}}

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
						.Select(o => new OptionViewModel(o.Id + "|" + o.ActualModelId, o.Value))
						.ToList();
				}

				return options;
			}
		}

		public class OptionViewModel
		{
			public string Id{get;private set;}
			public string Value{get;private set;}

			public OptionViewModel(string id, string value)
			{
				Id = id;
				Value = value;
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
