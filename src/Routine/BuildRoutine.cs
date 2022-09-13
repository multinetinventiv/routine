using Routine.Client;
using Routine.Core.Configuration.Convention;
using Routine.Core.Configuration;
using Routine.Core;
using Routine.Engine.Configuration.ConventionBased;
using Routine.Engine.Configuration;
using Routine.Engine.Extractor;
using Routine.Engine.Virtual;
using Routine.Engine;
using Routine.Interception.Configuration;
using Routine.Interception;
using Routine.Service.Configuration;
using Routine.Service;

namespace Routine;

public static class BuildRoutine
{
    public static ContextBuilder Context() => new();
    public static CodingStyleBuilder CodingStyle() => new();
    internal static PatternBuilder<ConventionBasedCodingStyle> CodingStylePattern() => new();
    public static InterceptionConfigurationBuilder InterceptionConfig() => new();
    internal static PatternBuilder<ConventionBasedInterceptionConfiguration> InterceptionPattern() => new();
    public static ServiceConfigurationBuilder ServiceConfig() => new();
    internal static PatternBuilder<ConventionBasedServiceConfiguration> ServicePattern() => new();
    public static ServiceClientConfigurationBuilder ServiceClientConfig() => new();
    internal static PatternBuilder<ConventionBasedServiceClientConfiguration> ServiceClientPattern() => new();
    public static LocatorBuilder Locator() => new();
    public static ConventionBuilder<TFrom, TData> Convention<TFrom, TData>() => new();
    public static IdExtractorBuilder IdExtractor() => new();
    public static ValueExtractorBuilder ValueExtractor() => new();
    public static ConverterBuilder Converter() => new();
    public static InterceptorBuilder<TContext> Interceptor<TContext>() where TContext : InterceptionContext => new();
    public static VirtualTypeBuilder VirtualType() => new();
    public static MethodBuilder Method(IType parentType) => new(parentType);
    public static ParameterBuilder Parameter(IParametric owner) => new(owner);
    public static HeaderProcessorBuilder HeaderProcessor() => new();
}

public static class BuildRoutineExtensions
{
    #region ContextBuilder

    public static IClientContext AsClientApplication(
        this ContextBuilder source,
        Func<CodingStyleBuilder, ICodingStyle> codingStyle
    ) => source.AsClientApplication(
        codingStyle(BuildRoutine.CodingStyle())
    );

    public static IServiceContext AsServiceApplication(
        this ContextBuilder source,
        Func<ServiceConfigurationBuilder, IServiceConfiguration> serviceConfiguration,
        Func<CodingStyleBuilder, ICodingStyle> codingStyle
    ) => source.AsServiceApplication(
        serviceConfiguration(BuildRoutine.ServiceConfig()),
        codingStyle(BuildRoutine.CodingStyle())
    );

    public static IClientContext AsServiceClient(
        this ContextBuilder source,
        Func<ServiceClientConfigurationBuilder, IServiceClientConfiguration> serviceClientConfiguration
    ) => source.AsServiceClient(
        serviceClientConfiguration(BuildRoutine.ServiceClientConfig())
    );

    #endregion

    #region IObjectService

    public static IObjectService Intercept(this IObjectService source, IInterceptionConfiguration interceptionConfiguration)
    {
        if (interceptionConfiguration == null) { return source; }

        return new InterceptedObjectService(source, interceptionConfiguration);
    }

    #endregion

    #region Convention

    public static TConfiguration Set<TConfiguration, TFrom, TResult>(
        this ConventionBasedConfiguration<TConfiguration, TFrom, TResult> source,
        Func<ConventionBuilder<TFrom, TResult>, IConvention<TFrom, TResult>> conventionDelegate
    ) where TConfiguration : ILayered =>
        source.Set(conventionDelegate(BuildRoutine.Convention<TFrom, TResult>()));

    public static TConfiguration Add<TConfiguration, TFrom, TItem>(
        this ConventionBasedListConfiguration<TConfiguration, TFrom, TItem> source,
        Func<ConventionBuilder<TFrom, List<TItem>>, IConvention<TFrom, List<TItem>>> conventionDelegate
    ) where TConfiguration : ILayered =>
        source.Add(conventionDelegate(BuildRoutine.Convention<TFrom, List<TItem>>()));

