using System;
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
		private List<IOperation> operations;

		public Dictionary<string, DomainParameter> Parameter{ get; private set;}
		public ICollection<DomainParameter> Parameters{get{return Parameter.Values;}}

		public string Id { get; private set; }
		public Marks Marks { get; private set; }
		public bool ResultIsVoid { get; private set; }
		public bool ResultIsList { get; private set; }
		public string ResultViewModelId { get; private set; }

		public DomainOperation For(DomainType domainType, IOperation operation)
		{
			this.domainType = domainType;
			this.operations = new List<IOperation>();
			Parameter = new Dictionary<string, DomainParameter>();

			Id = operation.Name;
			Marks = new Marks(ctx.CodingStyle.OperationMarkSelector.Select(operation));
			ResultIsVoid = operation.ReturnType.IsVoid;
			ResultIsList = operation.ReturnType.CanBeCollection();
			ResultViewModelId = ctx.CodingStyle.ModelIdSerializer.Serialize(ResultIsList ? operation.ReturnType.GetItemType() : operation.ReturnType);

			foreach(var parameter in operation.Parameters)
			{
				Parameter.Add(parameter.Name, ctx.CreateDomainParameter(parameter, operations.Count));
			}

			operations.Add(operation);

			return this;
		}

		public void AddGroup(IOperation operation)
		{
			if (operation.ReturnType != operations.Last().ReturnType) { throw new ReturnTypesDoNotMatchException(operations.Last().ReturnType, operation.ReturnType); }

			foreach (var parameter in operation.Parameters)
			{
				if (Parameter.ContainsKey(parameter.Name))
				{
					Parameter[parameter.Name].AddGroup(parameter, operations.Count);
				}
				else
				{
					Parameter.Add(parameter.Name, ctx.CreateDomainParameter(parameter, operations.Count));
				}
			}

			operations.Add(operation);

			Marks.Join(ctx.CodingStyle.OperationMarkSelector.Select(operation));
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
				GroupCount = operations.Count,
				Parameters = Parameters.Select(p => p.GetModel()).ToList(),
				Result = new ResultModel {
					IsList = ResultIsList,
					IsVoid = ResultIsVoid,
					ViewModelId = ResultViewModelId
				}
			};
		}

		public ValueData Perform(object target, Dictionary<string, ParameterValueData> parameterValues)
		{
			var resolution = new DomainParameterResolver<IOperation>(operations, Parameter, parameterValues).Resolve();

			var resultValue = resolution.Result.PerformOn(target, resolution.Parameters);

			if(ResultIsVoid)
			{
				return new ValueData();
			}

			return ctx.CreateValueData(resultValue, ResultIsList, ResultViewModelId, true);
		}
	}

	public class ReturnTypesDoNotMatchException : Exception 
	{
		public ReturnTypesDoNotMatchException(TypeInfo expected, TypeInfo actual) 
			: base(string.Format("Operation's expected return type is {0}, but given operation has a return type of {1}", expected, actual)) { }
	}
}
