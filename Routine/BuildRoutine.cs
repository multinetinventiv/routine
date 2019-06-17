using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Api.Configuration;
using Routine.Core.Configuration;
using Routine.Core.Configuration.Convention;
using Routine.Engine;
using Routine.Engine.Configuration;
using Routine.Engine.Configuration.ConventionBased;
using Routine.Engine.Extractor;
using Routine.Engine.Virtual;
using Routine.Interception;
using Routine.Interception.Configuration;
using Routine.Service;
using Routine.Service.Configuration;
using Routine.Ui.Configuration;

namespace Routine
{
	public static class BuildRoutine
	{
		public static ContextBuilder Context()
		{
			return new ContextBuilder();
		}

		public static CodingStyleBuilder CodingStyle()
		{
			return new CodingStyleBuilder();
		}

		internal static PatternBuilder<ConventionBasedCodingStyle> CodingStylePattern()
		{
			return new PatternBuilder<ConventionBasedCodingStyle>();
		}

		public static InterceptionConfigurationBuilder InterceptionConfig()
		{
			return new InterceptionConfigurationBuilder();
		}

		internal static PatternBuilder<ConventionBasedInterceptionConfiguration> InterceptionPattern()
		{
			return new PatternBuilder<ConventionBasedInterceptionConfiguration>();
		}

		public static ApiConfigurationBuilder ApiConfig()
		{
			return new ApiConfigurationBuilder();
		}

		internal static PatternBuilder<ConventionBasedApiConfiguration> ApiGenerationPattern()
		{
			return new PatternBuilder<ConventionBasedApiConfiguration>();
		}

		public static MvcConfigurationBuilder MvcConfig()
		{
			return new MvcConfigurationBuilder();
		}

		internal static PatternBuilder<ConventionBasedMvcConfiguration> MvcPattern()
		{
			return new PatternBuilder<ConventionBasedMvcConfiguration>();
		}

		public static ServiceConfigurationBuilder ServiceConfig()
		{
			return new ServiceConfigurationBuilder();
		}

		internal static PatternBuilder<ConventionBasedServiceConfiguration> ServicePattern()
		{
			return new PatternBuilder<ConventionBasedServiceConfiguration>();
		}

		public static ServiceClientConfigurationBuilder ServiceClientConfig()
		{
			return new ServiceClientConfigurationBuilder();
		}

		internal static PatternBuilder<ConventionBasedServiceClientConfiguration> ServiceClientPattern()
		{
			return new PatternBuilder<ConventionBasedServiceClientConfiguration>();
		}

		public static LocatorBuilder Locator()
		{
			return new LocatorBuilder();
		}

		public static ConventionBuilder<TFrom, TData> Convention<TFrom, TData>()
		{
			return new ConventionBuilder<TFrom, TData>();
		}

		public static IdExtractorBuilder IdExtractor()
		{
			return new IdExtractorBuilder();
		}

		public static ValueExtractorBuilder ValueExtractor()
		{
			return new ValueExtractorBuilder();
		}

		public static ConverterBuilder Converter()
		{
			return new ConverterBuilder();
		}

		public static InterceptorBuilder<TContext> Interceptor<TContext>()
			where TContext : InterceptionContext
		{
			return new InterceptorBuilder<TContext>();
		}

		public static VirtualTypeBuilder VirtualType()
		{
			return new VirtualTypeBuilder();
		}

		public static MethodBuilder Method(IType parentType)
		{
			return new MethodBuilder(parentType);
		}

		public static ParameterBuilder Parameter(IParametric owner)
		{
			return new ParameterBuilder(owner);
		}

		public static HeaderProcessorBuilder HeaderProcessor()
		{
			return new HeaderProcessorBuilder();
		}
	}

	public static class BuildRoutineExtensions
	{
		#region Convention

		public static TConfiguration Set<TConfiguration, TFrom, TResult>(
			this ConventionBasedConfiguration<TConfiguration, TFrom, TResult> source,
			Func<ConventionBuilder<TFrom, TResult>, IConvention<TFrom, TResult>> conventionDelegate) where TConfiguration : ILayered
		{
			return source.Set(conventionDelegate(BuildRoutine.Convention<TFrom, TResult>()));
		}

		public static TConfiguration Add<TConfiguration, TFrom, TItem>(
			this ConventionBasedListConfiguration<TConfiguration, TFrom, TItem> source,
			Func<ConventionBuilder<TFrom, List<TItem>>, IConvention<TFrom, List<TItem>>> conventionDelegate) where TConfiguration : ILayered
		{
			return source.Add(conventionDelegate(BuildRoutine.Convention<TFrom, List<TItem>>()));
		}

