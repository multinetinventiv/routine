namespace Routine.Core.Reflection;

internal class SwitchableMethodInvoker : IMethodInvoker
{
    private volatile IMethodInvoker _invoker;

    public IMethodInvoker Invoker => _invoker;

    public SwitchableMethodInvoker(IMethodInvoker invoker)
    {
        _invoker = invoker;
    }

    public event EventHandler Switched;

    public void Switch(IMethodInvoker invoker)
    {
        _invoker = invoker;

        Switched?.Invoke(this, EventArgs.Empty);
    }

    public object Invoke(object target, params object[] args) => Invoker.Invoke(target, args);
    public async Task<object> InvokeAsync(object target, params object[] args) => await Invoker.InvokeAsync(target, args);
}
