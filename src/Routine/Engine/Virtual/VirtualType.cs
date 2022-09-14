using Routine.Core.Configuration;

namespace Routine.Engine.Virtual;

public class VirtualType : IType
{
    public SingleConfiguration<VirtualType, string> Name { get; }
    public SingleConfiguration<VirtualType, string> Namespace { get; }
    public SingleConfiguration<VirtualType, bool> IsInterface { get; }
    public SingleConfiguration<VirtualType, string> DefaultInstanceId { get; }
    public SingleConfiguration<VirtualType, Func<VirtualObject, string>> ToStringMethod { get; }
    public ListConfiguration<VirtualType, VirtualType> AssignableTypes { get; }
    public ListConfiguration<VirtualType, IMethod> Methods { get; }

    public VirtualType()
    {
        Name = new SingleConfiguration<VirtualType, string>(this, nameof(Name), true);
        Namespace = new SingleConfiguration<VirtualType, string>(this, nameof(Namespace), true);
        IsInterface = new SingleConfiguration<VirtualType, bool>(this, nameof(IsInterface));
        DefaultInstanceId = new SingleConfiguration<VirtualType, string>(this, nameof(DefaultInstanceId), true);
        ToStringMethod = new SingleConfiguration<VirtualType, Func<VirtualObject, string>>(this, nameof(ToStringMethod));
        AssignableTypes = new ListConfiguration<VirtualType, VirtualType>(this, nameof(AssignableTypes));
        Methods = new ListConfiguration<VirtualType, IMethod>(this, nameof(Methods));
    }

    private object Cast(object @object, IType otherType)
    {
        if (@object is not VirtualObject vobject)
        {
            throw new InvalidCastException(
                $"Cannot cast a real object to a virtual type. {@object} as {ToString()} -> {otherType}");
        }

        if (!CanBe(otherType))
        {
            throw new InvalidCastException(
                $"Cannot cast object to given type. {vobject} as {ToString()} -> {otherType}");
        }

        return @object;
    }

    private bool CanBe(IType otherType) => Equals(this, otherType) || Equals(type.of<object>(), otherType) || AssignableTypes.Get().Contains(otherType);
    public override string ToString() => $"{Namespace.Get()}.{Name.Get()}";

    #region Equality & Hashcode

    protected bool Equals(VirtualType other)
    {
        return Equals(Name.Get(), other.Name.Get()) && Equals(Namespace.Get(), other.Namespace.Get());
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((VirtualType)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (Name.Get().GetHashCode() * 397) ^ Namespace.Get().GetHashCode();
        }
    }

    #endregion

    #region ITypeComponent implementation

    string ITypeComponent.Name => Name.Get();
    IType ITypeComponent.ParentType => null;
    object[] ITypeComponent.GetCustomAttributes() => Array.Empty<object>();

    #endregion

    #region IType implementation

    bool IType.IsPublic => true;
    bool IType.IsAbstract => false;
    bool IType.IsInterface => IsInterface.Get();
    bool IType.IsValueType => false;
    bool IType.IsGenericType => false;
    bool IType.IsPrimitive => false;
    bool IType.IsVoid => false;
    bool IType.IsEnum => false;
    bool IType.IsArray => false;
    string IType.FullName => $"{Namespace.Get()}.{Name.Get()}";
    string IType.Namespace => Namespace.Get();
    IType IType.BaseType => type.of<object>();
    List<IType> IType.AssignableTypes => AssignableTypes.Get().Cast<IType>().ToList();
    List<IConstructor> IType.Constructors => new();
    List<IProperty> IType.Properties => new();
    List<IMethod> IType.Methods => Methods.Get();
    List<IType> IType.GetGenericArguments() => new();
    IType IType.GetElementType() => null;
    IMethod IType.GetParseMethod() => null;
    List<string> IType.GetEnumNames() => new();
    List<object> IType.GetEnumValues() => new();
    IType IType.GetEnumUnderlyingType() => null;
    bool IType.CanBe(IType otherType) => CanBe(otherType);
    object IType.Cast(object @object, IType otherType) => Cast(@object, otherType);
    object IType.CreateInstance() => new VirtualObject(DefaultInstanceId.Get(), this);
    IList IType.CreateListInstance(int length) => throw new NotSupportedException("Virtual types does not support list type");

    #endregion
}
