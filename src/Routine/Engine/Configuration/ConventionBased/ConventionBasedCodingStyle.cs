using Routine.Core.Configuration;
using Routine.Engine.Virtual;

namespace Routine.Engine.Configuration.ConventionBased;

public class ConventionBasedCodingStyle : LayeredBase<ConventionBasedCodingStyle>, ICodingStyle
{
    private readonly List<IType> types;

    public SingleConfiguration<ConventionBasedCodingStyle, int> MaxFetchDepth { get; }

    public ConventionBasedConfiguration<ConventionBasedCodingStyle, object, IType> Type { get; }
    public ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, bool> TypeIsValue { get; }
    public ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, bool> TypeIsView { get; }
    public ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, IIdExtractor> IdExtractor { get; }
    public ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, IValueExtractor> ValueExtractor { get; }
    public ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, ILocator> Locator { get; }
    public ConventionBasedListConfiguration<ConventionBasedCodingStyle, IType, IConverter> Converters { get; }
    public ConventionBasedListConfiguration<ConventionBasedCodingStyle, IType, object> StaticInstances { get; }
    public ConventionBasedListConfiguration<ConventionBasedCodingStyle, IType, IConstructor> Initializers { get; }
    public ConventionBasedListConfiguration<ConventionBasedCodingStyle, IType, IProperty> Datas { get; }
    public ConventionBasedListConfiguration<ConventionBasedCodingStyle, IType, IMethod> Operations { get; }

    public ConventionBasedConfiguration<ConventionBasedCodingStyle, IProperty, bool> DataFetchedEagerly { get; }

    public ConventionBasedConfiguration<ConventionBasedCodingStyle, IParameter, bool> ParameterIsOptional { get; }
    public ConventionBasedConfiguration<ConventionBasedCodingStyle, IParameter, object> ParameterDefaultValue { get; }

    public ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, string> Module { get; }
    public ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, string> TypeName { get; }
    public ConventionBasedConfiguration<ConventionBasedCodingStyle, IProperty, string> DataName { get; }
    public ConventionBasedConfiguration<ConventionBasedCodingStyle, IMethod, string> OperationName { get; }
    public ConventionBasedConfiguration<ConventionBasedCodingStyle, IParameter, string> ParameterName { get; }

    public ConventionBasedListConfiguration<ConventionBasedCodingStyle, IType, string> TypeMarks { get; }
    public ConventionBasedListConfiguration<ConventionBasedCodingStyle, IConstructor, string> InitializerMarks { get; }
    public ConventionBasedListConfiguration<ConventionBasedCodingStyle, IProperty, string> DataMarks { get; }
    public ConventionBasedListConfiguration<ConventionBasedCodingStyle, IMethod, string> OperationMarks { get; }
    public ConventionBasedListConfiguration<ConventionBasedCodingStyle, IParameter, string> ParameterMarks { get; }