    public static ConventionBase<IType, List<IConstructor>> Constructors(this ConventionBuilder<IType, List<IConstructor>> source) => source.Constructors(_ => true);
    public static ConventionBase<IType, List<IConstructor>> Constructors(this ConventionBuilder<IType, List<IConstructor>> source, Func<IConstructor, bool> filter) =>
        source.By(t => t.Constructors.Where(filter).ToList());

    public static ConventionBase<IType, List<IConstructor>> PublicConstructors(this ConventionBuilder<IType, List<IConstructor>> source) => source.PublicConstructors(_ => true);
    public static ConventionBase<IType, List<IConstructor>> PublicConstructors(this ConventionBuilder<IType, List<IConstructor>> source, Func<IConstructor, bool> filter) =>
        source.Constructors(i => i.IsPublic && filter(i));

    public static ConventionBase<IType, List<IProperty>> Properties(this ConventionBuilder<IType, List<IProperty>> source) => source.Properties(_ => true);
    public static ConventionBase<IType, List<IProperty>> Properties(this ConventionBuilder<IType, List<IProperty>> source, Func<IProperty, bool> filter) =>
        source.By(t => t.Properties.Where(filter).ToList());

    public static ConventionBase<IType, List<IProperty>> PublicProperties(this ConventionBuilder<IType, List<IProperty>> source) => source.PublicProperties(_ => true);
    public static ConventionBase<IType, List<IProperty>> PublicProperties(this ConventionBuilder<IType, List<IProperty>> source, Func<IProperty, bool> filter) =>
        source.Properties(m => m.IsPublic && filter(m));

    public static ConventionBase<IType, List<IProperty>> Methods(this ConventionBuilder<IType, List<IProperty>> source, Func<IMethod, bool> filter) => source.Methods(string.Empty, filter);
    public static ConventionBase<IType, List<IProperty>> Methods(this ConventionBuilder<IType, List<IProperty>> source, string ignorePrefix, Func<IMethod, bool> filter) =>
        source.By(t => t
            .Methods
            .Where(filter)
            .Select(m => new MethodAsProperty(m, ignorePrefix) as IProperty)
            .ToList()
        );

    public static ConventionBase<IType, List<IProperty>> PublicMethods(this ConventionBuilder<IType, List<IProperty>> source, Func<IMethod, bool> filter) => source.PublicMethods(string.Empty, filter);
    public static ConventionBase<IType, List<IProperty>> PublicMethods(this ConventionBuilder<IType, List<IProperty>> source, string ignorePrefix, Func<IMethod, bool> filter) =>
        source.Methods(ignorePrefix, o => o.IsPublic && filter(o));

    public static ConventionBase<IType, List<IMethod>> Methods(this ConventionBuilder<IType, List<IMethod>> source) => source.Methods(_ => true);
    public static ConventionBase<IType, List<IMethod>> Methods(this ConventionBuilder<IType, List<IMethod>> source, Func<IMethod, bool> filter) =>
        source.By(t => t.Methods.Where(filter).ToList());

    public static ConventionBase<IType, List<IMethod>> PublicMethods(this ConventionBuilder<IType, List<IMethod>> source) => source.PublicMethods(_ => true);
    public static ConventionBase<IType, List<IMethod>> PublicMethods(this ConventionBuilder<IType, List<IMethod>> source, Func<IMethod, bool> filter) =>
        source.Methods(o => o.IsPublic && filter(o));

    public static ConventionBase<IType, List<IMethod>> Properties(this ConventionBuilder<IType, List<IMethod>> source, Func<IProperty, bool> filter) => source.Properties(Constants.PROPERTY_AS_METHOD_DEFAULT_PREFIX, filter);
    public static ConventionBase<IType, List<IMethod>> Properties(this ConventionBuilder<IType, List<IMethod>> source, string namePrefix, Func<IProperty, bool> propertyFilter) =>
        source.By(t => t
            .Properties
            .Where(propertyFilter)
            .Select(m => new PropertyAsMethod(m, namePrefix) as IMethod)
            .ToList()
        );

