using Routine.Core;
using System.Collections.Generic;
using System.Linq;

namespace Routine.Engine
{
    public class DomainObjectInitializer : IDomainParametric<IConstructor>
    {
        private readonly ICoreContext ctx;
        private readonly List<DomainParameter.Group<IConstructor>> groups;

        public Dictionary<string, DomainParameter> Parameter { get; }
        public Marks Marks { get; }

        public ICollection<DomainParameter> Parameters => Parameter.Values;

        ICoreContext IDomainParametric<IConstructor>.Ctx => ctx;
        int IDomainParametric<IConstructor>.NextGroupIndex => groups.Count;
        void IDomainParametric<IConstructor>.AddGroup(IConstructor parametric, IEnumerable<DomainParameter> parameters, int groupIndex) => groups.Add(new DomainParameter.Group<IConstructor>(parametric, parameters, groupIndex));

        public DomainObjectInitializer(ICoreContext ctx, IConstructor constructor)
        {
            this.ctx = ctx;

            groups = new List<DomainParameter.Group<IConstructor>>();
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

            DomainParameter.AddGroupToTarget(constructor, this);

            Marks.Join(ctx.CodingStyle.GetMarks(constructor));
        }

        public object Initialize(Dictionary<string, ParameterValueData> parameterValues)
        {
            var resolution = new DomainParameterResolver<IConstructor>(groups, parameterValues).Resolve();

            var result = resolution.Result.Initialize(resolution.Parameters);

            return result;
        }

        public InitializerModel GetModel() =>
            new()
            {
                Marks = Marks.List,
                GroupCount = groups.Count,
                Parameters = Parameters.Select(p => p.GetModel()).ToList()
            };
    }
}
