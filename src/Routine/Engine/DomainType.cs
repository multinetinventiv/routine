using Routine.Core.Configuration;
using Routine.Core;

using static Routine.Constants;

namespace Routine.Engine;

public class DomainType
{
    private readonly ICoreContext ctx;

    public IType Type { get; }

    private readonly List<DomainType> actualTypes;
    private readonly List<DomainType> viewTypes;

    public DomainObjectInitializer Initializer { get; private set; }

    public Dictionary<string, DomainData> Data { get; }
    public ICollection<DomainData> Datas => Data.Values;

    public Dictionary<string, DomainOperation> Operation { get; }
    public ICollection<DomainOperation> Operations => Operation.Values;

    private readonly ILocator locator;
    public IIdExtractor IdExtractor { get; }
    public IValueExtractor ValueExtractor { get; }
    private readonly Dictionary<IType, IConverter> converter;

    private readonly List<object> staticInstances;

    public int MaxFetchDepth { get; }
    public string Id { get; }
    public Marks Marks { get; }
    public string Name { get; }
    public string Module { get; }
    public bool IsValueModel { get; }
    public bool IsViewModel { get; }

    public bool Initializable => Initializer != null;
    public bool Locatable => locator != null;

    public DomainType(ICoreContext ctx, IType type)
    {
        this.ctx = ctx;

        Type = type;
        Data = new();
        Operation = new();

        Marks = new(ctx.CodingStyle.GetMarks(Type));

        Name = ctx.CodingStyle.GetName(Type);
        Module = ctx.CodingStyle.GetModule(Type);

        Id = ctx.BuildTypeId(Module, Name);

        MaxFetchDepth = ctx.CodingStyle.GetMaxFetchDepth();

        locator = ctx.CodingStyle.GetLocator(Type);
        IdExtractor = ctx.CodingStyle.GetIdExtractor(Type);
        ValueExtractor = ctx.CodingStyle.GetValueExtractor(Type);

        converter = new();
        var converters = ctx.CodingStyle.GetConverters(Type);
        foreach (var converterInstance in converters)
        {
            foreach (var targetType in converterInstance.GetTargetTypes(Type))
            {
                if (converter.ContainsKey(targetType))
                {
                    continue;
                }

                converter.Add(targetType, converterInstance);
            }
        }

        staticInstances = ctx.CodingStyle.GetStaticInstances(Type);

        IsValueModel = ctx.CodingStyle.IsValue(Type);
        IsViewModel = ctx.CodingStyle.IsView(Type);

        actualTypes = new();
        viewTypes = new();
    }

    internal void Initialize()
    {
        LoadInitializers();
        LoadDatas();
        LoadOperations();
        LoadCrossTypeRelations();
    }

    private void LoadInitializers()
    {
        foreach (var initializer in ctx.CodingStyle.GetInitializers(Type))
        {
            try
            {
                if (!Type.Equals(initializer.InitializedType))
                {
                    throw new InitializedTypeDoNotMatchException(initializer, Type, initializer.InitializedType);
                }

                if (Initializer == null)
                {
                    Initializer = new DomainObjectInitializer(ctx, initializer);
                }
                else
                {
                    Initializer.AddGroup(initializer);
                }
            }
            catch (TypeNotConfiguredException) { }
            catch (InitializedTypeDoNotMatchException) { }
            catch (ParameterTypesDoNotMatchException) { }
            catch (IdenticalSignatureAlreadyAddedException) { }
        }
    }

    private void LoadDatas()
    {
        foreach (var data in ctx.CodingStyle.GetDatas(Type))
        {
            try
            {
                Data.Add(data.Name, new DomainData(ctx, data));
            }
            catch (TypeNotConfiguredException)
            {
            }
        }
    }

    private void LoadOperations()
    {
        foreach (var operation in ctx.CodingStyle.GetOperations(Type))
        {
            try
            {
                if (Operation.ContainsKey(operation.Name))
                {
                    Operation[operation.Name].AddGroup(operation);
                }
                else
                {
                    Operation.Add(operation.Name, new DomainOperation(ctx, operation));
                }
            }
            catch (TypeNotConfiguredException) { }
            catch (ReturnTypesDoNotMatchException) { }
            catch (ParameterTypesDoNotMatchException) { }
            catch (IdenticalSignatureAlreadyAddedException) { }
        }
    }

