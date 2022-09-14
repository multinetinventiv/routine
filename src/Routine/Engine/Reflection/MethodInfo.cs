namespace Routine.Engine.Reflection;

public abstract class MethodInfo : MethodBase, IMethod
{
    internal static MethodInfo Reflected(System.Reflection.MethodInfo methodInfo) => new ReflectedMethodInfo(methodInfo).Load();
    internal static MethodInfo Preloaded(System.Reflection.MethodInfo methodInfo) => new PreloadedMethodInfo(methodInfo).Load();

    protected static Type IgnoreTask(Type type)
    {
        if (type == typeof(Task))
        {
            return typeof(void);
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
        {
            return type.GenericTypeArguments[0];
        }

        return type;
    }

    protected readonly System.Reflection.MethodInfo methodInfo;

    protected MethodInfo(System.Reflection.MethodInfo methodInfo)
    {
        this.methodInfo = methodInfo;
    }

    public System.Reflection.MethodInfo GetActualMethod() => methodInfo;

    protected abstract MethodInfo Load();

    public abstract bool IsStatic { get; }
    public abstract TypeInfo ReturnType { get; }

    public abstract TypeInfo GetFirstDeclaringType();
    public abstract object[] GetReturnTypeCustomAttributes();

    public abstract object Invoke(object target, params object[] parameters);
    public abstract Task<object> InvokeAsync(object target, params object[] parameters);
    public abstract object InvokeStatic(params object[] parameters);
    public abstract Task<object> InvokeStaticAsync(params object[] parameters);

    protected virtual TypeInfo SearchFirstDeclaringType()
    {
        var parameters = GetParameters();
        var result = methodInfo.GetBaseDefinition().DeclaringType;

        if (result == null) { throw new NotSupportedException(); }

        foreach (var interfaceType in result.GetInterfaces())
        {
            foreach (var interfaceMethodInfo in interfaceType.GetMethods())
            {
                if (interfaceMethodInfo.Name != methodInfo.Name)
                {
                    continue;
                }

                if (interfaceMethodInfo.GetParameters().Length != parameters.Length)
                {
                    continue;
                }

                if (parameters.Length == 0)
                {
                    return TypeInfo.Get(interfaceType);
                }

                var interfaceMethodParameters = interfaceMethodInfo.GetParameters();
                for (var i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterType.GetActualType() !=
                        interfaceMethodParameters[i].ParameterType)
                    {
                        break;
                    }

                    if (i == parameters.Length - 1)
                    {
                        return TypeInfo.Get(interfaceType);
                    }
                }
            }
        }

        return TypeInfo.Get(result);
    }

    #region IReturnable implementation

    IType IReturnable.ReturnType => ReturnType;

    #endregion

    #region IMethod implementation

    object IMethod.PerformOn(object target, params object[] parameters) => IsStatic ? InvokeStatic(parameters) : Invoke(target, parameters);
    async Task<object> IMethod.PerformOnAsync(object target, params object[] parameters) => IsStatic ? await InvokeStaticAsync(parameters) : await InvokeAsync(target, parameters);
    IType IMethod.GetDeclaringType(bool firstDeclaringType) => firstDeclaringType ? GetFirstDeclaringType() : DeclaringType;

    #endregion
}
