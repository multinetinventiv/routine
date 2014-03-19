using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core;

namespace Routine
{
	public static class ReflectionExtensions
	{
		#region string
		private static readonly Dictionary<string, TypeInfo> toTypeCache = new Dictionary<string, TypeInfo>();
		public static TypeInfo ToType(this string typeName)
		{
			TypeInfo result = null;
			if(toTypeCache.TryGetValue(typeName, out result))
			{
				return result;
			}

			lock(toTypeCache)
			{
				if(toTypeCache.ContainsKey(typeName))
				{
					return toTypeCache[typeName];
				}

				var types = TypeInfo.GetAllDomainTypes().Where(t => t.FullName == typeName).ToList();

				if(types.Count > 0) { toTypeCache[typeName] = types[0]; return types[0]; }

				try
				{
					result = TypeInfo.Get(Type.GetType(typeName));
					toTypeCache[typeName] = result;
					return result;
				}
				catch(Exception ex)
				{
					throw new Exception("Type cannot be found: " + typeName, ex);
				}

			}
		}
		#endregion

		#region internal Type
		internal static string ToCSharpString(this Type source)
		{
			if(!source.IsGenericType)
			{
				return source.FullName.Replace("+", ".");
			}

			var result = (source.Namespace != null)?source.Namespace + ".":"";
			result += source.Name.Before("`");

			result += "<" + string.Join(",", source.GetGenericArguments().Select(t => t.ToCSharpString())) + ">";

			return result.Replace("+", ".");
		}
		#endregion

		#region IOperation		
		public static bool HasNoParameters(this IOperation operation) { return operation.HasParameters(); }
		public static bool HasParameters<T>(this IOperation operation) { return operation.HasParameters(type.of<T>()); }
		public static bool HasParameters<T1, T2>(this IOperation operation) { return operation.HasParameters(type.of<T1>(), type.of<T2>()); }
		public static bool HasParameters<T1, T2, T3>(this IOperation operation) { return operation.HasParameters(type.of<T1>(), type.of<T2>(), type.of<T3>()); }
		public static bool HasParameters<T1, T2, T3, T4>(this IOperation operation) { return operation.HasParameters(type.of<T1>(), type.of<T2>(), type.of<T3>(), type.of<T4>()); }
		public static bool HasParameters(this IOperation operation, params TypeInfo[] parameterTypes)
		{
			if(operation.Parameters.Count != parameterTypes.Length) { return false; }

			for(int i = 0; i < operation.Parameters.Count; i++)
			{
				if (!parameterTypes[i].CanBe(operation.Parameters[i].ParameterType)) 
				{
					return false; 
				}
			}

			return true;
		}

		public static bool ReturnsVoid(this IOperation operation)
		{
			return operation.ReturnType.IsVoid;
		}
		#endregion

		#region IMember
		public static bool Returns<T>(this IReturnItem returnItem) { return returnItem.Returns(type.of<T>()); }
		public static bool Returns(this IReturnItem returnItem, TypeInfo returnType)
		{
			return returnItem.ReturnType.CanBe(returnType);
		}

		public static bool Returns<T>(this IReturnItem returnItem, string name) { return returnItem.Returns(type.of<T>(), name); }
		public static bool Returns(this IReturnItem returnItem, TypeInfo returnType, string name)
		{
			return returnItem.Returns(returnType) && returnItem.Name == name;
		}

		public static bool ReturnsCollection(this IReturnItem returnItem) { return returnItem.ReturnsCollection<object>(); }
		public static bool ReturnsCollection<T>(this IReturnItem returnItem) { return returnItem.ReturnsCollection(type.of<T>()); }
		public static bool ReturnsCollection(this IReturnItem returnItem, TypeInfo itemType)
		{
			return returnItem.ReturnType.CanBeCollection(itemType);
		}

		public static bool ReturnsCollection(this IReturnItem returnItem, string name) { return returnItem.ReturnsCollection<object>(name); }
		public static bool ReturnsCollection<T>(this IReturnItem returnItem, string name) { return returnItem.ReturnsCollection(type.of<T>(), name); }
		public static bool ReturnsCollection(this IReturnItem returnItem, TypeInfo itemType, string name)
		{
			return returnItem.ReturnsCollection(itemType) && returnItem.Name == name;
		}
		#endregion
	}
}

