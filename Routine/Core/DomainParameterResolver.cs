using System;
using System.Collections.Generic;
using System.Linq;

namespace Routine.Core
{
	internal class DomainParameterResolver<T> where T : class, IParametric
	{
		private List<T> alternatives;
		private Dictionary<string, DomainParameter> domainParameters;
		private Dictionary<string, ParameterValueData> values;

		public DomainParameterResolver(List<T> alternatives, Dictionary<string, DomainParameter> domainParameters, Dictionary<string, ParameterValueData> values)
		{
			if (values == null) { values = new Dictionary<string, ParameterValueData>(); }

			this.alternatives = alternatives;
			this.domainParameters = domainParameters;
			this.values = values;
		}

		public Resolution Resolve()
		{
			var parameterValues = GetParameterValues();

			var result = FindMostCompatibleAlternative(parameterValues);

			var parameters = PrepareParameters(result, parameterValues);

			return new Resolution(result, parameters);
		}

		private List<DomainParameterValue> GetParameterValues()
		{
			var result = new List<DomainParameterValue>();
			foreach (var parameterModelId in values.Keys)
			{
				var parameterValueData = values[parameterModelId];
				DomainParameter parameter;
				if (!domainParameters.TryGetValue(parameterModelId, out parameter))
				{
					continue;
				}

				result.Add(new DomainParameterValue(parameter, parameter.Locate(parameterValueData)));
			}
			return result;
		}

		private T FindMostCompatibleAlternative(List<DomainParameterValue> parameterValues)
		{
			if (alternatives.Count == 1)
			{
				return alternatives[0];
			}

			var exactMatch = alternatives.SingleOrDefault(ThatMatchesExactlyWith(parameterValues));

			if (exactMatch != null)
			{
				return exactMatch;
			}

			var foundAlternatives = FindAlternativesWithMostMatchedParameters(parameterValues);

			if (foundAlternatives.Count == 1)
			{
				return foundAlternatives[0];
			}

			return GetFirstAlternativeWithLeastNonMatchedParameters(foundAlternatives, parameterValues);
		}

		private Func<T, bool> ThatMatchesExactlyWith(List<DomainParameterValue> parameterValues)
		{
			return o =>
				o.Parameters.Count == parameterValues.Count &&
				o.Parameters.All(p => parameterValues.Any(pv => pv.Parameter.Id == p.Name));
		}

		private List<T> FindAlternativesWithMostMatchedParameters(List<DomainParameterValue> parameterValues)
		{
			var result = new List<T>();

			int matchCount = int.MinValue;

			foreach (var alternative in alternatives.OrderByDescending(o => o.Parameters.Count))
			{
				int tempCount = alternative.Parameters.Count(p => parameterValues.Any(pv => pv.Parameter.Id == p.Name));

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

		private T GetFirstAlternativeWithLeastNonMatchedParameters(List<T> foundAlternatives, List<DomainParameterValue> parameterValues)
		{
			T result = null;

			int nonMatchCount = int.MaxValue;

			foreach (var alternative in foundAlternatives.OrderByDescending(o => o.Parameters.Count))
			{
				int tempCount = alternative.Parameters.Count(p => parameterValues.All(pv => pv.Parameter.Id != p.Name));

				if (tempCount < nonMatchCount)
				{
					result = alternative;
					nonMatchCount = tempCount;
				}
			}

			return result;
		}

		private object[] PrepareParameters(T alternative, List<DomainParameterValue> parameterValues)
		{
			var result = new object[alternative.Parameters.Count];

			foreach (var parameter in alternative.Parameters)
			{
				var parameterValue = parameterValues.SingleOrDefault(p => p.Parameter.Id == parameter.Name);

				if (parameterValue == null) { continue; }

				result[parameter.Index] = parameterValue.Value;
			}

			return result;
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

		public class Resolution
		{
			public T Result { get; private set; }
			public object[] Parameters { get; private set; }

			public Resolution(T result, object[] parameters)
			{
				Result = result;
				Parameters = parameters;
			}
		}
	}
}
