using System.Collections.Generic;
using System.Linq;
using Routine.Core.Service;

namespace Routine.Api
{
	public class Roperation
	{
		private readonly IObjectService objectService;
		private readonly IFactory factory;

		public Roperation(IObjectService objectService, IFactory factory)
		{
			this.objectService = objectService;
			this.factory = factory;
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
				parameters[parameter.Id] = factory.Create<Rparameter>().With(this, parameter);
			}

			return this;
		}

		private OperationData data;
		internal void SetData(OperationData data)
		{
			this.data = data;
			foreach(var parameter in data.Parameters)
			{
				parameters[parameter.ModelId].SetData(parameter);
			}
		}

		private bool Available {get{FetchDataIfNecessary(); return data.IsAvailable;}}

		private void FetchDataIfNecessary()
		{
			if(data == null)
			{
				LoadOperation();
			}
		}

		internal void LoadOperation()
		{
			if(model.IsHeavy)
			{
				SetData(objectService.GetOperation(parentObject.ObjectReferenceData, model.Id));
			}
			else
			{
				parentObject.LoadObject();
			}
		}

		public Robject Object{get{return parentObject;}}
		public string Id {get{return model.Id;}}
		public List<Rparameter> Parameters{get{return parameters.Values.ToList();}}
		public bool ResultIsVoid{get{return model.Result.IsVoid;}}
		public bool ResultIsList{get{return model.Result.IsList;}}

		public bool IsAvailable(){return Available;}

		public Rvariable Perform(List<Rvariable> parameterVariables)
		{
			var parameterValues = new List<ParameterValueData>();
			foreach(var parameterVariable in parameterVariables)
			{
				var rparam = parameters[parameterVariable.Name];
				var parameterValue = rparam.CreateParameterValueData(parameterVariable.List);
				parameterValues.Add(parameterValue);
			}

			var resultData = objectService.PerformOperation(parentObject.ObjectReferenceData, model.Id, parameterValues);

			if(ResultIsVoid)
			{
				return factory.Create<Rvariable>().Void();
			}

			return factory.Create<Rvariable>().With(resultData.Value);
		}

		internal void Invalidate()
		{
			data = null;
			foreach(var parameter in parameters.Values)
			{
				parameter.Invalidate();
			}
		}
	}
	
}
