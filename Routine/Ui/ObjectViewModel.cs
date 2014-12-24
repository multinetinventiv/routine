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

		public string ViewModelId
		{
			get
			{
				if (Object.IsNull)
				{
					return string.Empty;
				}

				return Object.Type.Id;
			}
		}

		public string Module
		{
			get
			{
				if (Object.IsNull)
				{
					return string.Empty;
				}

				return Object.Type.Module;
			}
		}

		public string Title
		{
			get
			{
				if (Object.IsNull)
				{
					return Configuration.GetNullDisplayValue();
				}
				
				return Object.Value;
			}
		}

		public bool HasDetail
		{
			get
			{
				return !Object.IsNull && !Object.Type.IsValueType && Configuration.GetHasDetail(this);
			}
		}

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

		public bool HasOperation { get { return GetOperations().Any(); } }
		public bool HasMember { get { return GetMembers().Any(); } }

		public List<MemberViewModel> GetMembers()
		{
			return Object.MemberValues
					.Select(m => new MemberViewModel(Configuration, m))
					.Where(m => m.IsRendered)
					.OrderBy(m => m.GetOrder())
					.ToList();
		}

		public List<MemberViewModel> GetMembers(MemberTypes memberTypes)
		{
			return GetMembers()
					.Where(m => m.Is(memberTypes))
					.OrderBy(m => m.GetOrder(memberTypes))
					.ToList();
		}

		public List<OperationViewModel> GetOperations()
		{
			return Object.Type.Operations
					.Select(o => new OperationViewModel(Configuration, Object, o))
					.Where(o => o.IsRendered)
					.OrderBy(m => m.GetOrder())
					.ToList();
		}

		public List<OperationViewModel> GetOperations(OperationTypes operationTypes)
		{
			return GetOperations()
					.Where(o => o.Is(operationTypes))
					.OrderBy(m => m.GetOrder(operationTypes))
					.ToList();
		}

		public OptionViewModel Option { get { return new OptionViewModel(Configuration, Object); } }

		public bool MarkedAs(string mark)
		{
			if (Object.IsNull)
			{
				return false;
			}

			return Object.Type.MarkedAs(mark);
		}

		public VariableViewModel Perform(string operationModelId, Dictionary<string, string> parameterDictionary)
		{
			if (parameterDictionary == null) { parameterDictionary = new Dictionary<string, string>(); }
			if (Object.IsNull)
			{
				return new VariableViewModel(Configuration, new Rvariable());
			}

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
