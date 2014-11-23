using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core;

namespace Routine.Engine
{
	public class DomainObjectInitializer
	{
		private readonly ICoreContext ctx;
		private readonly List<DomainParameterGroup<IInitializer>> groups;

		public Dictionary<string, DomainParameter> Parameter { get; private set; }
		public ICollection<DomainParameter> Parameters { get { return Parameter.Values; } }

		public Marks Marks { get; private set; }

		public DomainObjectInitializer(ICoreContext ctx, IInitializer initializer)
		{
			this.ctx = ctx;

			groups = new List<DomainParameterGroup<IInitializer>>();
			Parameter = new Dictionary<string, DomainParameter>();

			Marks = new Marks();

			AddGroup(initializer);
		}

		public void AddGroup(IInitializer initializer)
		{
			if (groups.Any() && !initializer.InitializedType.Equals(groups.Last().Parametric.InitializedType)) { throw new InitializedTypeDoNotMatchException(initializer, groups.Last().Parametric.InitializedType, initializer.InitializedType); }

			foreach (var parameter in initializer.Parameters)
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

			groups.Add(new DomainParameterGroup<IInitializer>(initializer, Parameters.Where(p => p.Groups.Contains(groups.Count)), groups.Count));

			Marks.Join(ctx.CodingStyle.GetMarks(initializer));
		}

		public object Initialize(Dictionary<string, ParameterValueData> parameterValues)
		{
			var resolution = new DomainParameterResolver<IInitializer>(groups, parameterValues).Resolve();

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

	public class InitializedTypeDoNotMatchException : Exception
	{
		public InitializedTypeDoNotMatchException(IInitializer initializer, IType expected, IType actual)
			: base(string.Format("{0}.{1}: Expected initialized type is {2}, but given initialized type is {3}", initializer.ParentType.Name, initializer.Name, expected, actual)) { }
	}
}
