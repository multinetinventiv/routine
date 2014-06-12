using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Routine.Core.Reflection
{
	public abstract class MethodBase
	{
		public abstract bool IsPublic { get; }

		public abstract ParameterInfo[] GetParameters();
		public abstract object[] GetCustomAttributes();

		public bool HasNoParameters() { return GetParameters().Length == 0; }
		public bool HasParameters<T1>() { return HasParameters(type.of<T1>()); }
		public bool HasParameters<T1, T2>() { return HasParameters(type.of<T1>(), type.of<T2>()); }
		public bool HasParameters<T1, T2, T3>() { return HasParameters(type.of<T1>(), type.of<T2>(), type.of<T3>()); }
		public bool HasParameters<T1, T2, T3, T4>() { return HasParameters(type.of<T1>(), type.of<T2>(), type.of<T3>(), type.of<T4>()); }
		public bool HasParameters<T1, T2, T3, T4, T5>() { return HasParameters(type.of<T1>(), type.of<T2>(), type.of<T3>(), type.of<T4>(), type.of<T5>()); }
		public bool HasParameters<T1, T2, T3, T4, T5, T6>() { return HasParameters(type.of<T1>(), type.of<T2>(), type.of<T3>(), type.of<T4>(), type.of<T5>(), type.of<T6>()); }
		public bool HasParameters<T1, T2, T3, T4, T5, T6, T7>() { return HasParameters(type.of<T1>(), type.of<T2>(), type.of<T3>(), type.of<T4>(), type.of<T5>(), type.of<T6>(), type.of<T7>()); }
		public bool HasParameters(TypeInfo firstParameterType, params TypeInfo[] otherParameterTypes)
		{
			var parameterTypes = new List<TypeInfo>();
			parameterTypes.Add(firstParameterType);
			parameterTypes.AddRange(otherParameterTypes);

			var parameters = GetParameters();
			if (parameters.Length != parameterTypes.Count) { return false; }

			for (int i = 0; i < parameters.Length; i++)
			{
				if (!parameterTypes[i].CanBe(parameters[i].ParameterType))
				{
					return false;
				}
			}

			return true;
		}

		public bool Has<TAttribute>() where TAttribute : Attribute { return Has(type.of<TAttribute>()); }
		public bool Has(TypeInfo attributeType)
		{
			return GetCustomAttributes().Any(a => a.GetTypeInfo() == attributeType);
		}
	}
}
