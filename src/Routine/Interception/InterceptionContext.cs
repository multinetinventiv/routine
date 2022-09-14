namespace Routine.Interception;

public class InterceptionContext
{
    protected readonly Dictionary<string, object> data;

    public InterceptionContext(string target)
    {
        data = new Dictionary<string, object>();

        Target = target;
    }

    public virtual object this[string key]
    {
        get
        {
            data.TryGetValue(key, out var result);
            return result;
        }
        set => data[key] = value;
    }

    public string Target { get; }

    public virtual object Result { get; set; }
    public virtual bool Canceled { get; set; }
    public virtual Exception Exception { get; set; }
    public virtual bool ExceptionHandled { get; set; }
}