    private void LoadCrossTypeRelations()
    {
        foreach (var viewType in converter.Keys.Where(t => !Equals(t, Type)))
        {
            if (!ctx.CodingStyle.ContainsType(viewType))
            {
                continue;
            }

            var dt = ctx.GetDomainType(viewType);

            if (!IsViewModel)
            {
                dt.actualTypes.Add(this);
            }

            viewTypes.Add(dt);
        }
    }

    public bool MarkedAs(string mark) => Marks.Has(mark);

    public ObjectModel GetModel() =>
        new()
        {
            Id = Id,
            Marks = Marks.Set,
            Name = Name,
            Module = Module,
            IsViewModel = IsViewModel,
            IsValueModel = IsValueModel,
            ActualModelIds = actualTypes.Select(t => t.Id).ToList(),
            ViewModelIds = viewTypes.Select(t => t.Id).ToList(),
            Initializer = Initializer != null ? Initializer.GetModel() : new InitializerModel(),
            Datas = Datas.Select(m => m.GetModel()).ToList(),
            Operations = Operations.Select(o => o.GetModel()).ToList(),
            StaticInstances = staticInstances.Select(o => ctx.CreateDomainObject(o, this).GetObjectData(false)).ToList()
        };

    internal async Task<object> LocateAsync(ParameterData parameterData) => (await LocateManyAsync(new List<ParameterData> { parameterData })).First();

    internal async Task<List<object>> LocateManyAsync(List<ParameterData> parameterDatas)
    {
        var result = new object[parameterDatas.Count];

        var locateIdsWithOriginalIndex = new List<Tuple<int, string>>();

        for (var i = 0; i < parameterDatas.Count; i++)
        {
            var parameterData = parameterDatas[i];
            if (Initializable && parameterData != null && string.IsNullOrEmpty(parameterData.Id))
            {
                result[i] = await Initializer.InitializeAsync(parameterData.InitializationParameters);
            }
            else if (parameterData == null)
            {
                result[i] = null;
            }
            else
            {
                locateIdsWithOriginalIndex.Add(new Tuple<int, string>(i, parameterData.Id));
            }
        }

        if (locateIdsWithOriginalIndex.Any())
        {
            var locateIds = locateIdsWithOriginalIndex.Select(t => t.Item2).ToList();
            var located = await LocateManyAsync(locateIds);

            if (located.Count != locateIdsWithOriginalIndex.Count)
            {
                throw new InvalidOperationException(
                    $"Locator returned a result with different number of objects ({located.Count}) than given number of ids ({locateIds.Count}) when locating ids {locateIds.ToItemString()} of type {Type}");
            }

            for (var i = 0; i < located.Count; i++)
            {
                result[locateIdsWithOriginalIndex[i].Item1] = located[i];
            }
        }

        return result.ToList();
    }

    public async Task<object> LocateAsync(ReferenceData referenceData) => referenceData == null ? null : await LocateAsync(referenceData.Id);
    public async Task<object> LocateAsync(string id) => (await LocateManyAsync(new List<string> { id })).First();

    public async Task<List<object>> LocateManyAsync(List<string> ids)
    {
        if (!Locatable) { throw new CannotLocateException(Type, ids); }
        if (!ids.Any()) { return new(); }

        var notNullIds = ids.Select(id => id ?? string.Empty).ToList();

        return await locator.LocateAsync(Type, notNullIds);
    }

    public object Convert(object target, DomainType viewDomainType)
    {
        if (Equals(viewDomainType)) { return target; }
        if (Id == MODEL_ID_NULL) { return target; }

        if (!viewTypes.Contains(viewDomainType))
        {
            throw new ConfigurationException("Converter", Type, new CannotConvertException(target, viewDomainType.Type));
        }

        if (!converter.ContainsKey(viewDomainType.Type))
        {
            throw new ConfigurationException("Converter", Type);
        }

        return converter[viewDomainType.Type].Convert(target, Type, viewDomainType.Type);
    }

    #region Formatting & Equality

    protected bool Equals(DomainType other)
    {
        return string.Equals(Id, other.Id);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((DomainType)obj);
    }

    public override int GetHashCode()
    {
        return (Id != null ? Id.GetHashCode() : 0);
    }

    public override string ToString()
    {
        return Id;
    }

    #endregion
}

internal class TypeNotConfiguredException : Exception
{
    public TypeNotConfiguredException(IType type)
        : base($"Type '{(type == null ? "null" : type.ToString())}' is not configured.") { }
}
