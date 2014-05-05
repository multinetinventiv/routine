using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

		public ValueData Perform(object target, Dictionary<string, ReferenceData> parameterValues)
		{
			if (parameterValues == null) { parameterValues = new Dictionary<string, ReferenceData>(); }

			var parameters = new object[Parameter.Count];
			foreach(var parameterModelId in parameterValues.Keys)
			{
				var parameterData = parameterValues[parameterModelId];
				DomainParameter parameter;
				if (!Parameter.TryGetValue(parameterModelId, out parameter))
				{
					continue;
				}

				if(parameter.IsList)
				{
					var parameterValue = parameter.Type.CreateInstance() as IList;
					foreach (var parameterReference in parameterData.References)
					{
						parameterValue.Add(ctx.Locate(parameterReference));
					}
					parameters[parameter.Index] = parameterValue;
				}
				else if(parameterData.References.Any())
				{
					parameters[parameter.Index] = ctx.Locate(parameterData.References[0]);
				}
			}

			var resultValue = operation.PerformOn(target, parameters);

			if(ResultIsVoid)
			{
				return new ValueData();
			}

			return ctx.CreateValueData(resultValue, ResultIsList, ResultViewModelId, true);
		}
	}
}