    public ConventionBasedCodingStyle()
    {
        types = new List<IType>();

        MaxFetchDepth = new SingleConfiguration<ConventionBasedCodingStyle, int>(this, nameof(MaxFetchDepth), true);

        Type = new ConventionBasedConfiguration<ConventionBasedCodingStyle, object, IType>(this, nameof(Type));
        TypeIsValue = new ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, bool>(this, nameof(TypeIsValue));
        TypeIsView = new ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, bool>(this, nameof(TypeIsView));
        IdExtractor = new ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, IIdExtractor>(this, nameof(IdExtractor));
        ValueExtractor = new ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, IValueExtractor>(this, nameof(ValueExtractor));
        Locator = new ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, ILocator>(this, nameof(Locator));
        Converters = new ConventionBasedListConfiguration<ConventionBasedCodingStyle, IType, IConverter>(this, nameof(Converters));
        StaticInstances = new ConventionBasedListConfiguration<ConventionBasedCodingStyle, IType, object>(this, nameof(StaticInstances));
        Initializers = new ConventionBasedListConfiguration<ConventionBasedCodingStyle, IType, IConstructor>(this, nameof(Initializers));
        Datas = new ConventionBasedListConfiguration<ConventionBasedCodingStyle, IType, IProperty>(this, nameof(Datas));
        Operations = new ConventionBasedListConfiguration<ConventionBasedCodingStyle, IType, IMethod>(this, nameof(Operations));

        DataFetchedEagerly = new ConventionBasedConfiguration<ConventionBasedCodingStyle, IProperty, bool>(this, nameof(DataFetchedEagerly));

        ParameterIsOptional = new ConventionBasedConfiguration<ConventionBasedCodingStyle, IParameter, bool>(this, nameof(ParameterIsOptional));
        ParameterDefaultValue = new ConventionBasedConfiguration<ConventionBasedCodingStyle, IParameter, object>(this, nameof(ParameterDefaultValue));

        Module = new ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, string>(this, nameof(Module), true);
        TypeName = new ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, string>(this, nameof(TypeName), true);
        DataName = new ConventionBasedConfiguration<ConventionBasedCodingStyle, IProperty, string>(this, nameof(DataName), true);
        OperationName = new ConventionBasedConfiguration<ConventionBasedCodingStyle, IMethod, string>(this, nameof(OperationName), true);
        ParameterName = new ConventionBasedConfiguration<ConventionBasedCodingStyle, IParameter, string>(this, nameof(ParameterName), true);

        TypeMarks = new ConventionBasedListConfiguration<ConventionBasedCodingStyle, IType, string>(this, nameof(TypeMarks));
        InitializerMarks = new ConventionBasedListConfiguration<ConventionBasedCodingStyle, IConstructor, string>(this, nameof(InitializerMarks));
        DataMarks = new ConventionBasedListConfiguration<ConventionBasedCodingStyle, IProperty, string>(this, nameof(DataMarks));
        OperationMarks = new ConventionBasedListConfiguration<ConventionBasedCodingStyle, IMethod, string>(this, nameof(OperationMarks));
        ParameterMarks = new ConventionBasedListConfiguration<ConventionBasedCodingStyle, IParameter, string>(this, nameof(ParameterMarks));
    }

    public ConventionBasedCodingStyle Merge(ConventionBasedCodingStyle other)
    {
        AddTypes(other.types);

        Type.Merge(other.Type);
        TypeIsValue.Merge(other.TypeIsValue);
        TypeIsView.Merge(other.TypeIsView);
        IdExtractor.Merge(other.IdExtractor);
        ValueExtractor.Merge(other.ValueExtractor);
        Locator.Merge(other.Locator);
        Converters.Merge(other.Converters);
        StaticInstances.Merge(other.StaticInstances);
        Initializers.Merge(other.Initializers);
        Datas.Merge(other.Datas);
        Operations.Merge(other.Operations);

        DataFetchedEagerly.Merge(other.DataFetchedEagerly);

        ParameterIsOptional.Merge(other.ParameterIsOptional);
        ParameterDefaultValue.Merge(other.ParameterDefaultValue);

        Module.Merge(other.Module);
        TypeName.Merge(other.TypeName);
        DataName.Merge(other.DataName);
        OperationName.Merge(other.OperationName);
        ParameterName.Merge(other.ParameterName);

        TypeMarks.Merge(other.TypeMarks);
        InitializerMarks.Merge(other.InitializerMarks);
        DataMarks.Merge(other.DataMarks);
        OperationMarks.Merge(other.OperationMarks);
        ParameterMarks.Merge(other.ParameterMarks);

        return this;
    }

    public ConventionBasedCodingStyle RecognizeProxyTypesBy(Func<Type, bool> proxyMatcher, Func<Type, Type> actualTypeGetter)
    {
        TypeInfo.SetProxyMatcher(proxyMatcher, actualTypeGetter);

        return this;
    }

    public ConventionBasedCodingStyle AddTypes(IEnumerable<System.Reflection.Assembly> assemblies) => AddTypes(assemblies, _ => true);
    public ConventionBasedCodingStyle AddTypes(IEnumerable<System.Reflection.Assembly> assemblies, Func<Type, bool> typeFilter)
    {
        foreach (var assembly in assemblies)
        {
            AddTypes(assembly, typeFilter);
        }

        return this;
    }

    public ConventionBasedCodingStyle AddTypes(System.Reflection.Assembly assembly) => AddTypes(assembly, _ => true);
    public ConventionBasedCodingStyle AddTypes(System.Reflection.Assembly assembly, Func<Type, bool> typeFilter) => AddTypes(assembly.GetTypes().Where(typeFilter));
    public ConventionBasedCodingStyle AddTypes(IEnumerable<Type> types) => AddTypes(types.ToArray());
    public ConventionBasedCodingStyle AddTypes(params Type[] types)
    {
        types = types.Where(t => !t.IsByRefLike).ToArray();

        TypeInfo.Optimize(types);

        NeedRefresh();

        AddTypes(types.Select(t => TypeInfo.Get(t) as IType));

        AddNullableTypes(types);

        return this;
    }

