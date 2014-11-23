using System.Collections;
using System.Collections.Generic;

namespace Routine.Engine
{
	public interface IType : ITypeComponent
	{
		bool IsPublic { get; }
		bool IsAbstract { get; }
		bool IsInterface { get; }
		bool IsValueType { get; }
		bool IsGenericType { get; }
		bool IsPrimitive { get; }

		bool IsVoid { get; }
		bool IsEnum { get; }
		bool IsArray { get; }
		bool IsDomainType { get; }

		string FullName { get; }
		string Namespace { get; }
		IType BaseType { get; }

		List<IType> ConvertibleTypes { get; }
		List<IInitializer> Initializers { get; }
		List<IMember> Members { get; }
		List<IOperation> Operations { get; }

		List<IType> GetGenericArguments();
		IType GetElementType();
		IOperation GetParseOperation();

		List<string> GetEnumNames();
		List<object> GetEnumValues();
		IType GetEnumUnderlyingType();
		
		bool CanBe(IType otherType);
		object Convert(object target, IType otherType);

		object CreateInstance();
		IList CreateListInstance(int length);
	}
}