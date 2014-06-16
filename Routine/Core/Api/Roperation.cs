using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core;

namespace Routine.Core.Api
{
	public class Roperation
	{
		private readonly IApiContext context;

        public Roperation(IApiContext context)
		{
			this.context = context;
		}

		private Robject parentObject;
		private OperationModel model;
		private Dictionary<string, Rparameter> parameters;
		private List<List<Rparameter>> groups;

		internal Roperation With(Robject parentObject, OperationModel model)
		{
			this.parentObject = parentObject;
			this.model = model;
			this.parameters = new Dictionary<string, Rparameter>();
			this.groups = Enumerable.Range(0, model.GroupCount).Select(i => new List<Rparameter>()).ToList();

			foreach(var parameter in model.Parameters)
			{
				parameters[parameter.Id] = context.CreateRparameter().With(this, parameter);
			}

			foreach (var paramId in parameters.Keys)
			{
				var param = parameters[paramId];

				foreach (var group in param.Groups)
				{
					groups[group].Add(param);
				}
			}

			return this;
		}

		public Robject Object{get{return parentObject;}}
		public string Id {get{return model.Id;}}
		public List<Rparameter> Parameters{get{return parameters.Values.ToList();}}
		public bool ResultIsVoid{get{return model.Result.IsVoid;}}
		public bool ResultIsList{get{return model.Result.IsList;}}

		public List<List<Rparameter>> Groups { get { return groups; } }

		public bool MarkedAs(string mark)
		{
			return model.Marks.Any(m => m == mark);
		}

		public Rvariable Perform(List<Rvariable> parameterVariables)
		{
			var parameterValues = new Dictionary<string, ParameterValueData>();
			foreach(var parameterVariable in parameterVariables)
			{
				var rparam = parameters[parameterVariable.Name];
				var parameterValue = rparam.CreateParameterValueData(parameterVariable.List);
				parameterValues.Add(rparam.Id, parameterValue);
			}

            var resultData = context.ObjectService.PerformOperation(parentObject.ObjectReferenceData, model.Id, parameterValues);

			if(ResultIsVoid)
			{
                return context.CreateRvariable().Void();
			}

            return context.CreateRvariable().With(resultData);
		}
	}
}
