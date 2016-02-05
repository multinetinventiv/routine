using System.Collections.Generic;
using System.Linq;
using Routine.Core;

namespace Routine.Engine
{
	public class DomainObjectInitializer
	{
		private readonly ICoreContext ctx;
		private readonly List<DomainParameterGroup<IConstructor>> groups;

		public Dictionary<string, DomainParameter> Parameter { get; private set; }
		public ICollection<DomainParameter> Parameters { get { return Parameter.Values; } }

		public Marks Marks { get; private set; }

		public DomainObjectInitializer(ICoreContext ctx, IConstructor constructor)
		{
			this.ctx = ctx;

			groups = new List<DomainParameterGroup<IConstructor>>();
			Parameter = new Dictionary<string, DomainParameter>();

			Marks = new Marks();

			AddGroup(constructor);
		}

		public void AddGroup(IConstructor constructor)
		{
			if (groups.Any() &&
			    !constructor.InitializedType.Equals(groups.Last().Parametric.InitializedType))
			{
				throw new InitializedTypeDoNotMatchException(constructor, groups.Last().Parametric.InitializedType, constructor.InitializedType);
			}

			if (groups.Any(g => g.ContainsSameParameters(constructor)))
			{
				throw new IdenticalSignatureAlreadyAddedException(constructor);
			}

			foreach (var parameter in constructor.Parameters)
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

			groups.Add(new DomainParameterGroup<IConstructor>(constructor, Parameters.Where(p => p.Groups.Contains(groups.Count)), groups.Count));

			Marks.Join(ctx.CodingStyle.GetMarks(constructor));
		}

		public object Initialize(Dictionary<string, ParameterValueData> parameterValues)
		{
			var resolution = new DomainParameterResolver<IConstructor>(groups, parameterValues).Resolve();

			var result = resolution.Result.Initialize(resolution.Parameters);

			return result;
		}

		public InitializerModel GetModel()
		{
			return new InitializerModel {
				Marks = Marks.List,
				GroupCount = groups.Count,
				Parameters = Parameters.Select(p => p.GetModel()).ToList()
			};
		}
	}
}
