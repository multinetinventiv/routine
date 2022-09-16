namespace Routine.Engine;

public interface ILocator
{
    Task<List<object>> LocateAsync(IType type, List<string> ids);
}

public class CannotLocateException : Exception
{
    public CannotLocateException(IType type, IEnumerable<string> ids)
        : this(type, ids, null) { }

    public CannotLocateException(IType type, IEnumerable<string> ids, Exception innerException)
        : base($"Id: {ids.ToItemString()}, Type: {type}", innerException) { }
}
