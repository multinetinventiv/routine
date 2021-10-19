using System.Collections.Generic;
using System.Linq;
using Routine.Core;

namespace Routine.Engine
{
    internal class DomainParameterResolver<T> where T : class, IParametric
    {
        private readonly List<DomainParameter.Group<T>> alternatives;
        private readonly Dictionary<string, ParameterValueData> parameterValueDatas;

        public DomainParameterResolver(List<DomainParameter.Group<T>> alternatives, Dictionary<string, ParameterValueData> parameterValueDatas)
        {
            parameterValueDatas ??= new Dictionary<string, ParameterValueData>();

            this.alternatives = alternatives;
            this.parameterValueDatas = parameterValueDatas;
        }

        public Resolution Resolve()
        {
            var mostCompatibleAlternative = FindMostCompatibleAlternative();

            var parameters = PrepareParametersFor(mostCompatibleAlternative);

            return new Resolution(mostCompatibleAlternative.Parametric, parameters);
        }

        private object[] PrepareParametersFor(DomainParameter.Group<T> alternative)
        {
            var result = new object[alternative.Parameters.Count];

            foreach (var parameter in alternative.Parametric.Parameters)
            {
                if (parameterValueDatas.TryGetValue(parameter.Name, out var parameterValueData))
                {
                    result[parameter.Index] = alternative.Parameters[parameter.Index].Locate(parameterValueData);
                }
                else if (parameter.IsOptional && parameter.HasDefaultValue)
                {
                    result[parameter.Index] = parameter.DefaultValue;
                }
            }

            return result;
        }

        private DomainParameter.Group<T> FindMostCompatibleAlternative()
        {
            if (alternatives.Count == 1)
            {
                return alternatives[0];
            }

            var exactMatch = alternatives.SingleOrDefault(MatchesExactlyWithValues);

            if (exactMatch != null)
            {
                return exactMatch;
            }

            var foundAlternatives = FindAlternativesWithMostMatchedParameters();

            return foundAlternatives.Count == 1
                ? foundAlternatives[0]
                : GetFirstAlternativeWithLeastNonMatchedParameters(foundAlternatives);
        }

        private bool MatchesExactlyWithValues(DomainParameter.Group<T> alternative) =>
            alternative.Parametric.Parameters.Count == parameterValueDatas.Count &&
            alternative.Parametric.Parameters.All(p => parameterValueDatas.ContainsKey(p.Name));

        private List<DomainParameter.Group<T>> FindAlternativesWithMostMatchedParameters()
        {
            var result = new List<DomainParameter.Group<T>>();

            var matchCount = int.MinValue;

            foreach (var alternative in alternatives.OrderByDescending(o => o.Parameters.Count))
            {
                var tempCount = alternative.Parametric.Parameters.Count(p => parameterValueDatas.ContainsKey(p.Name));

                if (tempCount > matchCount)
                {
                    result.Clear();
                    result.Add(alternative);
                    matchCount = tempCount;
                }
                else if (tempCount == matchCount)
                {
                    result.Add(alternative);
                }
            }

            return result;
        }

        private DomainParameter.Group<T> GetFirstAlternativeWithLeastNonMatchedParameters(List<DomainParameter.Group<T>> foundAlternatives)
        {
            DomainParameter.Group<T> result = null;

            var nonMatchCount = int.MaxValue;

            foreach (var alternative in foundAlternatives.OrderByDescending(o => o.Parameters.Count))
            {
                var tempCount = alternative.Parametric.Parameters.Count(p => !parameterValueDatas.ContainsKey(p.Name));

                if (tempCount < nonMatchCount)
                {
                    result = alternative;
                    nonMatchCount = tempCount;
                }
            }

            return result;
        }

        public record Resolution(T Result, object[] Parameters);
    }
}
