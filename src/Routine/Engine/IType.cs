namespace Routine.Engine;

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

    string FullName { get; }
    string? Namespace { get; }
    IType? BaseType { get; }

    List<IType> AssignableTypes { get; }
    List<IConstructor> Constructors { get; }
    List<IProperty> Properties { get; }
    List<IMethod> Methods { get; }

    List<IType> GetGenericArguments();
    IType? GetElementType();
    IMethod? GetParseMethod();

    List<string> GetEnumNames();
    List<object> GetEnumValues();
    IType? GetEnumUnderlyingType();

    bool CanBe(IType otherType);
    object Cast(object @object, IType otherType);

    object CreateInstance();
    IList CreateListInstance(int length);
}