    public static ConventionBase<IType, List<IMethod>> PublicProperties(this ConventionBuilder<IType, List<IMethod>> source, Func<IProperty, bool> filter) => source.PublicProperties(Constants.PROPERTY_AS_METHOD_DEFAULT_PREFIX, filter);
    public static ConventionBase<IType, List<IMethod>> PublicProperties(this ConventionBuilder<IType, List<IMethod>> source, string namePrefix, Func<IProperty, bool> filter) =>
        source.Properties(namePrefix, m => m.IsPublic && filter(m));

    public static ConventionBase<TFrom, List<TResultItem>> Constant<TFrom, TResultItem>(
        this ConventionBuilder<TFrom, List<TResultItem>> source,
        params TResultItem[] items
    ) => source.Constant(items.ToList());

    public static ConventionBase<TFrom, List<TResultItem>> By<TFrom, TResultItem>(
        this ConventionBuilder<TFrom, List<TResultItem>> source,
        Func<TFrom, TResultItem> converterDelegate
    ) => source.By(o => new List<TResultItem> { converterDelegate(o) });

    public static ConventionBase<TFrom, ILocator> Locator<TFrom>(
        this ConventionBuilder<TFrom, ILocator> source,
        Func<LocatorBuilder, ILocator> locatorDelegate
    ) => source.Constant(locatorDelegate(BuildRoutine.Locator()));

    public static ConventionBase<TFrom, IIdExtractor> Id<TFrom>(
        this ConventionBuilder<TFrom, IIdExtractor> source,
        Func<IdExtractorBuilder, IIdExtractor> idExtractorDelegate
    ) => source.Constant(idExtractorDelegate(BuildRoutine.IdExtractor()));

    public static ConventionBase<TFrom, IValueExtractor> Value<TFrom>(
        this ConventionBuilder<TFrom, IValueExtractor> source,
        Func<ValueExtractorBuilder, IValueExtractor> valueExtractorDelegate
    ) => source.Constant(valueExtractorDelegate(BuildRoutine.ValueExtractor()));

    public static ConventionBase<TFrom, List<IConverter>> Convert<TFrom>(
        this ConventionBuilder<TFrom, List<IConverter>> source,
        Func<ConverterBuilder, IConverter> converterDelegate
    ) => source.Constant(converterDelegate(BuildRoutine.Converter()));

    public static ConventionBase<TFrom, List<IInterceptor<TContext>>> Interceptor<TFrom, TContext>(
        this ConventionBuilder<TFrom, List<IInterceptor<TContext>>> source,
        Func<InterceptorBuilder<TContext>, IInterceptor<TContext>> interceptorDelegate)
        where TContext : InterceptionContext =>
        source.Constant(interceptorDelegate(BuildRoutine.Interceptor<TContext>()));

    #region IdByProperty

    public static ConventionBase<IType, IIdExtractor> IdByPublicProperty(
        this ConventionBuilder<IType, IIdExtractor> source, Func<IProperty, bool> filter
    ) => source.IdByPublicProperty(filter, e => e);

    public static ConventionBase<IType, IIdExtractor> IdByPublicProperty(
        this ConventionBuilder<IType, IIdExtractor> source, Func<IProperty, bool> filter,
        Func<PropertyValueExtractor, PropertyValueExtractor> configurationDelegate
    ) => source.IdByProperty(filter.And(m => m.IsPublic), configurationDelegate);

    public static ConventionBase<IType, IIdExtractor> IdByProperty(
        this ConventionBuilder<IType, IIdExtractor> source, Func<IProperty, bool> filter
    ) => source.IdByProperty(filter, e => e);

    public static ConventionBase<IType, IIdExtractor> IdByProperty(
        this ConventionBuilder<IType, IIdExtractor> source, Func<IProperty, bool> filter,
        Func<PropertyValueExtractor, PropertyValueExtractor> configurationDelegate
    ) => source
        .By(t => configurationDelegate(BuildRoutine.IdExtractor().ByPropertyValue(t.Properties.First(filter))))
        .When(t => t.Properties.Any(filter));

    #endregion

    #region IdByMethod

    public static ConventionBase<IType, IIdExtractor> IdByPublicMethod(
        this ConventionBuilder<IType, IIdExtractor> source, Func<IMethod, bool> filter
    ) => source.IdByPublicMethod(filter, e => e);

