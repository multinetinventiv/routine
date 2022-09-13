namespace Routine.Test;

[Serializable]
public struct Text : IEquatable<Text>
{
    public static Text Parse(string value) => new(value);

    private readonly string value;

    public Text(string value)
    {
        this.value = value;
    }

    public string Value => ToString();
    public override string ToString() => value;

    public static bool operator ==(Text l, Text r) => Equals(l, r);
    public static bool operator !=(Text l, Text r) => !(l == r);

    public bool Equals(Text other) => value == other.value;
    public override bool Equals(object obj) => obj is Text other && Equals(other);
    public override int GetHashCode() => value != null ? value.GetHashCode() : 0;
}
