namespace Routine.Core.Configuration.Convention;

public class ConventionBuilder<TFrom, TData>
{
    internal DelegateBasedConvention<TFrom, TData> By() => new();

    public DelegateBasedConvention<TFrom, TData> By(Func<TFrom, TData> converterDelegate) => By().Return(converterDelegate);
    public DelegateBasedConvention<TFrom, TData> Constant(TData result) => By(_ => result);
}