		public static ConventionBase<IType, List<IConstructor>> Constructors(this ConventionBuilder<IType, List<IConstructor>> source) { return source.Constructors(i => true); }
		public static ConventionBase<IType, List<IConstructor>> Constructors(this ConventionBuilder<IType, List<IConstructor>> source, Func<IConstructor, bool> filter)
		{
			return source.By(t => t.Constructors.Where(filter).ToList());
		}

		public static ConventionBase<IType, List<IConstructor>> PublicConstructors(this ConventionBuilder<IType, List<IConstructor>> source) { return source.PublicConstructors(i => true); }
		public static ConventionBase<IType, List<IConstructor>> PublicConstructors(this ConventionBuilder<IType, List<IConstructor>> source, Func<IConstructor, bool> filter)
		{
			return source.Constructors(i => i.IsPublic && filter(i));
		}

		public static ConventionBase<IType, List<IProperty>> Properties(this ConventionBuilder<IType, List<IProperty>> source) { return source.Properties(m => true); }
		public static ConventionBase<IType, List<IProperty>> Properties(this ConventionBuilder<IType, List<IProperty>> source, Func<IProperty, bool> filter)
		{
			return source.By(t => t.Properties.Where(filter).ToList());
		}

		public static ConventionBase<IType, List<IProperty>> PublicProperties(this ConventionBuilder<IType, List<IProperty>> source) { return source.PublicProperties(m => true); }
		public static ConventionBase<IType, List<IProperty>> PublicProperties(this ConventionBuilder<IType, List<IProperty>> source, Func<IProperty, bool> filter)
		{
			return source.Properties(m => m.IsPublic && filter(m));
		}

		public static ConventionBase<IType, List<IProperty>> Methods(this ConventionBuilder<IType, List<IProperty>> source, Func<IMethod, bool> filter) { return source.Methods(string.Empty, filter); }
		public static ConventionBase<IType, List<IProperty>> Methods(this ConventionBuilder<IType, List<IProperty>> source, string ignorePrefix, Func<IMethod, bool> filter)
		{
			return source.By(t => t
				.Methods
				.Where(filter)
				.Select(m => new MethodAsProperty(m, ignorePrefix) as IProperty)
				.ToList()
			);
		}

		public static ConventionBase<IType, List<IProperty>> PublicMethods(this ConventionBuilder<IType, List<IProperty>> source, Func<IMethod, bool> filter) { return source.PublicMethods(string.Empty, filter); }
		public static ConventionBase<IType, List<IProperty>> PublicMethods(this ConventionBuilder<IType, List<IProperty>> source, string ignorePrefix, Func<IMethod, bool> filter)
		{
			return source.Methods(o => o.IsPublic && filter(o));
		}

		public static ConventionBase<IType, List<IMethod>> Methods(this ConventionBuilder<IType, List<IMethod>> source) { return source.Methods(o => true); }
		public static ConventionBase<IType, List<IMethod>> Methods(this ConventionBuilder<IType, List<IMethod>> source, Func<IMethod, bool> filter)
		{
			return source.By(t => t.Methods.Where(filter).ToList());
		}

		public static ConventionBase<IType, List<IMethod>> PublicMethods(this ConventionBuilder<IType, List<IMethod>> source) { return source.PublicMethods(o => true); }
		public static ConventionBase<IType, List<IMethod>> PublicMethods(this ConventionBuilder<IType, List<IMethod>> source, Func<IMethod, bool> filter)
		{
			return source.Methods(o => o.IsPublic && filter(o));
		}

		public static ConventionBase<IType, List<IMethod>> Properties(this ConventionBuilder<IType, List<IMethod>> source, Func<IProperty, bool> filter) { return source.Properties(Constants.PROPERTY_AS_METHOD_DEFAULT_PREFIX, filter); }
		public static ConventionBase<IType, List<IMethod>> Properties(this ConventionBuilder<IType, List<IMethod>> source, string namePrefix, Func<IProperty, bool> propertyFilter)
		{
			return source.By(t => t
				.Properties
				.Where(propertyFilter)
				.Select(m => new PropertyAsMethod(m, namePrefix) as IMethod)
				.ToList()
			);
		}

