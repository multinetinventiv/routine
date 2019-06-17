using System;
using System.Collections.Generic;
using System.Linq;
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

		public override string SpecificViewName { get { return Object.IsNull ? null : Object.Type.Name; } }

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

				return Object.Display;
			}
		}

		public bool HasDetail
		{
			get
			{
				return !Object.IsNull && !Object.Type.IsValueType && Configuration.GetHasDetail(this);
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
		public bool HasData { get { return GetDatas().Any(); } }

		public List<DataViewModel> GetDatas()
		{
			return Object.DataValues
					.Select(m => new DataViewModel(Configuration, m))
					.Where(m => m.IsRendered)
					.OrderBy(m => m.GetOrder())
					.ToList();
		}

		public List<DataViewModel> GetDatas(DataLocations dataLocations)
		{
			return GetDatas()
					.Where(m => m.Is(dataLocations))
					.OrderBy(m => m.GetOrder(dataLocations))
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

		public VariableViewModel Perform(string operationName, Dictionary<string, string> parameterDictionary)
		{
			if (parameterDictionary == null) { parameterDictionary = new Dictionary<string, string>(); }
			if (Object.IsNull)
			{
				return new VariableViewModel(Configuration, new Rvariable());
			}

			var rop = Object.Type.Operations.Single(o => o.Name == operationName);
			var rparams = rop.Parameters;

			var parameters = new List<Rvariable>();
			foreach (var item in parameterDictionary)
			{
				var rparam = rparams.SingleOrDefault(p => p.Name == item.Key);

				if (rparam == null) { continue; }

				var robjs = item.Value.Trim().Split(Configuration.GetListValueSeparator()).Select(id => Object.Type.Application.Get(id, rparam.ParameterType.Id));

				parameters.Add(rparam.CreateVariable(robjs.ToArray()));
			}

			var result = Object.Perform(operationName, parameters);

			return new VariableViewModel(Configuration, result);
		}
	}
}
