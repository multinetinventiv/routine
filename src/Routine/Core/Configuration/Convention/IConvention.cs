namespace Routine.Core.Configuration.Convention;

public interface IConvention<in TFrom, out TResult>
{
    bool AppliesTo(TFrom obj);
    TResult Apply(TFrom obj);
}