		public static ConventionBase<IType, List<IMethod>> PublicProperties(this ConventionBuilder<IType, List<IMethod>> source, Func<IProperty, bool> filter) { return source.PublicProperties(Constants.PROPERTY_AS_METHOD_DEFAULT_PREFIX, filter); }
		public static ConventionBase<IType, List<IMethod>> PublicProperties(this ConventionBuilder<IType, List<IMethod>> source, string namePrefix, Func<IProperty, bool> filter)
		{
			return source.Properties(namePrefix, m => m.IsPublic && filter(m));
		}

		public static ConventionBase<TFrom, List<TResultItem>> Constant<TFrom, TResultItem>(
			this ConventionBuilder<TFrom, List<TResultItem>> source,
			params TResultItem[] items)
		{
			return source.Constant(items.ToList());
		}

		public static ConventionBase<TFrom, List<TResultItem>> By<TFrom, TResultItem>(
			this ConventionBuilder<TFrom, List<TResultItem>> source,
			Func<TFrom, TResultItem> converterDelegate)
		{
			return source.By(o => new List<TResultItem>{converterDelegate(o)});
		}

		public static ConventionBase<TFrom, ILocator> Locator<TFrom>(
			this ConventionBuilder<TFrom, ILocator> source,
			Func<LocatorBuilder, ILocator> locatorDelegate)
		{
			return source.Constant(locatorDelegate(BuildRoutine.Locator()));
		}

		public static ConventionBase<TFrom, IIdExtractor> Id<TFrom>(
			this ConventionBuilder<TFrom, IIdExtractor> source,
			Func<IdExtractorBuilder, IIdExtractor> idExtractorDelegate)
		{
			return source.Constant(idExtractorDelegate(BuildRoutine.IdExtractor()));
		}

		public static ConventionBase<TFrom, IValueExtractor> Value<TFrom>(
			this ConventionBuilder<TFrom, IValueExtractor> source,
			Func<ValueExtractorBuilder, IValueExtractor> valueExtractorDelegate)
		{
			return source.Constant(valueExtractorDelegate(BuildRoutine.ValueExtractor()));
		}

		public static ConventionBase<TFrom, List<IConverter>> Convert<TFrom>(
			this ConventionBuilder<TFrom, List<IConverter>> source,
			Func<ConverterBuilder, IConverter> converterDelegate)
		{
			return source.Constant(converterDelegate(BuildRoutine.Converter()));
		}

		public static ConventionBase<TFrom, List<IInterceptor<TContext>>> Interceptor<TFrom, TContext>(
			this ConventionBuilder<TFrom, List<IInterceptor<TContext>>> source,
			Func<InterceptorBuilder<TContext>, IInterceptor<TContext>> interceptorDelegate)
			where TContext : InterceptionContext
		{
			return source.Constant(interceptorDelegate(BuildRoutine.Interceptor<TContext>()));
		}

		#region IdByProperty

		public static ConventionBase<IType, IIdExtractor> IdByPublicProperty(
			this ConventionBuilder<IType, IIdExtractor> source, Func<IProperty, bool> filter)
		{
			return source.IdByPublicProperty(filter, e => e);
		}

		public static ConventionBase<IType, IIdExtractor> IdByPublicProperty(
			this ConventionBuilder<IType, IIdExtractor> source, Func<IProperty, bool> filter, Func<PropertyValueExtractor, PropertyValueExtractor> configurationDelegate)
		{
			return source.IdByProperty(filter.And(m => m.IsPublic), configurationDelegate);
		}

		public static ConventionBase<IType, IIdExtractor> IdByProperty(
			this ConventionBuilder<IType, IIdExtractor> source, Func<IProperty, bool> filter)
		{
			return source.IdByProperty(filter, e => e);
		}

		public static ConventionBase<IType, IIdExtractor> IdByProperty(
			this ConventionBuilder<IType, IIdExtractor> source, Func<IProperty, bool> filter, Func<PropertyValueExtractor, PropertyValueExtractor> configurationDelegate)
		{
			return source
				.By(t => configurationDelegate(BuildRoutine.IdExtractor().ByPropertyValue(t.Properties.First(filter))))
				.When(t => t.Properties.Any(filter));
		}

		#endregion

		#region IdByMethod

		public static ConventionBase<IType, IIdExtractor> IdByPublicMethod(
			this ConventionBuilder<IType, IIdExtractor> source, Func<IMethod, bool> filter)
		{
			return source.IdByPublicMethod(filter, e => e);
		}

