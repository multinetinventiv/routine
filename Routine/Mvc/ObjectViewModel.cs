using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Routine.Api;

namespace Routine.Mvc
{
	public class ObjectViewModel : ViewModelBase
	{
		public ObjectViewModel(IMvcConfiguration mvcConfig, IFactory factory)
			: base(mvcConfig, factory) {}

		private Robject robj;

		internal ObjectViewModel With(Robject robj)
		{
			this.robj = robj;

			return this;
		}

		public string ViewModelId {get{return robj.ViewModelId;}}

		public string Title { get { return robj.IsNull?MvcConfig.NullDisplayValue:robj.Value; } }
		public bool HasDetail { get { return !robj.IsNull && robj.IsDomain && (robj.Members.Any() || robj.Operations.Any()); } }

		private string ViewRouteNameBase{get{return MvcConfig.ViewRouteNameExtractor.Extract(this);}}
		public string ViewRouteName { get {return robj.IsNaked?ViewRouteNameBase:ViewRouteNameBase+"As";} }

		private string PerformRouteNameBase{get{return MvcConfig.PerformRouteNameExtractor.Extract(this);}}
		public string PerformRouteName { get {return robj.IsNaked?PerformRouteNameBase:PerformRouteNameBase+"As";} }

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

		public RouteValueDictionary RouteValues
		{
			get
			{
				if(robj.IsNaked)
				{
					return new RouteValueDictionary(new {id = robj.Id, modelId = robj.ActualModelId});
				}

				return new RouteValueDictionary(new {id = robj.Id, actualModelId = robj.ActualModelId, viewModelId = robj.ViewModelId});
			}
		}

		private List<OperationViewModel> Operations
		{
			get
			{
				return robj.Operations
						.Select(op => CreateOperation().With(op))
						.OrderBy(MvcConfig.OperationOrderExtractor.Extract(this))
						.ToList();
			}
		}

		public List<OperationViewModel> OperationMenu
		{
			get
			{				
				var result = new List<OperationViewModel>();

				var operations = Operations;

				var groups = MvcConfig.OperationGroupSelector.Select(this);

				foreach(var group in groups)
				{
					var groupOperations = operations.Where(group);

					if(groupOperations.Any())
					{
						if(result.Any())
						{
							result.Add(CreateOperation().Separator());
						}
						
						result.AddRange(groupOperations);
						operations = operations.Except(groupOperations).ToList();
					}
				}

				if(operations.Any())
				{
					if(result.Any())
					{
						result.Insert(0, CreateOperation().Separator());
					}

					result.InsertRange(0, operations);
				}

				return result;
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
			return CreateVariable().With(robj.Perform(operationModelId, parameters));
		}
	}

	public static class UrlHelper_ObjectViewModelExtensions
	{
		public static string Route(this UrlHelper source, ObjectViewModel model)
		{
			return source.RouteUrl(model.ViewRouteName, model.RouteValues);
		}
	}
}
