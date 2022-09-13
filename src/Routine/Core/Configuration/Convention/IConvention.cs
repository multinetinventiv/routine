namespace Routine.Core.Configuration.Convention;

public interface IConvention<in TFrom, out TResult>
    where TFrom : notnull
    where TResult : notnull
{
    bool AppliesTo(TFrom? obj);
    TResult? Apply(TFrom? obj);
}
