namespace Routine.Interception;

public class InterceptionContext
{
    protected readonly Dictionary<string, object> _data;

    public InterceptionContext(string target)
    {
        _data = new();

        Target = target;
    }

    public virtual object this[string key]
    {
        get
        {
            _data.TryGetValue(key, out var result);
            return result;
        }
        set => _data[key] = value;
    }

    public string Target { get; }

    public virtual object Result { get; set; }
    public virtual bool Canceled { get; set; }
    public virtual Exception Exception { get; set; }
    public virtual bool ExceptionHandled { get; set; }
}
