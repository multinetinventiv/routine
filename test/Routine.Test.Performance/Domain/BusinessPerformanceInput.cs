using System;

namespace Routine.Test.Performance.Domain;

public readonly struct BusinessPerformanceInput
{
    public string Str { get; }
    public int Int { get; }
    public string Str2 { get; }
    public string Str3 { get; }
    public string Str4 { get; }

    //public BusinessPerformanceInput(string s, int i, string s2)
    //	: this()
    //{
    //	Str = s;
    //	Int = i;
    //	Str2 = s2;
    //}
    public BusinessPerformanceInput(string s, int i, string s2, string s3, string s4)
        : this()
    {
        Str = s;
        Int = i;
        Str2 = s2;
        Str3 = s3;
        Str4 = s4;
    }

    public bool Equals(BusinessPerformanceInput other) =>
        string.Equals(Str, other.Str) &&
        Int == other.Int &&
        string.Equals(Str2, other.Str2) &&
        string.Equals(Str3, other.Str3) &&
        string.Equals(Str4, other.Str4);

    public override bool Equals(object obj) => obj is BusinessPerformanceInput input && Equals(input);

    public override int GetHashCode() => HashCode.Combine(Str, Int, Str2, Str3, Str4);
}