		public static ConventionBase<IType, IIdExtractor> IdByPublicMethod(
			this ConventionBuilder<IType, IIdExtractor> source, Func<IMethod, bool> filter, Func<PropertyValueExtractor, PropertyValueExtractor> configurationDelegate)
		{
			return source.IdByMethod(filter.And(o => o.IsPublic), configurationDelegate);
		}

		public static ConventionBase<IType, IIdExtractor> IdByMethod(
			this ConventionBuilder<IType, IIdExtractor> source, Func<IMethod, bool> filter)
		{
			return source.IdByMethod(filter, e => e);
		}

		public static ConventionBase<IType, IIdExtractor> IdByMethod(
			this ConventionBuilder<IType, IIdExtractor> source, Func<IMethod, bool> filter, Func<PropertyValueExtractor, PropertyValueExtractor> configurationDelegate)
		{
			return source
				.By(t => configurationDelegate(BuildRoutine.IdExtractor()
					.ByPropertyValue(new MethodAsProperty(t.Methods.First(filter.And(o => o.HasNoParameters() && !o.ReturnsVoid()))))))
				.When(t => t.Methods.Any(filter.And(o => o.HasNoParameters() && !o.ReturnsVoid()))) as DelegateBasedConvention<IType, IIdExtractor>;
		}

		#endregion

		#region ValueByProperty

		public static ConventionBase<IType, IValueExtractor> ValueByPublicProperty(
			this ConventionBuilder<IType, IValueExtractor> source, Func<IProperty, bool> filter)
		{
			return source.ValueByPublicProperty(filter, e => e);
		}

		public static ConventionBase<IType, IValueExtractor> ValueByPublicProperty(
			this ConventionBuilder<IType, IValueExtractor> source, Func<IProperty, bool> filter, Func<PropertyValueExtractor, PropertyValueExtractor> configurationDelegate)
		{
			return source.ValueByProperty(filter.And(m => m.IsPublic), configurationDelegate);
		}

		public static ConventionBase<IType, IValueExtractor> ValueByProperty(
			this ConventionBuilder<IType, IValueExtractor> source, Func<IProperty, bool> filter)
		{
			return source.ValueByProperty(filter, e => e);
		}

		public static ConventionBase<IType, IValueExtractor> ValueByProperty(
			this ConventionBuilder<IType, IValueExtractor> source, Func<IProperty, bool> filter, Func<PropertyValueExtractor, PropertyValueExtractor> configurationDelegate)
		{
			return source
				.By(t => configurationDelegate(BuildRoutine.ValueExtractor().ByPropertyValue(t.Properties.First(filter))))
				.When(t => t.Properties.Any(filter));
		}

		#endregion

		#region ValueByMethod

		public static ConventionBase<IType, IValueExtractor> ValueByPublicMethod(
			this ConventionBuilder<IType, IValueExtractor> source, Func<IMethod, bool> filter)
		{
			return source.ValueByPublicMethod(filter.And(o => o.IsPublic), e => e);
		}

		public static ConventionBase<IType, IValueExtractor> ValueByPublicMethod(
			this ConventionBuilder<IType, IValueExtractor> source, Func<IMethod, bool> filter, Func<PropertyValueExtractor, PropertyValueExtractor> configurationDelegate)
		{
			return source.ValueByMethod(filter.And(o => o.IsPublic), configurationDelegate);
		}

		public static ConventionBase<IType, IValueExtractor> ValueByMethod(
			this ConventionBuilder<IType, IValueExtractor> source, Func<IMethod, bool> filter)
		{
			return source.ValueByMethod(filter, e => e);
		}

		public static ConventionBase<IType, IValueExtractor> ValueByMethod(
			this ConventionBuilder<IType, IValueExtractor> source, Func<IMethod, bool> filter, Func<PropertyValueExtractor, PropertyValueExtractor> configurationDelegate)
		{
			return source
				.By(t => configurationDelegate(BuildRoutine.ValueExtractor()
					.ByPropertyValue(new MethodAsProperty(t.Methods.First(filter.And(o => o.HasNoParameters() && !o.ReturnsVoid()))))))
				.When(t => t.Methods.Any(filter.And(o => o.HasNoParameters() && !o.ReturnsVoid())));
		}

		#endregion

		#endregion

		#region PatternBuilder

		public static ConventionBasedCodingStyle Use(this ConventionBasedCodingStyle source, Func<PatternBuilder<ConventionBasedCodingStyle>, ConventionBasedCodingStyle> pattern)
		{
			return source.Merge(pattern(BuildRoutine.CodingStylePattern()));
		}

