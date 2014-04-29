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

		internal ReferenceData CreateReferenceData(params Robject[] robjs) { return CreateReferenceData(robjs.ToList());}
		internal ReferenceData CreateReferenceData(List<Robject> robjs)
		{
			return new ReferenceData
			{
				IsList = IsList,
				References = robjs.Select(robj =>
					new ObjectReferenceData
						{
							Id = robj.ObjectReferenceData.Id,
							ActualModelId = robj.ObjectReferenceData.ActualModelId,
							ViewModelId = ViewModelId,
							IsNull = robj.ObjectReferenceData.IsNull
						}).ToList()
			};
		}
	}
}
