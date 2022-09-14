namespace Routine.Engine.Reflection;

internal class ParseableTypeInfo : PreloadedTypeInfo
{
    private MethodInfo parseMethod;

    internal ParseableTypeInfo(Type type)
        : base(type) { }

    protected override void Load()
    {
        base.Load();

        parseMethod = MethodInfo.Preloaded(type.GetMethod("Parse", new[] { typeof(string) }));

        if (parseMethod.ReturnType != this)
        {
            throw new InvalidOperationException(
                $"{type} was loaded as Parseable but its static Parse method does not return {type}"
            );
        }
    }

    protected override MethodInfo GetParseMethod() => parseMethod;
}
