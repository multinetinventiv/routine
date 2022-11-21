namespace Routine.Engine.Reflection;

internal class ParseableTypeInfo : PreloadedTypeInfo
{
    private MethodInfo _parseMethod;

    internal ParseableTypeInfo(Type type)
        : base(type) { }

    protected internal override void Load()
    {
        base.Load();

        _parseMethod = MethodInfo.Preloaded(_type.GetMethod("Parse", new[] { typeof(string) }));

        if (_parseMethod.ReturnType != this)
        {
            throw new InvalidOperationException(
                $"{_type} was loaded as Parseable but its static Parse method does not return {_type}"
            );
        }
    }

    protected internal override MethodInfo GetParseMethod() => _parseMethod;
}
