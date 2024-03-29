﻿namespace Routine.Engine.Reflection;

public abstract class ConstructorInfo : MethodBase, IConstructor
{
    internal static ConstructorInfo Reflected(System.Reflection.ConstructorInfo constructor) => new ReflectedConstructorInfo(constructor).Load();
    internal static ConstructorInfo Preloaded(System.Reflection.ConstructorInfo constructor) => new PreloadedConstructorInfo(constructor).Load();

    protected readonly System.Reflection.ConstructorInfo _constructorInfo;

    protected ConstructorInfo(System.Reflection.ConstructorInfo constructorInfo)
    {
        _constructorInfo = constructorInfo;
    }

    public System.Reflection.ConstructorInfo GetActualConstructor() => _constructorInfo;

    public override string Name => _constructorInfo.Name;

    protected abstract ConstructorInfo Load();

    public abstract object Invoke(params object[] parameters);

    #region IInitializer implementation

    IType IConstructor.InitializedType => ReflectedType;
    object IConstructor.Initialize(params object[] parameters) => Invoke(parameters);

    #endregion
}
