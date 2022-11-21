namespace Routine.Core.Configuration;

public class Layer
{
    public static readonly Layer LeastSpecific = new(0, true);
    public static readonly Layer MostSpecific = new(int.MaxValue, true);

    private readonly int _order;

    public Layer(int order) : this(order, false) { }
    private Layer(int order, bool internalCall)
    {
        if (!internalCall)
        {
            if (order <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(order), order, "\"order\" must be greater than zero");
            }

            if (order == int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(order), order, $"\"order\" must be less than {int.MaxValue}");
            }
        }

        _order = order;
    }

    public int Order => _order;

    public Layer MoreSpecific()
    {
        if (this == MostSpecific)
        {
            throw new InvalidOperationException("Layer cannot get any more specific than this");
        }

        if (Order + 1 == MostSpecific.Order)
        {
            return MostSpecific;
        }

        return new(Order + 1);
    }

    public Layer LessSpecific()
    {
        if (this == LeastSpecific)
        {
            throw new InvalidOperationException("Layer cannot get any less specific than this");
        }

        if (Order - 1 == LeastSpecific.Order)
        {
            return LeastSpecific;
        }

        return new(Order - 1);
    }

    public override string ToString() => $"Layer ({_order})";

    #region Equality & HashCode

    public static bool operator ==(Layer a, Layer b) { return Equals(a, b); }
    public static bool operator !=(Layer a, Layer b) { return !(a == b); }

    protected bool Equals(Layer other)
    {
        return Order == other.Order;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Layer)obj);
    }

    public override int GetHashCode()
    {
        return Order;
    }

    #endregion
}
