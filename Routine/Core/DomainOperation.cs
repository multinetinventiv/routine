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
				Parameter.Add(parameter.Name, ctx.CreateDomainParameter(this, parameter, operations.Count));
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
					Parameter.Add(parameter.Name, ctx.CreateDomainParameter(this, parameter, operations.Count));
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
				GroupCount = operations.Count,
				Marks = Marks.List,
				Result = new ResultModel {
					IsList = ResultIsList,
					IsVoid = ResultIsVoid,
					ViewModelId = ResultViewModelId
				},
				Parameters = Parameters.Select(p => p.GetModel()).ToList()
			};
		}

		public ValueData Perform(object target, Dictionary<string, ParameterValueData> parameterValues)
		{
			object resultValue = new PerformOperation(this, target, parameterValues).Do();

			if(ResultIsVoid)
			{
				return new ValueData();
			}

			return ctx.CreateValueData(resultValue, ResultIsList, ResultViewModelId, true);
		}

		private class PerformOperation
		{
			private DomainOperation owner;
			private object target;
			private Dictionary<string, ParameterValueData> values;

			public PerformOperation(DomainOperation owner, object target, Dictionary<string, ParameterValueData> values)
			{
				if (values == null) { values = new Dictionary<string, ParameterValueData>(); }

				this.owner = owner;
				this.target = target;
				this.values = values;
			}

			internal object Do()
			{
				var parameterValues = GetParameterValues();

				var foundOperation = FindMostCompatibleOperation(parameterValues);

				var parameterArray = PrepareParameters(foundOperation, parameterValues);

				return foundOperation.PerformOn(target, parameterArray);
			}

			private List<DomainParameterValue> GetParameterValues()
			{
				var parameters = new List<DomainParameterValue>();
				foreach (var parameterModelId in values.Keys)
				{
					var parameterValueData = values[parameterModelId];
					DomainParameter parameter;
					if (!owner.Parameter.TryGetValue(parameterModelId, out parameter))
					{
						continue;
					}

					parameters.Add(new DomainParameterValue(parameter, parameter.Locate(parameterValueData)));
				}
				return parameters;
			}

			private IOperation FindMostCompatibleOperation(List<DomainParameterValue> parameterValues)
			{
				if (owner.operations.Count == 1)
				{
					return owner.operations[0];
				}
				
				var exactMatch = owner.operations.SingleOrDefault(ThatMatchesExactlyWith(parameterValues));

				if (exactMatch != null)
				{
					return exactMatch;
				}

				var foundOperations = FindOperationsWithMostMatchedParameters(parameterValues);

				if (foundOperations.Count == 1)
				{
					return foundOperations[0];
				}

				return GetFirstOperationWithLeastNonMatchedParameters(foundOperations, parameterValues);
			}

			private Func<IOperation, bool> ThatMatchesExactlyWith(List<DomainParameterValue> parameterValues)
			{
				return o =>
					o.Parameters.Count == parameterValues.Count &&
					o.Parameters.All(p => parameterValues.Any(pv => pv.Parameter.Id == p.Name));
			}

			private List<IOperation> FindOperationsWithMostMatchedParameters(List<DomainParameterValue> parameterValues)
			{
				var result = new List<IOperation>();

				int matchCount = int.MinValue;

				foreach (var operation in owner.operations.OrderByDescending(o => o.Parameters.Count))
				{
					int tempCount = operation.Parameters.Count(p => parameterValues.Any(pv => pv.Parameter.Id == p.Name));

					if (tempCount > matchCount)
					{
						result.Clear();
						result.Add(operation);
						matchCount = tempCount;
					}
					else if (tempCount == matchCount)
					{
						result.Add(operation);
					}
				}

				return result;
			}

			private IOperation GetFirstOperationWithLeastNonMatchedParameters(List<IOperation> foundOperations, List<DomainParameterValue> parameterValues)
			{
				IOperation result = null;

				int nonMatchCount = int.MaxValue;

				foreach (var operation in foundOperations.OrderByDescending(o => o.Parameters.Count))
				{
					int tempCount = operation.Parameters.Count(p => parameterValues.All(pv => pv.Parameter.Id != p.Name));

					if (tempCount < nonMatchCount)
					{
						result = operation;
						nonMatchCount = tempCount;
					}
				}

				return result;
			}

			private object[] PrepareParameters(IOperation operation, List<DomainParameterValue> parameterValues)
			{
				var result = new object[operation.Parameters.Count];

				foreach (var parameter in operation.Parameters)
				{
					var parameterValue = parameterValues.SingleOrDefault(p => p.Parameter.Id == parameter.Name);

					if (parameterValue == null) { continue; }

					result[parameter.Index] = parameterValue.Value;
				}

				return result;
			}
		}

		private class DomainParameterValue
		{
			public DomainParameter Parameter { get; private set; }
			public object Value { get; private set; }

			public DomainParameterValue(DomainParameter parameter, object value)
			{
				Parameter = parameter;
				Value = value;
			}
		}
	}

	public class ReturnTypesDoNotMatchException : Exception 
	{
		public ReturnTypesDoNotMatchException(TypeInfo expected, TypeInfo actual) 
			: base(string.Format("Operation's expected return type is {0}, but given operation has a return type of {1}", expected, actual)) { }
	}
}
