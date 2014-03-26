using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Routine.Core.Service;
using Routine.Core.Service.Impl;

namespace Routine.Core
{
	public class DomainOperation
	{
		private readonly ICoreContext ctx;

		public DomainOperation(ICoreContext ctx)
		{
			this.ctx = ctx;
		}

		private DomainType domainType;
		private IOperation operation;

		public Dictionary<string, DomainParameter> Parameter{ get; private set;}
		public ICollection<DomainParameter> Parameters{get{return Parameter.Values;}}

		public string Id { get; private set; }
		public Marks Marks { get; private set; }
		public bool ResultIsVoid{ get; private set;}
		public bool ResultIsList{ get; private set;}
		public string ResultViewModelId{ get; private set;}
		public bool IsHeavy{ get; private set;}

		public DomainOperation For(DomainType domainType, IOperation operation)
		{
			this.domainType = domainType;
			this.operation = operation;
			Parameter = new Dictionary<string, DomainParameter>();

			Id = operation.Name;
			Marks = new Marks(ctx.CodingStyle.OperationMarkSelector.Select(operation));
			ResultIsVoid = operation.ReturnType.IsVoid;
			ResultIsList = operation.ReturnType.CanBeCollection();
			ResultViewModelId = ctx.CodingStyle.ModelIdSerializer.Serialize(ResultIsList ?operation.ReturnType.GetItemType():operation.ReturnType);
			IsHeavy = ctx.CodingStyle.OperationIsHeavyExtractor.Extract(operation);

			foreach(var parameter in operation.Parameters)
			{
				Parameter.Add(parameter.Name, ctx.CreateDomainParameter(this, parameter));
			}

			return this;
		}

		public bool MarkedAs(string mark)
		{
			return Marks.Has(mark);
		}

		public OperationModel GetModel()
		{
			return new OperationModel {
				Id = Id,
				Marks = Marks.List,
				IsHeavy = IsHeavy,
				Result = new ResultModel {
					IsList = ResultIsList,
					IsVoid = ResultIsVoid,
					ViewModelId = ResultViewModelId
				},
				Parameters = Parameters.Select(p => p.GetModel()).ToList()
			};
		}

		public OperationData CreateData(object target)
		{
			return new OperationData {
				ModelId = Id,
				IsAvailable = ctx.CodingStyle.OperationIsAvailableExtractor.Extract(target, operation),
				Parameters = Parameters.Select(p => p.CreateData(target)).ToList()
			};
		}

		public ResultData Perform(object target, List<ParameterValueData> parameterValues)
		{
			if (parameterValues == null) { parameterValues = new List<ParameterValueData>(); }

			var parameters = new object[Parameter.Count];
			foreach(var parameterData in parameterValues)
			{
				DomainParameter parameter;
				if(!Parameter.TryGetValue(parameterData.ParameterModelId, out parameter))
				{
					continue;
				}

				if(parameter.IsList)
				{
					var parameterValue = parameter.Type.CreateInstance() as IList;
					foreach (var parameterReference in parameterData.Value.References)
					{
						parameterValue.Add(ctx.Locate(parameterReference));
					}
					parameters[parameter.Index] = parameterValue;
				}
				else if(parameterData.Value.References.Any())
				{
					parameters[parameter.Index] = ctx.Locate(parameterData.Value.References[0]);
				}
			}

			var resultValue = operation.PerformOn(target, parameters);

			var result = new ResultData();
			if(!ResultIsVoid)
			{
				result.Value = ctx.CreateValueData(resultValue, ResultIsList, ResultViewModelId);
			}

			return result;
		}
	}
}
