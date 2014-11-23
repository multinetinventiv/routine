using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Routine.Engine;

namespace Routine
{
	public static class ReflectionExtensions
	{
		#region string

		public static TypeInfo ToTypeInfo(this string typeName) { return typeName.ToTypeInfo(false); }
		public static TypeInfo ToTypeInfo(this string typeName, bool deepSearch)
		{
			try
			{
				var type = Type.GetType(typeName);

				if (type == null && deepSearch)
				{
					foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
					{
						type = assembly.GetTypes().SingleOrDefault(t => t.FullName == typeName);
						if (type != null)
						{
							break;
						}
					}
				}

				if (type == null)
				{
					throw new Exception("Type cannot be found: " + typeName);
				}

				return TypeInfo.Get(type);
			}
			catch (Exception ex)
			{
				throw new Exception("Type cannot be found: " + typeName, ex);
			}
		}

		#endregion

		#region internal Type

		internal static string ToCSharpString(this Type source)
		{
			if (!source.IsGenericType)
			{
				return source.FullName.Replace("+", ".");
			}

			var result = (source.Namespace != null) ? source.Namespace + "." : "";
			result += source.Name.Before("`");

			result += "<" + string.Join(",", source.GetGenericArguments().Select(t => t.ToCSharpString())) + ">";

			return result.Replace("+", ".");
		}

		#endregion

		#region IType

		public static bool CanBe<T>(this IType source) { return source.CanBe(type.of<T>()); }

		public static bool CanBeCollection(this IType source) { return source.CanBeCollection<object>(); }
		public static bool CanBeCollection<T>(this IType source) { return source.CanBeCollection(type.of<T>()); }
		public static bool CanBeCollection(this IType source, IType itemType)
		{
			return source.CanBe<ICollection>() &&
				(source.IsGenericType && source.GetGenericArguments()[0].CanBe(itemType)) ||
				(source.IsArray && source.GetElementType().CanBe(itemType));
		}

		public static IType GetItemType(this IType source)
		{
			if (!source.CanBeCollection()) { throw new ArgumentException("Type should be a generic collection or an array to have an item type", "source"); }
			if (source.IsGenericType) { return source.GetGenericArguments()[0]; }
			if (source.IsArray) { return source.GetElementType(); }

			throw new NotSupportedException();
		}

		public static bool CanParse(this IType source) { return source.GetParseOperation() != null; }
		public static object Parse(this IType source, string value) { return source.GetParseOperation().PerformOn(null, value); }

		#endregion

		#region ITypeComponent

		public static bool Has<TAttribute>(this ITypeComponent source) where TAttribute : Attribute { return source.Has(type.of<TAttribute>()); }
		public static bool Has(this ITypeComponent source, TypeInfo attributeType)
		{
			return source.GetCustomAttributes().Any(a => a.GetTypeInfo() == attributeType);
		}

		#endregion

		#region IParametric

