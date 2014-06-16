using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Routine.Core
{
	public class DomainObjectInitializer
	{
		private readonly ICoreContext ctx;

		public DomainObjectInitializer(ICoreContext ctx)
		{
			this.ctx = ctx;
		}

		private DomainType domainType;
		private List<IInitializer> initializers;

		public Dictionary<string, DomainParameter> Parameter { get; private set; }
		public ICollection<DomainParameter> Parameters { get { return Parameter.Values; } }

		public Marks Marks { get; private set; }

		public DomainObjectInitializer For(DomainType domainType, IInitializer initializer)
		{
			if (domainType.Type != initializer.InitializedType) { throw new InitializedTypeDoNotMatchException(domainType.Type, initializer.InitializedType); }

			this.domainType = domainType;
			this.initializers = new List<IInitializer>();
			Parameter = new Dictionary<string, DomainParameter>();

			Marks = new Marks(ctx.CodingStyle.InitializerMarkSelector.Select(initializer));

			foreach (var parameter in initializer.Parameters)
			{
				Parameter.Add(parameter.Name, ctx.CreateDomainParameter(parameter, initializers.Count));
			}

			initializers.Add(initializer);

			return this;
		}

		public void AddGroup(IInitializer initializer)
		{
			if (domainType.Type != initializer.InitializedType) { throw new InitializedTypeDoNotMatchException(domainType.Type, initializer.InitializedType); }

			foreach (var parameter in initializer.Parameters)
			{
				if (Parameter.ContainsKey(parameter.Name))
				{
					Parameter[parameter.Name].AddGroup(parameter, initializers.Count);
				}
				else
				{
					Parameter.Add(parameter.Name, ctx.CreateDomainParameter(parameter, initializers.Count));
				}
			}

			initializers.Add(initializer);

			Marks.Join(ctx.CodingStyle.InitializerMarkSelector.Select(initializer));
		}

		public object Initialize(Dictionary<string, ParameterValueData> parameterValues)
		{
			var resolution = new DomainParameterResolver<IInitializer>(initializers, Parameter, parameterValues).Resolve();

			return resolution.Result.Initialize(resolution.Parameters);
		}

		public InitializerModel GetModel()
		{
			return new InitializerModel {
				Marks = Marks.List,
				GroupCount = initializers.Count,
				Parameters = Parameters.Select(p => p.GetModel()).ToList()
			};
		}
	}

	public class InitializedTypeDoNotMatchException : Exception
	{
		public InitializedTypeDoNotMatchException(TypeInfo expected, TypeInfo actual)
			: base(string.Format("Given initializer is expected to initialize {0}, but it initializes {1}", expected, actual)) { }
	}
}
