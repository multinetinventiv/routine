using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Routine.Api.Builder;
using Routine.Api.Configuration;
using Routine.Core;
using Routine.Core.Builder;
using Routine.Core.Configuration;
using Routine.Core.Extractor;
using Routine.Core.Interceptor;
using Routine.Core.Locator;
using Routine.Core.DomainApi;
using Routine.Core.Reflection;
using Routine.Core.Selector;
using Routine.Core.Serializer;
using Routine.Mvc.Builder;
using Routine.Mvc.Configuration;
using Routine.Soa.Builder;
using Routine.Soa.Configuration;

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

		internal static PatternBuilder<GenericCodingStyle> CodingStylePattern()
		{
			return new PatternBuilder<GenericCodingStyle>();
		}

		public static InterceptionConfigurationBuilder InterceptionConfig()
		{
			return new InterceptionConfigurationBuilder();
		}

		internal static PatternBuilder<GenericInterceptionConfiguration> InterceptionPattern()
		{
			return new PatternBuilder<GenericInterceptionConfiguration>();
		}

		public static ApiGenerationConfigurationBuilder ApiGenerationConfig()
		{
			return new ApiGenerationConfigurationBuilder();
		}

		internal static PatternBuilder<GenericApiGenerationConfiguration> ApiGenerationPattern()
		{
			return new PatternBuilder<GenericApiGenerationConfiguration>();
		}

		public static MvcConfigurationBuilder MvcConfig()
		{
			return new MvcConfigurationBuilder();
		}

		internal static PatternBuilder<GenericMvcConfiguration> MvcPattern()
		{
			return new PatternBuilder<GenericMvcConfiguration>();
		}

		public static SoaConfigurationBuilder SoaConfig()
		{
			return new SoaConfigurationBuilder();
		}

		internal static PatternBuilder<GenericSoaConfiguration> SoaPattern()
		{
			return new PatternBuilder<GenericSoaConfiguration>();
		}

		public static SoaClientConfigurationBuilder SoaClientConfig()
		{
			return new SoaClientConfigurationBuilder();
		}

		internal static PatternBuilder<GenericSoaClientConfiguration> SoaClientPattern()
		{
			return new PatternBuilder<GenericSoaClientConfiguration>();
		}

		public static LocatorBuilder Locator()
		{
			return new LocatorBuilder();
		}

		public static SelectorBuilder<TFrom, TItem> Selector<TFrom, TItem>()
		{
			return new SelectorBuilder<TFrom, TItem>();
		}

		public static ExtractorBuilder<TFrom, TData> Extractor<TFrom, TData>()
		{
			return new ExtractorBuilder<TFrom, TData>();
		}

		public static SerializerBuilder<T> Serializer<T>()
		{
			return new SerializerBuilder<T>();
		}

		public static InterceptorBuilder<TContext> Interceptor<TContext>()
			where TContext : InterceptionContext
		{
			return new InterceptorBuilder<TContext>();
		}
	}
	
	public static class BuildRoutineExtensions
	{
		#region Extractor

		public static MultipleExtractor<TConfigurator, TFrom, TResult> Add<TConfigurator, TFrom, TResult>(
			this MultipleExtractor<TConfigurator, TFrom, TResult> source, 
			Func<ExtractorBuilder<TFrom, TResult>, IOptionalExtractor<TFrom, TResult>> extractorDelegate)
		{
			return source.Add(extractorDelegate(BuildRoutine.Extractor<TFrom, TResult>()));
		}

		public static TConfigurator Done<TConfigurator, TFrom, TResult>(
			this MultipleExtractor<TConfigurator, TFrom, TResult> source, 
			Func<ExtractorBuilder<TFrom, TResult>, IOptionalExtractor<TFrom, TResult>> extractorDelegate)
		{
			return source.Done(extractorDelegate(BuildRoutine.Extractor<TFrom, TResult>()));
		}

		public static MemberValueExtractor<TFrom, List<TResultItem>> ReturnCastedList<TFrom, TResultItem>(
			this MemberValueExtractor<TFrom, List<TResultItem>> source)
		{
			return source.Return(o => ((ICollection)o).Cast<TResultItem>().ToList());
		}

		public static MemberValueExtractor<TFrom, string> ReturnAsString<TFrom>(
			this MemberValueExtractor<TFrom, string> source)
		{
			return source.Return(o => o.ToString());
		}

		public static TConcrete WhenType<TConcrete, TResult>(
			this BaseOptionalExtractor<TConcrete, object, TResult> source,
			Func<TypeInfo, bool> whenDelegate)
			where TConcrete : BaseOptionalExtractor<TConcrete, object, TResult>
		{
			return source.When(o => o != null && whenDelegate(o.GetTypeInfo())); 
		}

		#endregion

		#region Locator

		public static MultipleLocator<TConfigurator> Add<TConfigurator>(
			this MultipleLocator<TConfigurator> source, 
			Func<LocatorBuilder, IOptionalLocator> locatorDelegate)
		{
			return source.Add(locatorDelegate(BuildRoutine.Locator()));
		}

		public static TConfigurator Done<TConfigurator>(
			this MultipleLocator<TConfigurator> source, 
			Func<LocatorBuilder, IOptionalLocator> locatorDelegate)
		{
			return source.Done(locatorDelegate(BuildRoutine.Locator()));
		}

		#endregion

		#region Selector

		public static MultipleSelector<TConfigurator, TFrom, TItem> Add<TConfigurator, TFrom, TItem>(
			this MultipleSelector<TConfigurator, TFrom, TItem> source, 
			Func<SelectorBuilder<TFrom, TItem>, IOptionalSelector<TFrom, TItem>> selectorDelegate)
		{
			return source.Add(selectorDelegate(BuildRoutine.Selector<TFrom, TItem>()));
		}

		public static TConfigurator Done<TConfigurator, TFrom, TItem>(
			this MultipleSelector<TConfigurator, TFrom, TItem> source, 
			Func<SelectorBuilder<TFrom, TItem>, IOptionalSelector<TFrom, TItem>> selectorDelegate)
		{
			return source.Done(selectorDelegate(BuildRoutine.Selector<TFrom, TItem>()));
		}

		public static DelegateSelector<TypeInfo, IInitializer> ByConstructors(this SelectorBuilder<TypeInfo, IInitializer> source) { return source.ByConstructors(c => true); }
		public static DelegateSelector<TypeInfo, IInitializer> ByConstructors(this SelectorBuilder<TypeInfo, IInitializer> source, Func<ConstructorInfo, bool> constructorFilter)
		{
			return source.By(t => t.GetAllConstructors()
								   .Where(constructorFilter)
								   .Select(c => c.ToInitializer()));
		}

		public static DelegateSelector<TypeInfo, IInitializer> ByPublicConstructors(this SelectorBuilder<TypeInfo, IInitializer> source) { return source.ByPublicConstructors(c => true); }
		public static DelegateSelector<TypeInfo, IInitializer> ByPublicConstructors(this SelectorBuilder<TypeInfo, IInitializer> source, Func<ConstructorInfo, bool> constructorFilter)
		{
			return source.By(t => t.GetPublicConstructors()
								   .Where(constructorFilter)
								   .Select(c => c.ToInitializer()));
		}

		public static DelegateSelector<TypeInfo, IMember> ByProperties(this SelectorBuilder<TypeInfo, IMember> source, Func<PropertyInfo, bool> propertyFilter)
		{
			return source.By(t => t.GetAllProperties()
								   .Where(propertyFilter)
								   .Select(p => p.ToMember()));
		}

		public static DelegateSelector<TypeInfo, IMember> ByPublicProperties(this SelectorBuilder<TypeInfo, IMember> source, Func<PropertyInfo, bool> propertyFilter)
		{
			return source.By(t => t.GetPublicProperties()
								   .Where(propertyFilter)
								   .Select(p => p.ToMember()));
		}

		public static DelegateSelector<TypeInfo, IMember> ByMethods(this SelectorBuilder<TypeInfo, IMember> source, Func<MethodInfo, bool> memberFilter)
		{
			return source.By(t => t.GetAllMethods()
				.Where(memberFilter)
				.Select(m => m.ToMember()));
		}

		public static DelegateSelector<TypeInfo, IMember> ByPublicMethods(this SelectorBuilder<TypeInfo, IMember> source, Func<MethodInfo, bool> memberFilter)
		{
			return source.By(t => t.GetPublicMethods()
				.Where(memberFilter)
				.Select(m => m.ToMember()));
		}

		public static DelegateSelector<TypeInfo, IOperation> ByMethods(this SelectorBuilder<TypeInfo, IOperation> source, Func<MethodInfo, bool> memberFilter)
		{
			return source.By(t => t.GetAllMethods()
				.Where(memberFilter)
				.Select(m => m.ToOperation()));
		}

		public static DelegateSelector<TypeInfo, IOperation> ByPublicMethods(this SelectorBuilder<TypeInfo, IOperation> source, Func<MethodInfo, bool> memberFilter)
		{
			return source.By(t => t.GetPublicMethods()
				.Where(memberFilter)
				.Select(m => m.ToOperation()));
		}

		public static DelegateSelector<TypeInfo, IOperation> ByProperties(this SelectorBuilder<TypeInfo, IOperation> source, Func<PropertyInfo, bool> memberFilter) 
		{
			return source.ByProperties(Constants.PROPERTY_OPERATION_DEFAULT_PREFIX, memberFilter);
		}

		public static DelegateSelector<TypeInfo, IOperation> ByProperties(this SelectorBuilder<TypeInfo, IOperation> source, string operationNamePrefix, Func<PropertyInfo, bool> memberFilter)
		{
			return source.By(t => t.GetAllProperties()
				.Where(memberFilter)
				.Select(p => p.ToOperation(operationNamePrefix)));
		}

		public static DelegateSelector<TypeInfo, IOperation> ByPublicProperties(this SelectorBuilder<TypeInfo, IOperation> source, Func<PropertyInfo, bool> memberFilter)
		{
			return source.ByPublicProperties(Constants.PROPERTY_OPERATION_DEFAULT_PREFIX, memberFilter);
		}

		public static DelegateSelector<TypeInfo, IOperation> ByPublicProperties(this SelectorBuilder<TypeInfo, IOperation> source, string operationNamePrefix, Func<PropertyInfo, bool> memberFilter)
		{
			return source.By(t => t.GetPublicProperties()
				.Where(memberFilter)
				.Select(m => m.ToOperation(operationNamePrefix)));
		}

		#endregion

		#region Serializer

		public static MultipleSerializer<TConfigurator, TSerializable> Add<TConfigurator, TSerializable>(
			this MultipleSerializer<TConfigurator, TSerializable> source, 
			Func<SerializerBuilder<TSerializable>, IOptionalSerializer<TSerializable>> serializerDelegate)
		{
			return source.Add(serializerDelegate(BuildRoutine.Serializer<TSerializable>()));
		}

		public static TConfigurator Done<TConfigurator, TSerializable>(
			this MultipleSerializer<TConfigurator, TSerializable> source, 
			Func<SerializerBuilder<TSerializable>, IOptionalSerializer<TSerializable>> serializerDelegate)
		{
			return source.Done(serializerDelegate(BuildRoutine.Serializer<TSerializable>()));
		}

		#endregion

		#region Interceptor

		public static ChainInterceptor<TConfiguration, TContext> Add<TConfiguration, TContext>(
			this ChainInterceptor<TConfiguration, TContext> source,
			Func<InterceptorBuilder<TContext>, IInterceptor<TContext>> interceptorDelegate)
			where TContext : InterceptionContext
		{
			return source.Add(interceptorDelegate(BuildRoutine.Interceptor<TContext>()));
		}

		public static TConfiguration Done<TConfiguration, TContext>(
			this ChainInterceptor<TConfiguration, TContext> source,
			Func<InterceptorBuilder<TContext>, IInterceptor<TContext>> interceptorDelegate)
			where TContext : InterceptionContext
		{
			return source.Done(interceptorDelegate(BuildRoutine.Interceptor<TContext>()));
		}

		#endregion

		#region PatternBuilder

		public static GenericCodingStyle Use(this GenericCodingStyle source, Func<PatternBuilder<GenericCodingStyle>, GenericCodingStyle> pattern)
		{
			return source.Merge(pattern(BuildRoutine.CodingStylePattern()));
		}

		public static GenericInterceptionConfiguration Use(this GenericInterceptionConfiguration source, Func<PatternBuilder<GenericInterceptionConfiguration>, GenericInterceptionConfiguration> pattern)
		{
			return source.Merge(pattern(BuildRoutine.InterceptionPattern()));
		}

		public static GenericSoaConfiguration Use(this GenericSoaConfiguration source, Func<PatternBuilder<GenericSoaConfiguration>, GenericSoaConfiguration> pattern)
		{
			return source.Merge(pattern(BuildRoutine.SoaPattern()));
		}

		public static GenericApiGenerationConfiguration Use(this GenericApiGenerationConfiguration source, Func<PatternBuilder<GenericApiGenerationConfiguration>, GenericApiGenerationConfiguration> pattern)
		{
			return source.Merge(pattern(BuildRoutine.ApiGenerationPattern()));
		}

		public static GenericMvcConfiguration Use(this GenericMvcConfiguration source, Func<PatternBuilder<GenericMvcConfiguration>, GenericMvcConfiguration> pattern)
		{
			return source.Merge(pattern(BuildRoutine.MvcPattern()));
		}

		public static GenericSoaClientConfiguration Use(this GenericSoaClientConfiguration source, Func<PatternBuilder<GenericSoaClientConfiguration>, GenericSoaClientConfiguration> pattern)
		{
			return source.Merge(pattern(BuildRoutine.SoaClientPattern()));
		}

		#endregion

		#region InterceptorBuilder

		public static AdapterInterceptor<TContext, TAdaptedContext> Adapt<TContext, TAdaptedContext>(
			this InterceptorBuilder<TContext> source, IInterceptor<TAdaptedContext> childInterceptor)
			where TContext : TAdaptedContext
			where TAdaptedContext : InterceptionContext
		{
			return new AdapterInterceptor<TContext, TAdaptedContext>(childInterceptor);
		}

		#endregion
	}

	public static class ExtractorBuilderExtensions
	{
		#region TFrom => list Extensions

		public static StaticExtractor<TFrom, List<TResultItem>> Always<TFrom, TResultItem>(
			this ExtractorBuilder<TFrom, List<TResultItem>> source,
			params TResultItem[] items)
		{
			return source.Always(items.ToList());
		}

		#endregion

		#region MemberValueExtractor Extensions

		public static MemberValueExtractor<TFrom, TData> ByPublicProperty<TFrom, TData>(
			this ExtractorBuilder<TFrom, TData> source,
			Func<PropertyInfo, bool> propertyFilter)
		{
			return source.ByMember(o => o.GetTypeInfo().GetPublicProperties().FirstOrDefault(Wrap(propertyFilter)).ToMember());
		}

		public static MemberValueExtractor<TFrom, TData> ByProperty<TFrom, TData>(
			this ExtractorBuilder<TFrom, TData> source,
			Func<PropertyInfo, bool> propertyFilter)
		{
			return source.ByMember(o => o.GetTypeInfo().GetAllProperties().FirstOrDefault(Wrap(propertyFilter)).ToMember());
		}

		public static MemberValueExtractor<TFrom, TData> ByPublicMethod<TFrom, TData>(
			this ExtractorBuilder<TFrom, TData> source,
			Func<MethodInfo, bool> methodFilter)
		{
			return source.ByMember(o => o.GetTypeInfo().GetPublicMethods().FirstOrDefault(Wrap(methodFilter)).ToMember());
		}

		public static MemberValueExtractor<TFrom, TData> ByMethod<TFrom, TData>(
			this ExtractorBuilder<TFrom, TData> source,
			Func<MethodInfo, bool> methodFilter)
		{
			return source.ByMember(o => o.GetTypeInfo().GetAllMethods().FirstOrDefault(Wrap(methodFilter)).ToMember());
		}

		private static Func<MethodInfo, bool> Wrap(Func<MethodInfo, bool> original) { return m => m.HasNoParameters() && !m.ReturnsVoid() && original(m); }
		private static Func<PropertyInfo, bool> Wrap(Func<PropertyInfo, bool> original) { return p => !p.IsIndexer && original(p); }

		#endregion
	}
}