    public static ConventionBase<IType, IIdExtractor> IdByPublicMethod(
        this ConventionBuilder<IType, IIdExtractor> source, Func<IMethod, bool> filter,
        Func<PropertyValueExtractor, PropertyValueExtractor> configurationDelegate
    ) => source.IdByMethod(filter.And(o => o.IsPublic), configurationDelegate);

    public static ConventionBase<IType, IIdExtractor> IdByMethod(
        this ConventionBuilder<IType, IIdExtractor> source, Func<IMethod, bool> filter
    ) => source.IdByMethod(filter, e => e);

    public static ConventionBase<IType, IIdExtractor> IdByMethod(
        this ConventionBuilder<IType, IIdExtractor> source, Func<IMethod, bool> filter,
        Func<PropertyValueExtractor, PropertyValueExtractor> configurationDelegate
    ) => source.By(t => configurationDelegate(BuildRoutine.IdExtractor()
            .ByPropertyValue(new MethodAsProperty(
                t.Methods.First(filter.And(o => o.HasNoParameters() && !o.ReturnsVoid()))
            ))
        )).When(t => t.Methods.Any(filter.And(o => o.HasNoParameters() && !o.ReturnsVoid())))
        as DelegateBasedConvention<IType, IIdExtractor>;

    #endregion

    #region ValueByProperty

    public static ConventionBase<IType, IValueExtractor> ValueByPublicProperty(
        this ConventionBuilder<IType, IValueExtractor> source, Func<IProperty, bool> filter
    ) => source.ValueByPublicProperty(filter, e => e);

    public static ConventionBase<IType, IValueExtractor> ValueByPublicProperty(
        this ConventionBuilder<IType, IValueExtractor> source, Func<IProperty, bool> filter,
        Func<PropertyValueExtractor, PropertyValueExtractor> configurationDelegate
    ) => source.ValueByProperty(filter.And(m => m.IsPublic), configurationDelegate);

    public static ConventionBase<IType, IValueExtractor> ValueByProperty(
        this ConventionBuilder<IType, IValueExtractor> source, Func<IProperty, bool> filter
    ) => source.ValueByProperty(filter, e => e);

    public static ConventionBase<IType, IValueExtractor> ValueByProperty(
        this ConventionBuilder<IType, IValueExtractor> source, Func<IProperty, bool> filter,
        Func<PropertyValueExtractor, PropertyValueExtractor> configurationDelegate
    ) => source
        .By(t => configurationDelegate(BuildRoutine.ValueExtractor().ByPropertyValue(t.Properties.First(filter))))
        .When(t => t.Properties.Any(filter));

    #endregion

    #region ValueByMethod

    public static ConventionBase<IType, IValueExtractor> ValueByPublicMethod(
        this ConventionBuilder<IType, IValueExtractor> source, Func<IMethod, bool> filter
    ) => source.ValueByPublicMethod(filter.And(o => o.IsPublic), e => e);

    public static ConventionBase<IType, IValueExtractor> ValueByPublicMethod(
        this ConventionBuilder<IType, IValueExtractor> source, Func<IMethod, bool> filter,
        Func<PropertyValueExtractor, PropertyValueExtractor> configurationDelegate
    ) => source.ValueByMethod(filter.And(o => o.IsPublic), configurationDelegate);

    public static ConventionBase<IType, IValueExtractor> ValueByMethod(
        this ConventionBuilder<IType, IValueExtractor> source, Func<IMethod, bool> filter
    ) => source.ValueByMethod(filter, e => e);

    public static ConventionBase<IType, IValueExtractor> ValueByMethod(
        this ConventionBuilder<IType, IValueExtractor> source, Func<IMethod, bool> filter,
        Func<PropertyValueExtractor, PropertyValueExtractor> configurationDelegate
    ) => source
        .By(t => configurationDelegate(BuildRoutine.ValueExtractor()
            .ByPropertyValue(new MethodAsProperty(
                t.Methods.First(filter.And(o => o.HasNoParameters() && !o.ReturnsVoid()))
            ))
        ))
        .When(t => t.Methods.Any(filter.And(o => o.HasNoParameters() && !o.ReturnsVoid())));

    #endregion

    #endregion

    #region PatternBuilder