    public ConventionBasedCodingStyle AddTypes(params IType[] types) => AddTypes(types.AsEnumerable());
    public ConventionBasedCodingStyle AddTypes(IEnumerable<IType> types)
    {
        foreach (var type in types)
        {
            AddType(type);
        }

        return this;
    }

    private void AddType(IType type)
    {
        if (types.Contains(type)) { return; }

        types.Add(type);

        if (type is VirtualType)
        {
            AddTypes(type.AssignableTypes);
        }
    }

    private bool needsRefresh;
    private void NeedRefresh() { needsRefresh = true; }
    private void RefreshIfNecessary()
    {
        if (!needsRefresh) { return; }

        needsRefresh = false;

        //TODO refactor - TypeInfo should handle this by itself. Proxy instances should be given, so that domain type changes affects immediately
        for (var i = 0; i < types.Count; i++)
        {
            if (types[i] is not TypeInfo type) { continue; }

            types[i] = TypeInfo.Get(type.GetActualType());
        }
    }

    private void AddNullableTypes(Type[] types)
    {
        if (types.Length <= 0) { return; }

        AddTypes(types
            .Where(t => t.IsValueType &&
                        t != typeof(void) &&
                        !(t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>)))
            .Select(t => typeof(Nullable<>).MakeGenericType(t))
        );
    }

    #region ICodingStyle implementation

    int ICodingStyle.GetMaxFetchDepth() => MaxFetchDepth.Get();

    List<IType> ICodingStyle.GetTypes() { RefreshIfNecessary(); return types; }
    bool ICodingStyle.ContainsType(IType type) { RefreshIfNecessary(); return types.Contains(type); }

    IType ICodingStyle.GetType(object @object) => Type.Get(@object);

    bool ICodingStyle.IsValue(IType type) => TypeIsValue.Get(type);
    bool ICodingStyle.IsView(IType type) => TypeIsView.Get(type);
    IIdExtractor ICodingStyle.GetIdExtractor(IType type) => IdExtractor.Get(type);
    IValueExtractor ICodingStyle.GetValueExtractor(IType type) => ValueExtractor.Get(type);
    ILocator ICodingStyle.GetLocator(IType type) => Locator.Get(type);
    List<IConverter> ICodingStyle.GetConverters(IType type) => Converters.Get(type);
    List<object> ICodingStyle.GetStaticInstances(IType type) => StaticInstances.Get(type);
    List<IConstructor> ICodingStyle.GetInitializers(IType type) => Initializers.Get(type);
    List<IProperty> ICodingStyle.GetDatas(IType type) => Datas.Get(type);
    List<IMethod> ICodingStyle.GetOperations(IType type) => Operations.Get(type);

    bool ICodingStyle.IsFetchedEagerly(IProperty property) => DataFetchedEagerly.Get(property);

    bool ICodingStyle.IsOptional(IParameter parameter) => ParameterIsOptional.Get(parameter);
    object ICodingStyle.GetDefaultValue(IParameter parameter) => ParameterDefaultValue.Get(parameter);

    string ICodingStyle.GetModule(IType type) => Module.Get(type);
    string ICodingStyle.GetName(IType type) => TypeName.Get(type);
    string ICodingStyle.GetName(IProperty property) => DataName.Get(property);
    string ICodingStyle.GetName(IMethod method) => OperationName.Get(method);
    string ICodingStyle.GetName(IParameter parameter) => ParameterName.Get(parameter);

    List<string> ICodingStyle.GetMarks(IType type) => TypeMarks.Get(type);
    List<string> ICodingStyle.GetMarks(IConstructor constructor) => InitializerMarks.Get(constructor);
    List<string> ICodingStyle.GetMarks(IProperty property) => DataMarks.Get(property);
    List<string> ICodingStyle.GetMarks(IMethod method) => OperationMarks.Get(method);
    List<string> ICodingStyle.GetMarks(IParameter parameter) => ParameterMarks.Get(parameter);

    #endregion
}
