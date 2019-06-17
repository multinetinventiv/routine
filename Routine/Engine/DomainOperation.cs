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
		private readonly List<DomainParameterGroup<IMethod>> groups;

		public Dictionary<string, DomainParameter> Parameter { get; private set; }
		public ICollection<DomainParameter> Parameters { get { return Parameter.Values; } }

		public string Name { get; private set; }
		public Marks Marks { get; private set; }
		public bool ResultIsVoid { get; private set; }
		public bool ResultIsList { get; private set; }
		public DomainType ResultType { get; private set; }

		public DomainOperation(ICoreContext ctx, IMethod method)
		{
			this.ctx = ctx;

			groups = new List<DomainParameterGroup<IMethod>>();

			Name = ctx.CodingStyle.GetName(method);
			Marks = new Marks();
			ResultIsVoid = method.ReturnType.IsVoid;
			ResultIsList = method.ReturnType.CanBeCollection();
			Parameter = new Dictionary<string, DomainParameter>();

			var returnType = ResultIsList ? method.ReturnType.GetItemType() : method.ReturnType;

			if (!ctx.CodingStyle.ContainsType(returnType))
			{
				throw new TypeNotConfiguredException(returnType);
			}

			ResultType = ctx.GetDomainType(returnType);

			AddGroup(method);
		}

		public void AddGroup(IMethod method)
		{
			if (groups.Any() &&
			    !method.ReturnType.Equals(groups.Last().Parametric.ReturnType))
			{
				throw new ReturnTypesDoNotMatchException(method, groups.Last().Parametric.ReturnType, method.ReturnType);
			}

			if (groups.Any(g => g.ContainsSameParameters(method)))
			{
				throw new IdenticalSignatureAlreadyAddedException(method);
			}

			foreach (var parameter in method.Parameters)
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

			groups.Add(new DomainParameterGroup<IMethod>(method, Parameters.Where(p => p.Groups.Contains(groups.Count)), groups.Count));

			Marks.Join(ctx.CodingStyle.GetMarks(method));
		}

		public bool MarkedAs(string mark)
		{
			return Marks.Has(mark);
		}

		public OperationModel GetModel()
		{
			return new OperationModel
			{
				Name = Name,
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

		public VariableData Perform(object target, Dictionary<string, ParameterValueData> parameterValues)
		{
			var resolution = new DomainParameterResolver<IMethod>(groups, parameterValues).Resolve();

			var resultValue = resolution.Result.PerformOn(target, resolution.Parameters);

			if (ResultIsVoid)
			{
				return new VariableData();
			}

			return ctx.CreateValueData(resultValue, ResultIsList, ResultType, true);
		}

		#region Formatting & Equality

		protected bool Equals(DomainOperation other)
		{
			return string.Equals(Name, other.Name);
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
			return (Name != null ? Name.GetHashCode() : 0);
		}

		public override string ToString()
		{
			return string.Format("{1} {0}(...)", Name, ResultType);
		}

		#endregion
	}
}
