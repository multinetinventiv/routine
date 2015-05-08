using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core;
using Routine.Core.Configuration;

namespace Routine.Engine
{
	public class DomainOperation
	{
		private readonly ICoreContext ctx;
		private readonly Lazy<DomainType> lazyResultType;
		private readonly List<DomainParameterGroup<IOperation>> groups;

		public Dictionary<string, DomainParameter> Parameter { get; private set; }
		public ICollection<DomainParameter> Parameters { get { return Parameter.Values; } }

		public string Id { get; private set; }
		public Marks Marks { get; private set; }
		public bool ResultIsVoid { get; private set; }
		public bool ResultIsList { get; private set; }
		public DomainType ResultType { get { return lazyResultType.Value; } }

		public DomainOperation(ICoreContext ctx, IOperation operation)
		{
			this.ctx = ctx;

			groups = new List<DomainParameterGroup<IOperation>>();

			Id = operation.Name;
			Marks = new Marks();
			ResultIsVoid = operation.ReturnType.IsVoid;
			ResultIsList = operation.ReturnType.CanBeCollection();
			Parameter = new Dictionary<string, DomainParameter>();

			var returnType = ResultIsList ? operation.ReturnType.GetItemType() : operation.ReturnType;

			try
			{
				ctx.CodingStyle.GetTypeId(returnType); //To eagerly check if type is configured
				lazyResultType = new Lazy<DomainType>(() => ctx.GetDomainType(returnType));
			}
			catch (ConfigurationException ex)
			{
				throw new TypeNotConfiguredException(returnType, ex);
			}

			AddGroup(operation);
		}

		public void AddGroup(IOperation operation)
		{
			if (groups.Any() &&
			    !operation.ReturnType.Equals(groups.Last().Parametric.ReturnType))
			{
				throw new ReturnTypesDoNotMatchException(operation, groups.Last().Parametric.ReturnType, operation.ReturnType);
			}

			if (groups.Any(g => g.ContainsSameParameters(operation)))
			{
				throw new IdenticalSignatureAlreadyAddedException(operation);
			}

			foreach (var parameter in operation.Parameters)
			{
				if (Parameter.ContainsKey(parameter.Name))
				{
					Parameter[parameter.Name].AddGroup(parameter, groups.Count);
				}
				else
				{
					Parameter.Add(parameter.Name, new DomainParameter(ctx, parameter, groups.Count));
				}
			}

			groups.Add(new DomainParameterGroup<IOperation>(operation, Parameters.Where(p => p.Groups.Contains(groups.Count)), groups.Count));

			Marks.Join(ctx.CodingStyle.GetMarks(operation));
		}

		public bool MarkedAs(string mark)
		{
			return Marks.Has(mark);
		}

		public OperationModel GetModel()
		{
			return new OperationModel
			{
				Id = Id,
				Marks = Marks.List,
				GroupCount = groups.Count,
				Parameters = Parameters.Select(p => p.GetModel()).ToList(),
				Result = new ResultModel
				{
					IsList = ResultIsList,
					IsVoid = ResultIsVoid,
					ViewModelId = ResultType.Id
				}
			};
		}

		public ValueData Perform(object target, Dictionary<string, ParameterValueData> parameterValues)
		{
			var resolution = new DomainParameterResolver<IOperation>(groups, parameterValues).Resolve();

			var resultValue = resolution.Result.PerformOn(target, resolution.Parameters);

			if (ResultIsVoid)
			{
				return new ValueData();
			}

			return ctx.CreateValueData(resultValue, ResultIsList, ResultType, true);
		}

		internal void LoadSubTypes()
		{
			foreach (var parameter in Parameters)
			{
				parameter.LoadSubTypes();
			}

			//to force type to load
			var type = lazyResultType.Value;
		}

		#region Formatting & Equality

		protected bool Equals(DomainOperation other)
		{
			return string.Equals(Id, other.Id);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((DomainOperation)obj);
		}

		public override int GetHashCode()
		{
			return (Id != null ? Id.GetHashCode() : 0);
		}

		public override string ToString()
		{
			return string.Format("Id: {0}", Id);
		}

		#endregion
	}
}
