namespace Routine.Engine.Virtual;

public class VirtualObject
{
    private readonly VirtualType _type;

    public string Id { get; }

    public VirtualObject(string id, VirtualType type)
    {
        Id = id;
        _type = type;
    }

    public IType Type => _type;

    public override string ToString() => _type.ToStringMethod.Get()(this);

    protected bool Equals(VirtualObject other) => Equals(_type, other._type) && string.Equals(Id, other.Id);

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((VirtualObject)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ((_type != null ? _type.GetHashCode() : 0) * 397) ^ (Id != null ? Id.GetHashCode() : 0);
        }
    }
}
