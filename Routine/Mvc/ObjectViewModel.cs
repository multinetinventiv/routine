using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Routine.Core.Api;
using System.Diagnostics;

namespace Routine.Mvc
{
	public class ObjectViewModel : ViewModelBase
	{
        public ObjectViewModel(IMvcContext mvcContext)
			: base(mvcContext) {}

		private Robject robj;

		internal ObjectViewModel With(Robject robj)
		{
			this.robj = robj;

			return this;
		}

		public string ViewModelId {get{return robj.ViewModelId;}}
		public string Module{get{return robj.Module;}}

		public string Title { get { return robj.IsNull?MvcConfig.NullDisplayValue:robj.Value; } }
		public bool HasDetail { get { return !robj.IsNull && robj.IsDomain && (robj.Members.Any() || robj.Operations.Any()); } }

		private string ViewRouteNameBase{get{return MvcConfig.ViewRouteNameExtractor.Extract(this);}}
		public string ViewRouteName { get {return ViewRouteNameBase;} }

		private string PerformRouteNameBase{get{return MvcConfig.PerformRouteNameExtractor.Extract(this);}}
		public string PerformRouteName { get {return robj.IsNaked?PerformRouteNameBase:PerformRouteNameBase+"As";} }

		public RouteValueDictionary RouteValues
		{
			get
			{
				return new RouteValueDictionary(new {id = robj.Id, modelId = robj.ActualModelId});
			}
		}

		public RouteValueDictionary RouteValuesIncludingViewModelId
		{
			get
			{
				if(robj.IsNaked) {return RouteValues;}

				return new RouteValueDictionary(new {id = robj.Id, actualModelId = robj.ActualModelId, viewModelId = robj.ViewModelId});
			}
		}

		public bool HasOperation{get{return robj.Operations.Any();}}
		public bool HasSimpleMember{get{return robj.Members.Any(m => !m.IsList);}}
		public bool HasTableMember{get{return robj.Members.Any(m => m.IsList);}}

		private List<MemberViewModel> Members
		{
			get
			{
				return robj.Members
						.Select(m => CreateMember().With(m))
						.OrderBy(MvcConfig.MemberOrderExtractor.Extract(this))
						.ToList();
			}
		}

		public List<MemberViewModel> SimpleMembers
		{
			get
			{
				return Members
					//TODO to mvcconfig
						.Where(m => !m.IsList)
						.ToList();
			}
		}

		public List<MemberViewModel> TableMembers
		{
			get
			{
				return Members
					//TODO to mvcconfig
						.Where(m => m.IsList)
						.ToList();
			}
		}

		public List<OperationViewModel> Operations
		{
			get
			{
				return robj.Operations
						.Select(op => CreateOperation().With(op))
						.OrderBy(MvcConfig.OperationOrderExtractor.Extract(this))
						.ToList();
			}
		}

		public List<OperationViewModel> SimpleOperations
		{
			get
			{
				return Operations
						.Where(o => !o.HasParameter)
						.ToList();
			}
		}

		public ParameterViewModel.OptionViewModel Option{ get { return new ParameterViewModel.OptionViewModel(robj); } }

		public bool MarkedAs(string mark)
		{
			return robj.MarkedAs(mark);
		}

		public VariableViewModel Perform(string operationModelId, Dictionary<string, string> parameterDictionary)
		{
			if(parameterDictionary == null) {parameterDictionary = new Dictionary<string, string>();}

			var rop = robj.Operations.Single(o => o.Id == operationModelId);
			var rparams = rop.Parameters;

			var parameters = new List<Rvariable>();
			foreach(var item in parameterDictionary)
			{
				var rparam = rparams.Single(p => p.Id == item.Key);

				var robjs = item.Value.Trim().Split(MvcConfig.ListValueSeparator).Select(id => {
					//TODO split char workaround cozulmeli...
					if(id.Contains("|"))
					{
						return Robj(id.Split('|')[0], id.Split('|')[1]);
					}

					return Robj(id, rparam.ViewModelId);
				});

				parameters.Add(rparam.CreateVariable(robjs.ToArray()));
			}

			var result = robj.Perform(operationModelId, parameters);

			return CreateVariable().With(result);
		}
	}

	public static class UrlHelper_ObjectViewModelExtensions
	{
		public static string Route(this UrlHelper source, ObjectViewModel model)
		{
			if(model == null) {return null;}
			return source.RouteUrl(model.ViewRouteName, model.RouteValues);
		}
	}
}
