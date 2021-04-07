using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core;

namespace Routine.Engine
{
    public class DomainParameter
    {
        #region internal class DomainParameter.Group

        internal class Group<T> where T : class, IParametric
        {
            public T Parametric { get; }
            public List<DomainParameter> Parameters { get; }
            public int GroupIndex { get; }

            public Group(T parametric, IEnumerable<DomainParameter> parameters, int groupIndex)
            {
                Parametric = parametric;
                Parameters = parameters.OrderBy(p => parametric.Parameters.Single(p2 => p2.Name == p.Name).Index).ToList();
                GroupIndex = groupIndex;
            }

            public bool ContainsSameParameters(T parametric)
            {
                return Parametric.Parameters.Count == parametric.Parameters.Count &&
                       Parametric.Parameters.All(p1 => parametric.Parameters.Any(p2 => p1.Name == p2.Name));
            }
        }

        #endregion

        #region internal static void AddGroupToTarget<T>(T group, IDomainParametric<T> target)

        internal static void AddGroupToTarget<T>(T group, IDomainParametric<T> target)
            where T : class, IParametric
        {
            Validate(group, target);

            foreach (var parameter in group.Parameters)
            {
                if (target.Parameter.TryGetValue(parameter.Name, out var domainParameter))
                {
                    domainParameter.AddGroup(parameter, target.NextGroupIndex);
                }
                else
                {
                    target.Parameter.Add(parameter.Name, new DomainParameter(target.Ctx, parameter, target.NextGroupIndex));
                }
            }

            target.AddGroup(group, target.Parameter.Values.Where(p => p.Groups.Contains(target.NextGroupIndex)), target.NextGroupIndex);
        }

        private static void Validate<T>(T group, IDomainParametric<T> target)
            where T : class, IParametric
        {
            foreach (var parameter in group.Parameters)
            {
                if (target.Parameter.TryGetValue(parameter.Name, out var domainParameter))
                {
                    if (domainParameter.Groups.Contains(target.NextGroupIndex))
                    {
                        throw new InvalidOperationException(
                            $"{parameter.Owner.ParentType.Name}.{parameter.Owner.Name}(...,{parameter.Name},...): Given groupIndex ({target.NextGroupIndex}) was already added!");
                    }

                    if (!domainParameter.parameter.ParameterType.Equals(parameter.ParameterType))
                    {
                        throw new ParameterTypesDoNotMatchException(
                            parameter,
                            domainParameter.ParameterType.Type,
                            parameter.ParameterType
                        );
                    }
                }
                else if (!target.Ctx.CodingStyle.ContainsType(Fix(parameter.ParameterType)))
                {
                    throw new TypeNotConfiguredException(Fix(parameter.ParameterType));
                }
            }
        }

        private static IType Fix(IType type) => type.CanBeCollection() ? type.GetItemType() : type;

        #endregion

        private readonly ICoreContext ctx;
        private readonly IParameter parameter;

        public string Name { get; }
        public Marks Marks { get; }
        public List<int> Groups { get; }
        public bool IsList { get; }
        public DomainType ParameterType { get; }

        private DomainParameter(ICoreContext ctx, IParameter parameter, int initialGroupIndex)
        {
            this.ctx = ctx;
            this.parameter = parameter;

            Name = ctx.CodingStyle.GetName(parameter);
            Marks = new Marks(ctx.CodingStyle.GetMarks(parameter));
            Groups = new List<int> { initialGroupIndex };
            IsList = parameter.ParameterType.CanBeCollection();
            ParameterType = ctx.GetDomainType(Fix(parameter.ParameterType));
        }

        private void AddGroup(IParameter parameter, int groupIndex)
        {
            Groups.Add(groupIndex);

            Marks.Join(ctx.CodingStyle.GetMarks(parameter));
        }

        public bool MarkedAs(string mark)
        {
            return Marks.Has(mark);
        }

        public ParameterModel GetModel()
        {
            return new ParameterModel
            {
                Name = Name,
                Marks = Marks.List,
                Groups = Groups,
                IsList = IsList,
                ViewModelId = ParameterType.Id
            };
        }

        internal object Locate(ParameterValueData parameterValueData)
        {
            if (!IsList)
            {
                return GetObject(parameterValueData);
            }

            var result = parameter.ParameterType.CreateListInstance(parameterValueData.Values.Count);

            var objects = GetObjects(parameterValueData);

            for (int i = 0; i < objects.Count; i++)
            {
                if (parameter.ParameterType.IsArray)
                {
                    result[i] = objects[i];
                }
                else
                {
                    result.Add(objects[i]);
                }
            }

            return result;
        }

        private object GetObject(ParameterValueData parameterValueData)
        {
            if (!parameterValueData.Values.Any())
            {
                return null;
            }

            var parameterData = parameterValueData.Values[0];

            return GetDomainType(parameterData).Locate(parameterData);
        }

        private List<object> GetObjects(ParameterValueData parameterValueData)
        {
            if (!parameterValueData.Values.Any())
            {
                return new List<object>();
            }

            var result = new List<object>();

            var domainTypes = parameterValueData.Values.Select(pd => GetDomainType(pd)).ToList();

            if (domainTypes.Any(dt => !Equals(dt, ParameterType)))
            {
                for (int i = 0; i < parameterValueData.Values.Count; i++)
                {
                    var parameterData = parameterValueData.Values[i];
                    var domainType = domainTypes[i];

                    result.Add(domainType.Locate(parameterData));
                }
            }
            else
            {
                result.AddRange(ParameterType.LocateMany(parameterValueData.Values));
            }

            return result;
        }

        private DomainType GetDomainType(ParameterData parameterData)
        {
            if (parameterData == null)
            {
                return ctx.GetDomainType((IType)null);
            }

            var domainType = ParameterType;

            if (parameterData.ModelId != domainType.Id && !string.IsNullOrEmpty(parameterData.ModelId))
            {
                domainType = ctx.GetDomainType(parameterData.ModelId);
            }

            return domainType;
        }

        #region Formatting & Equality

        protected bool Equals(DomainParameter other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals((DomainParameter)obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("{1} {0}", Name, ParameterType);
        }

        #endregion
    }
}