		public static ConventionBasedInterceptionConfiguration Use(this ConventionBasedInterceptionConfiguration source, Func<PatternBuilder<ConventionBasedInterceptionConfiguration>, ConventionBasedInterceptionConfiguration> pattern)
		{
			return source.Merge(pattern(BuildRoutine.InterceptionPattern()));
		}

		public static ConventionBasedServiceConfiguration Use(this ConventionBasedServiceConfiguration source, Func<PatternBuilder<ConventionBasedServiceConfiguration>, ConventionBasedServiceConfiguration> pattern)
		{
			return source.Merge(pattern(BuildRoutine.ServicePattern()));
		}

		public static ConventionBasedApiConfiguration Use(this ConventionBasedApiConfiguration source, Func<PatternBuilder<ConventionBasedApiConfiguration>, ConventionBasedApiConfiguration> pattern)
		{
			return source.Merge(pattern(BuildRoutine.ApiGenerationPattern()));
		}

		public static ConventionBasedMvcConfiguration Use(this ConventionBasedMvcConfiguration source, Func<PatternBuilder<ConventionBasedMvcConfiguration>, ConventionBasedMvcConfiguration> pattern)
		{
			return source.Merge(pattern(BuildRoutine.MvcPattern()));
		}

		public static ConventionBasedServiceClientConfiguration Use(this ConventionBasedServiceClientConfiguration source, Func<PatternBuilder<ConventionBasedServiceClientConfiguration>, ConventionBasedServiceClientConfiguration> pattern)
		{
			return source.Merge(pattern(BuildRoutine.ServiceClientPattern()));
		}

		#endregion

		#region Virtual

		public static ConventionBasedCodingStyle AddTypes(this ConventionBasedCodingStyle source, params Func<VirtualTypeBuilder, VirtualType>[] typeBuilders)
		{
			return source.AddTypes(typeBuilders.Select(builder => builder(BuildRoutine.VirtualType())));
		}

		public static TConfiguration Add<TConfiguration>(this ListConfiguration<TConfiguration, VirtualType> source, params Func<VirtualTypeBuilder, VirtualType>[] typeBuilders)
		{
			return source.Add(
				typeBuilders.Select(builder => 
					builder(BuildRoutine.VirtualType())
				)
			);
		}

		public static TConfiguration Add<TConfiguration>(this ListConfiguration<TConfiguration, IMethod> source, Func<MethodBuilder, IEnumerable<IMethod>> builder)
			where TConfiguration : IType
		{
			return source.Add(t => builder(BuildRoutine.Method(t)));
		}

		public static TConfiguration Add<TConfiguration>(this ListConfiguration<TConfiguration, IMethod> source, Func<MethodBuilder, IMethod> builder)
			where TConfiguration : IType
		{
			return source.Add(t => builder(BuildRoutine.Method(t)));
		}

		public static TConfiguration Add<TConfiguration>(this ListConfiguration<TConfiguration, IParameter> source, Func<ParameterBuilder, IEnumerable<IParameter>> builder)
			where TConfiguration : IParametric
		{
			return source.Add(o => builder(BuildRoutine.Parameter(o)));
		}

		public static TConfiguration Add<TConfiguration>(this ListConfiguration<TConfiguration, IParameter> source, Func<ParameterBuilder, IParameter> builder)
			where TConfiguration : IParametric
		{
			return source.Add(o => builder(BuildRoutine.Parameter(o)));
		}

		public static ConventionBase<IType, List<IMethod>> Build(this ConventionBuilder<IType, List<IMethod>> source, Func<MethodBuilder, IEnumerable<IMethod>> builder)
		{
			return source.By(t => builder(BuildRoutine.Method(t)).ToList());
		}

		public static ConventionBase<IType, List<IMethod>> Build(this ConventionBuilder<IType, List<IMethod>> source, Func<MethodBuilder, IMethod> builder)
		{
			return source.By(t => new List<IMethod> { builder(BuildRoutine.Method(t)) });
		}

		#endregion

		#region Add System Types

		public static ConventionBasedCodingStyle AddCommonSystemTypes(this ConventionBasedCodingStyle source)
		{
			return source.AddTypes(
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
		}

		#endregion

		#region Header Processor

		public static TConfiguration Add<TConfiguration>(this ListConfiguration<TConfiguration, IHeaderProcessor> source, Func<HeaderProcessorBuilder, IHeaderProcessor> builder)
		{
			return source.Add(builder(BuildRoutine.HeaderProcessor()));
		}

		#endregion
	}
}
