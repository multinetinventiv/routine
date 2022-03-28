using System;

namespace Routine
{
    public static class FuncExtensions
    {
        public static Func<T, bool> And<T>(this Func<T, bool> left, Func<T, bool> right) => o => left(o) && right(o);
        public static Func<T, bool> Or<T>(this Func<T, bool> left, Func<T, bool> right) => o => left(o) || right(o);
    }
}
