using System.Collections.Generic;
using System.Linq;
using Routine.Core;

namespace Routine.Engine
{
	internal class DomainParameterResolver<T> where T : class, IParametric
	{
		private readonly List<DomainParameterGroup<T>> alternatives;
		private readonly Dictionary<string, ParameterValueData> parameterValueDatas;

		public DomainParameterResolver(List<DomainParameterGroup<T>> alternatives, Dictionary<string, ParameterValueData> parameterValueDatas)
		{
			if (parameterValueDatas == null) { parameterValueDatas = new Dictionary<string, ParameterValueData>(); }

			this.alternatives = alternatives;
			this.parameterValueDatas = parameterValueDatas;
		}

		public Resolution Resolve()
		{
			var mostCompatibleAlternative = FindMostCompatibleAlternative();

			var parameters = PrepareParametersFor(mostCompatibleAlternative);

			return new Resolution(mostCompatibleAlternative.Parametric, parameters);
		}

		private object[] PrepareParametersFor(DomainParameterGroup<T> alternative)
		{
			var result = new object[alternative.Parameters.Count];

			foreach (var parameter in alternative.Parametric.Parameters)
			{
				ParameterValueData parameterValueData;
				if (parameterValueDatas.TryGetValue(parameter.Name, out parameterValueData))
				{
					result[parameter.Index] = alternative.Parameters[parameter.Index].Locate(parameterValueData);
				}
			}

			return result;
		}

		private DomainParameterGroup<T> FindMostCompatibleAlternative()
		{
			if (alternatives.Count == 1)
			{
				return alternatives[0];
			}

			var exactMatch = alternatives.SingleOrDefault(a => MatchesExactlyWithValues(a));

			if (exactMatch != null)
			{
				return exactMatch;
			}

			var foundAlternatives = FindAlternativesWithMostMatchedParameters();

			if (foundAlternatives.Count == 1) { return foundAlternatives[0]; }

			return GetFirstAlternativeWithLeastNonMatchedParameters(foundAlternatives);
		}

		private bool MatchesExactlyWithValues(DomainParameterGroup<T> alternative)
		{
			return
				alternative.Parametric.Parameters.Count == parameterValueDatas.Count &&
				alternative.Parametric.Parameters.All(p => parameterValueDatas.ContainsKey(p.Name));
		}

		private List<DomainParameterGroup<T>> FindAlternativesWithMostMatchedParameters()
		{
			var result = new List<DomainParameterGroup<T>>();

			int matchCount = int.MinValue;

			foreach (var alternative in alternatives.OrderByDescending(o => o.Parameters.Count))
			{
				int tempCount = alternative.Parametric.Parameters.Count(p => parameterValueDatas.ContainsKey(p.Name));

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

		private DomainParameterGroup<T> GetFirstAlternativeWithLeastNonMatchedParameters(List<DomainParameterGroup<T>> foundAlternatives)
		{
			DomainParameterGroup<T> result = null;

			int nonMatchCount = int.MaxValue;

			foreach (var alternative in foundAlternatives.OrderByDescending(o => o.Parameters.Count))
			{
				int tempCount = alternative.Parametric.Parameters.Count(p => !parameterValueDatas.ContainsKey(p.Name));

				if (tempCount < nonMatchCount)
				{
					result = alternative;
					nonMatchCount = tempCount;
				}
			}

			return result;
		}

		public class Resolution
		{
			public T Result { get; private set; }
			public object[] Parameters { get; private set; }

			internal Resolution(T result, object[] parameters)
			{
				Result = result;
				Parameters = parameters;
			}
		}
	}
}
