using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Routine.Client;
using Routine.Core.Rest;

namespace Routine.Ui
{
	public class ObjectViewModel : ViewModelBase
	{
		public Robject Object { get; private set; }

		public ObjectViewModel(IMvcConfiguration configuration, Robject @object)
			: base(configuration)
		{
			Object = @object;
		}
		public string ViewModelId { get { return Object.Type.Id; } }
		public string Module { get { return Object.Type.Module; } }

		public string Title { get { return Object.IsNull ? Configuration.GetNullDisplayValue() : Object.Value; } }

		//TODO get from mvc configuration
		public bool HasDetail { get { return !Object.IsNull && !Object.Type.IsValueType && (Object.Type.Members.Any() || Object.Type.Operations.Any()); } }

		private string ViewRouteNameBase { get { return Configuration.GetViewRouteName(this); } }
		public string ViewRouteName { get { return ViewRouteNameBase; } }

		private string PerformRouteNameBase { get { return Configuration.GetPerformRouteName(this); } }
		public string PerformRouteName { get { return Object.IsNaked ? PerformRouteNameBase : PerformRouteNameBase + "As"; } }

		public RouteValueDictionary RouteValues
		{
			get
			{
				return new RouteValueDictionary(new { id = Object.Id, modelId = Object.ActualType.Id });
			}
		}

		public RouteValueDictionary RouteValuesIncludingViewModelId
		{
			get
			{
				if (Object.IsNaked) { return RouteValues; }

				return new RouteValueDictionary(new { id = Object.Id, actualModelId = Object.ActualType.Id, viewModelId = Object.ViewType.Id });
			}
		}

		public MenuViewModel Menu
		{
			get
			{
				var menuObjs = new List<Robject>();

				foreach (var type in Object.Type.Application.Types)
				{
					menuObjs.AddRange(Configuration
						.GetMenuIds(type)
						.Select(id => Object.Type.Application.Get(id, type.Id))
					);
				}

				return new MenuViewModel(Configuration, menuObjs);
			}
		}

		public bool HasOperation { get { return Object.Type.Operations.Any(); } }
		public bool HasSimpleMember { get { return Object.Type.Members.Any(m => !m.IsList); } }
		public bool HasTableMember { get { return Object.Type.Members.Any(m => m.IsList); } }

		private List<MemberViewModel> Members
		{
			get
			{
				return Object.MemberValues
						.Select(m => new MemberViewModel(Configuration, m))
						.Where(m => Configuration.IsRendered(m))
						.OrderBy(m => Configuration.GetMemberOrder(m))
						.ToList();
			}
		}

		public List<MemberViewModel> SimpleMembers
		{
			get
			{
				return Members
						.Where(m => m.IsSimple)
						.ToList();
			}
		}

		public List<MemberViewModel> TableMembers
		{
			get
			{
				return Members
						.Where(m => m.IsTable)
						.ToList();
			}
		}

		public List<OperationViewModel> Operations
		{
			get
			{
				return Object.Type.Operations
						.Select(o => new OperationViewModel(Configuration, Object, o))
						.Where(o => Configuration.IsRendered(o))
						.OrderBy(o => Configuration.GetOperationOrder(o))
						.ToList();
			}
		}

		public List<OperationViewModel> SimpleOperations
		{
			get
			{
				return Operations
						.Where(o => o.IsSimple)
						.ToList();
			}
		}

		public ParameterViewModel.OptionViewModel Option { get { return new ParameterViewModel.OptionViewModel(Object); } }

		public bool MarkedAs(string mark)
		{
			return Object.Type.MarkedAs(mark);
		}

		public VariableViewModel Perform(string operationModelId, Dictionary<string, string> parameterDictionary)
		{
			if (parameterDictionary == null) { parameterDictionary = new Dictionary<string, string>(); }

			var rop = Object.Type.Operations.Single(o => o.Id == operationModelId);
			var rparams = rop.Parameters;

			var parameters = new List<Rvariable>();
			foreach (var item in parameterDictionary)
			{
				var rparam = rparams.Single(p => p.Id == item.Key);

				var robjs = item.Value.Trim().Split(Configuration.GetListValueSeparator()).Select(id =>
				{
					try
					{
						var ord = SerializationExtensions.DeserializeObjectReferenceData(id);

						return Object.Type.Application.Get(ord.Id, ord.ActualModelId);
					}
					catch (ArgumentException)
					{
						return Object.Type.Application.Get(id, rparam.ParameterType.Id);
					}
				});

				parameters.Add(rparam.CreateVariable(robjs.ToArray()));
			}

			var result = Object.Perform(operationModelId, parameters);

			return new VariableViewModel(Configuration, result);
		}
	}

	public static class UrlHelper_ObjectViewModelExtensions
	{
		public static string Route(this UrlHelper source, ObjectViewModel model)
		{
			if (model == null) { return null; }
			return source.RouteUrl(model.ViewRouteName, model.RouteValues);
		}
	}
}
