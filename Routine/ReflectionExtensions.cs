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
					var type = Type.GetType(typeName);
					if (type == null)
					{
						foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
						{
							type = assembly.GetTypes().SingleOrDefault(t => t.FullName == typeName);
							if (type != null)
							{
								break;
							}
						}

						if (type == null)
						{
							throw new NullReferenceException();
						}
					}

					result = TypeInfo.Get(type);
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

		#region ITypeComponent

		public static bool Has<TAttribute>(this ITypeComponent source) where TAttribute : Attribute { return source.Has(type.of<TAttribute>()); }
		public static bool Has(this ITypeComponent source, TypeInfo attributeType)
		{
			return source.GetCustomAttributes().Any(a => a.GetTypeInfo() == attributeType);
		}

		#endregion

		#region IOperation

		public static bool HasNoParameters(this IParametric source) { return source.Parameters.Count == 0; }
		public static bool HasParameters<T>(this IParametric source) { return source.HasParameters(type.of<T>()); }
		public static bool HasParameters<T1, T2>(this IParametric source) { return source.HasParameters(type.of<T1>(), type.of<T2>()); }
		public static bool HasParameters<T1, T2, T3>(this IParametric source) { return source.HasParameters(type.of<T1>(), type.of<T2>(), type.of<T3>()); }
		public static bool HasParameters<T1, T2, T3, T4>(this IParametric source) { return source.HasParameters(type.of<T1>(), type.of<T2>(), type.of<T3>(), type.of<T4>()); }
		public static bool HasParameters<T1, T2, T3, T4, T5>(this IParametric source) { return source.HasParameters(type.of<T1>(), type.of<T2>(), type.of<T3>(), type.of<T4>(), type.of<T5>()); }
		public static bool HasParameters<T1, T2, T3, T4, T5, T6>(this IParametric source) { return source.HasParameters(type.of<T1>(), type.of<T2>(), type.of<T3>(), type.of<T4>(), type.of<T5>(), type.of<T6>()); }
		public static bool HasParameters<T1, T2, T3, T4, T5, T6, T7>(this IParametric source) { return source.HasParameters(type.of<T1>(), type.of<T2>(), type.of<T3>(), type.of<T4>(), type.of<T5>(), type.of<T6>(), type.of<T7>()); }
		public static bool HasParameters(this IParametric source, TypeInfo firstParameterType, params TypeInfo[] otherParameterTypes)
		{
			var parameterTypes = new List<TypeInfo>();
			parameterTypes.Add(firstParameterType);
			parameterTypes.AddRange(otherParameterTypes);

			if (source.Parameters.Count != parameterTypes.Count) { return false; }

			for (int i = 0; i < source.Parameters.Count; i++)
			{
				if (!parameterTypes[i].CanBe(source.Parameters[i].ParameterType)) 
				{
					return false; 
				}
			}

			return true;
		}

		#endregion

		#region IOperation

		public static bool ReturnsVoid(this IOperation operation)
		{
			return operation.ReturnType.IsVoid;
		}

		#endregion

		#region IReturnable

		public static bool Returns<T>(this IReturnable returnItem) { return returnItem.Returns(type.of<T>()); }
		public static bool Returns(this IReturnable returnItem, TypeInfo returnType)
		{
			return returnItem.ReturnType.CanBe(returnType);
		}

		public static bool Returns<T>(this IReturnable returnItem, string name) { return returnItem.Returns(type.of<T>(), name); }
		public static bool Returns(this IReturnable returnItem, TypeInfo returnType, string name)
		{
			return returnItem.Returns(returnType) && returnItem.Name == name;
		}

		public static bool ReturnsCollection(this IReturnable returnItem) { return returnItem.ReturnsCollection<object>(); }
		public static bool ReturnsCollection<T>(this IReturnable returnItem) { return returnItem.ReturnsCollection(type.of<T>()); }
		public static bool ReturnsCollection(this IReturnable returnItem, TypeInfo itemType)
		{
			return returnItem.ReturnType.CanBeCollection(itemType);
		}

		public static bool ReturnsCollection(this IReturnable returnItem, string name) { return returnItem.ReturnsCollection<object>(name); }
		public static bool ReturnsCollection<T>(this IReturnable returnItem, string name) { return returnItem.ReturnsCollection(type.of<T>(), name); }
		public static bool ReturnsCollection(this IReturnable returnItem, TypeInfo itemType, string name)
		{
			return returnItem.ReturnsCollection(itemType) && returnItem.Name == name;
		}

		#endregion
	}
}

