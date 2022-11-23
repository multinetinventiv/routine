namespace Routine.Test;

[Serializable]
public struct Text : IEquatable<Text>
{
    public static Text Parse(string value) => new(value);

    private readonly string _value;

    public Text(string value)
    {
        _value = value;
    }

    public string Value => ToString();
    public override string ToString() => _value;

    public static bool operator ==(Text l, Text r) => Equals(l, r);
    public static bool operator !=(Text l, Text r) => !(l == r);

    public bool Equals(Text other) => _value == other._value;
    public override bool Equals(object obj) => obj is Text other && Equals(other);
    public override int GetHashCode() => _value != null ? _value.GetHashCode() : 0;
}
