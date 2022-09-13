namespace Routine.Engine;

public interface IConverter
{
    List<IType> GetTargetTypes(IType type);
    object Convert(object @object, IType from, IType to);
}

public class CannotConvertException : Exception
{
    public CannotConvertException(object @object, IType targetType) : this(@object, targetType, null) { }
    public CannotConvertException(object @object, IType targetType, Exception innerException)
        : base($"{@object} ({@object?.GetType()}) cannot be converted to {targetType}", innerException) { }
}
