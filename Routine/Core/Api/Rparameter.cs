using System.Collections.Generic;
using System.Linq;
using Routine.Core;

namespace Routine.Core.Api
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

		public Roperation Operation { get { return parentOperation; } }
		public string ViewModelId { get { return model.ViewModelId; } }
		public string Id { get { return model.Id; } }
		public bool IsList { get { return model.IsList; } }
		
		internal List<int> Groups { get { return model.Groups; } }

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

		internal ParameterValueData CreateParameterValueData(params Robject[] robjs) { return CreateParameterValueData(robjs.ToList()); }
		internal ParameterValueData CreateParameterValueData(List<Robject> robjs)
		{
			return new ParameterValueData
			{
				IsList = IsList,
				Values = robjs.Select(robj => robj.ParameterData).ToList()
			};
		}
	}
}