    public static ConventionBasedCodingStyle Use(this ConventionBasedCodingStyle source,
        Func<PatternBuilder<ConventionBasedCodingStyle>, ConventionBasedCodingStyle> pattern
    ) => source.Merge(pattern(BuildRoutine.CodingStylePattern()));

    public static ConventionBasedInterceptionConfiguration Use(this ConventionBasedInterceptionConfiguration source,
        Func<PatternBuilder<ConventionBasedInterceptionConfiguration>, ConventionBasedInterceptionConfiguration> pattern
    ) => source.Merge(pattern(BuildRoutine.InterceptionPattern()));

    public static ConventionBasedServiceConfiguration Use(this ConventionBasedServiceConfiguration source,
        Func<PatternBuilder<ConventionBasedServiceConfiguration>, ConventionBasedServiceConfiguration> pattern
    ) => source.Merge(pattern(BuildRoutine.ServicePattern()));

    public static ConventionBasedServiceClientConfiguration Use(this ConventionBasedServiceClientConfiguration source,
        Func<PatternBuilder<ConventionBasedServiceClientConfiguration>, ConventionBasedServiceClientConfiguration> pattern
    ) => source.Merge(pattern(BuildRoutine.ServiceClientPattern()));

    #endregion

    #region Virtual

    public static ConventionBasedCodingStyle AddTypes(this ConventionBasedCodingStyle source,
        params Func<VirtualTypeBuilder, VirtualType>[] typeBuilders
    ) => source.AddTypes(typeBuilders.Select(builder => builder(BuildRoutine.VirtualType())));

    public static TConfiguration Add<TConfiguration>(this ListConfiguration<TConfiguration, VirtualType> source,
        params Func<VirtualTypeBuilder, VirtualType>[] typeBuilders
    ) => source.Add(
        typeBuilders.Select(builder =>
            builder(BuildRoutine.VirtualType())
        )
    );

    public static TConfiguration Add<TConfiguration>(this ListConfiguration<TConfiguration, IMethod> source,
        Func<MethodBuilder, IEnumerable<IMethod>> builder
    ) where TConfiguration : IType =>
        source.Add(t => builder(BuildRoutine.Method(t)));

    public static TConfiguration Add<TConfiguration>(this ListConfiguration<TConfiguration, IMethod> source,
        Func<MethodBuilder, IMethod> builder
    ) where TConfiguration : IType =>
        source.Add(t => builder(BuildRoutine.Method(t)));

    public static TConfiguration Add<TConfiguration>(this ListConfiguration<TConfiguration, IParameter> source,
        Func<ParameterBuilder, IEnumerable<IParameter>> builder
    ) where TConfiguration : IParametric =>
        source.Add(o => builder(BuildRoutine.Parameter(o)));

    public static TConfiguration Add<TConfiguration>(this ListConfiguration<TConfiguration, IParameter> source,
        Func<ParameterBuilder, IParameter> builder
    ) where TConfiguration : IParametric =>
        source.Add(o => builder(BuildRoutine.Parameter(o)));

    public static ConventionBase<IType, List<IMethod>> Build(this ConventionBuilder<IType, List<IMethod>> source,
        Func<MethodBuilder, IEnumerable<IMethod>> builder
    ) => source.By(t => builder(BuildRoutine.Method(t)).ToList());

    public static ConventionBase<IType, List<IMethod>> Build(this ConventionBuilder<IType, List<IMethod>> source,
        Func<MethodBuilder, IMethod> builder
    ) => source.By(t => new List<IMethod> { builder(BuildRoutine.Method(t)) });

    #endregion

    #region Add System Types

    public static ConventionBasedCodingStyle AddCommonSystemTypes(this ConventionBasedCodingStyle source) =>
        source.AddTypes(
            typeof(void),
            typeof(bool),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(TimeSpan),
            typeof(DateTime),
            typeof(Guid),
            typeof(string)
        );

    #endregion

    #region Header Processor

    public static TConfiguration Add<TConfiguration>(
        this ListConfiguration<TConfiguration, IHeaderProcessor> source,
        Func<HeaderProcessorBuilder, IHeaderProcessor> builder
    ) => source.Add(builder(BuildRoutine.HeaderProcessor()));

    #endregion
}
}
