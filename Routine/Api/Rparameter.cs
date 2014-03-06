using System.Collections.Generic;
using System.Linq;
using Routine.Core.Service;

namespace Routine.Api
{
	public class Rparameter
	{		
		private readonly IApiContext context;

		public Rparameter(IApiContext context)
		{
            this.context = context;
		}

		private Roperation parentOperation;
		private ParameterModel model;
		internal Rparameter With(Roperation parentOperation, ParameterModel model)
		{
			this.parentOperation = parentOperation;
			this.model = model;

			return this;
		}

		private ParameterData data;
		internal void SetData(ParameterData data)
		{
			this.data = data;
		}

		private void FetchDataIfNecessary()
		{
			if(data == null)
			{
				parentOperation.LoadOperation();
			}
		}

		public Roperation Operation{get{return parentOperation;}}
		public string ViewModelId {get{return model.ViewModelId;}}
		public string Id {get{return model.Id;}}
		public bool IsList {get{return model.IsList;}}

		public Rvariable CreateVariable(params Robject[] robjs) {return CreateVariable(robjs.ToList());}
		public Rvariable CreateVariable(List<Robject> robjs)
		{
            var result = context.CreateRvariable().WithList(Id, robjs);

			if(!IsList) 
			{
				return result.ToSingle();
			}

			return result;
		}

		internal ParameterValueData CreateParameterValueData(params Robject[] robjs) {return CreateParameterValueData(robjs.ToList());}
		internal ParameterValueData CreateParameterValueData(List<Robject> robjs)
		{
			var result = new ParameterValueData();

			result.ParameterModelId = Id;
			result.Value = CreateReferenceData(robjs);

			return result;
		}

		private ReferenceData CreateReferenceData(params Robject[] robjs) { return CreateReferenceData(robjs.ToList());}
		private ReferenceData CreateReferenceData(List<Robject> robjs)
		{
			var result = new ReferenceData();
			result.IsList = IsList;
			foreach(var robj in robjs)
			{
				result.References.Add(new ObjectReferenceData {
					Id = robj.Id,
					ActualModelId = robj.ActualModelId,
					ViewModelId = model.ViewModelId,
					IsNull = robj.IsNull
				});
			}
			return result;
		}

		internal void Invalidate()
		{
			data = null;
		}
	}
}