		public static bool HasNoParameters(this IParametric source) { return source.Parameters.Count == 0; }
		public static bool HasParameters<T>(this IParametric source) { return source.HasParameters(type.of<T>()); }
		public static bool HasParameters<T1, T2>(this IParametric source) { return source.HasParameters(type.of<T1>(), type.of<T2>()); }
		public static bool HasParameters<T1, T2, T3>(this IParametric source) { return source.HasParameters(type.of<T1>(), type.of<T2>(), type.of<T3>()); }
		public static bool HasParameters<T1, T2, T3, T4>(this IParametric source) { return source.HasParameters(type.of<T1>(), type.of<T2>(), type.of<T3>(), type.of<T4>()); }
		public static bool HasParameters<T1, T2, T3, T4, T5>(this IParametric source) { return source.HasParameters(type.of<T1>(), type.of<T2>(), type.of<T3>(), type.of<T4>(), type.of<T5>()); }
		public static bool HasParameters<T1, T2, T3, T4, T5, T6>(this IParametric source) { return source.HasParameters(type.of<T1>(), type.of<T2>(), type.of<T3>(), type.of<T4>(), type.of<T5>(), type.of<T6>()); }
		public static bool HasParameters<T1, T2, T3, T4, T5, T6, T7>(this IParametric source) { return source.HasParameters(type.of<T1>(), type.of<T2>(), type.of<T3>(), type.of<T4>(), type.of<T5>(), type.of<T6>(), type.of<T7>()); }
		public static bool HasParameters(this IParametric source, IType firstParameterType, params IType[] otherParameterTypes)
		{
			var parameterTypes = new List<IType>();
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

		public static bool IsInherited(this IOperation operation) { return operation.IsInherited(false); }
		public static bool IsInherited(this IOperation operation, bool ignoreSameRootNamespace) { return operation.IsInherited(ignoreSameRootNamespace, false); }
		public static bool IsInherited(this IOperation operation, bool ignoreSameRootNamespace, bool useFirstDeclaration)
		{
			var parent = operation.ParentType;
			var declaring = operation.GetDeclaringType(useFirstDeclaration);

			if (!ignoreSameRootNamespace)
			{
				return !declaring.Equals(parent);
			}

			if (parent.Namespace == null && declaring.Namespace == null) { return true; }
			if (parent.Namespace == null || declaring.Namespace == null) { return false; }

			return parent.Namespace.Before(".") != declaring.Namespace.Before(".");
		}

		#endregion

		#region IMember

		public static bool IsInherited(this IMember member) { return member.IsInherited(false); }
		public static bool IsInherited(this IMember member, bool ignoreSameRootNamespace) { return member.IsInherited(ignoreSameRootNamespace, false); }
		public static bool IsInherited(this IMember member, bool ignoreSameRootNamespace, bool useFirstDeclaration)
		{
			var parent = member.ParentType;
			var declaring = member.GetDeclaringType(useFirstDeclaration);

			if (!ignoreSameRootNamespace)
			{
				return !declaring.Equals(parent);
			}

			if (parent.Namespace == null && declaring.Namespace == null) { return true; }
			if (parent.Namespace == null || declaring.Namespace == null) { return false; }

			return parent.Namespace.Before(".") != declaring.Namespace.Before(".");
		}

		#endregion

		#region IReturnable

		public static bool Returns<T>(this IReturnable source) { return source.Returns(type.of<T>()); }
		public static bool Returns(this IReturnable source, IType returnType)
		{
			return source.ReturnType.CanBe(returnType);
		}

		public static bool Returns<T>(this IReturnable source, string name) { return source.Returns(type.of<T>(), name); }
		public static bool Returns(this IReturnable source, IType returnType, string name)
		{
			return source.Returns(returnType) && source.Name == name;
		}

		public static bool ReturnsCollection(this IReturnable source) { return source.ReturnsCollection<object>(); }
		public static bool ReturnsCollection<T>(this IReturnable source) { return source.ReturnsCollection(type.of<T>()); }
		public static bool ReturnsCollection(this IReturnable source, IType itemType)
		{
			return source.ReturnType.CanBeCollection(itemType);
		}

		public static bool ReturnsCollection(this IReturnable source, string name) { return source.ReturnsCollection<object>(name); }
		public static bool ReturnsCollection<T>(this IReturnable source, string name) { return source.ReturnsCollection(type.of<T>(), name); }
		public static bool ReturnsCollection(this IReturnable source, IType itemType, string name)
		{
			return source.ReturnsCollection(itemType) && source.Name == name;
		}

		public static bool ReturnTypeHas<TAttribute>(this IReturnable source) where TAttribute : Attribute { return source.ReturnTypeHas(type.of<TAttribute>()); }
		public static bool ReturnTypeHas(this IReturnable source, TypeInfo attributeType)
		{
			return source.GetReturnTypeCustomAttributes().Any(a => a.GetTypeInfo() == attributeType);
		}

		#endregion
	}
}

