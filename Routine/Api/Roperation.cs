using System.Collections.Generic;
using System.Linq;
using Routine.Core.Service;

namespace Routine.Api
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

		internal Roperation With(Robject parentObject, OperationModel model)
		{
			this.parentObject = parentObject;
			this.model = model;
			this.parameters = new Dictionary<string, Rparameter>();
			foreach(var parameter in model.Parameters)
			{
				parameters[parameter.Id] = context.CreateRparameter().With(this, parameter);
			}

			return this;
		}

		public Robject Object{get{return parentObject;}}
		public string Id {get{return model.Id;}}
		public List<Rparameter> Parameters{get{return parameters.Values.ToList();}}
		public bool ResultIsVoid{get{return model.Result.IsVoid;}}
		public bool ResultIsList{get{return model.Result.IsList;}}

		public bool MarkedAs(string mark)
		{
			return model.Marks.Any(m => m == mark);
		}

		public Rvariable Perform(List<Rvariable> parameterVariables)
		{
			var parameterValues = new Dictionary<string, ReferenceData>();
			foreach(var parameterVariable in parameterVariables)
			{
				var rparam = parameters[parameterVariable.Name];
				var parameterValue = rparam.CreateReferenceData(parameterVariable.List);
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
