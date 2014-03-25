using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Routine.Api;
using Routine.Api.Context;
using Routine.Core;
using Routine.Core.Builder;
using Routine.Core.Cache;
using Routine.Core.Configuration;
using Routine.Core.Context;
using Routine.Core.Extractor;
using Routine.Core.Interceptor;
using Routine.Core.Locator;
using Routine.Core.Member;
using Routine.Core.Operation;
using Routine.Core.Reflection;
using Routine.Core.Rest;
using Routine.Core.Selector;
using Routine.Core.Serializer;
using Routine.Core.Service;
using Routine.Core.Service.Impl;
using Routine.Mvc;
using Routine.Mvc.Builder;
using Routine.Mvc.Configuration;
using Routine.Mvc.Context;
using Routine.Soa;
using Routine.Soa.Builder;
using Routine.Soa.Configuration;
using Routine.Soa.Context;

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

		public static ReferenceValueExtractor<TFrom, List<TResultItem>> ReturnCastedList<TFrom, TResultItem>(
			this ReferenceValueExtractor<TFrom, List<TResultItem>> source)
		{
			return source.Return(o => ((ICollection)o).Cast<TResultItem>().ToList());
		}

		public static ReferenceValueExtractor<TFrom, string> ReturnAsString<TFrom>(
			this ReferenceValueExtractor<TFrom, string> source)
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

		public static TConcrete When<TConcrete, TFrom1, TFrom2, TResult>(
			this BaseOptionalExtractor<TConcrete, Tuple<TFrom1, TFrom2>, TResult> source,
			Func<TFrom1, TFrom2, bool> whenDelegate)
			where TConcrete : BaseOptionalExtractor<TConcrete, Tuple<TFrom1, TFrom2>, TResult>
		{
			return source.When(o => whenDelegate(o.Item1, o.Item2));
		}

		public static TConcrete When<TConcrete, TFrom1, TFrom2, TFrom3, TResult>(
			this BaseOptionalExtractor<TConcrete, Tuple<TFrom1, TFrom2, TFrom3>, TResult> source,
			Func<TFrom1, TFrom2, TFrom3, bool> whenDelegate)
			where TConcrete : BaseOptionalExtractor<TConcrete, Tuple<TFrom1, TFrom2, TFrom3>, TResult>
		{
			return source.When(o => whenDelegate(o.Item1, o.Item2, o.Item3));
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


		public static DelegateSelector<TypeInfo, IMember> ByProperties(this SelectorBuilder<TypeInfo, IMember> source, Func<PropertyInfo, bool> propertyFilter)
		{
			return source.By(t => t.GetAllProperties()
								   .Where(propertyFilter)
								   .Select(p => new PropertyMember(p) as IMember));
		}

		public static DelegateSelector<TypeInfo, IMember> ByPublicProperties(this SelectorBuilder<TypeInfo, IMember> source, Func<PropertyInfo, bool> propertyFilter)
		{
			return source.By(t => t.GetPublicProperties()
								   .Where(propertyFilter)
								   .Select(p => new PropertyMember(p) as IMember));
		}

		public static DelegateSelector<TypeInfo, IMember> ByMethods(this SelectorBuilder<TypeInfo, IMember> source, Func<MethodInfo, bool> memberFilter)
		{
			return source.By(t => t.GetAllMethods()
				.Where(memberFilter)
				.Select(m => new MethodMember(m) as IMember));
		}

		public static DelegateSelector<TypeInfo, IMember> ByPublicMethods(this SelectorBuilder<TypeInfo, IMember> source, Func<MethodInfo, bool> memberFilter)
		{
			return source.By(t => t.GetPublicMethods()
				.Where(memberFilter)
				.Select(m => new MethodMember(m) as IMember));
		}

		public static DelegateSelector<TypeInfo, IOperation> ByMethods(this SelectorBuilder<TypeInfo, IOperation> source, Func<MethodInfo, bool> memberFilter)
		{
			return source.By(t => t.GetAllMethods()
				.Where(memberFilter)
				.Select(m => new MethodOperation(m) as IOperation));
		}

		public static DelegateSelector<TypeInfo, IOperation> ByPublicMethods(this SelectorBuilder<TypeInfo, IOperation> source, Func<MethodInfo, bool> memberFilter)
		{
			return source.By(t => t.GetPublicMethods()
				.Where(memberFilter)
				.Select(m => new MethodOperation(m) as IOperation));
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
		public static MultipleInterceptor<TConfiguration, TContext> Add<TConfiguration, TContext>(
			this MultipleInterceptor<TConfiguration, TContext> source,
			Func<InterceptorBuilder<TContext>, IInterceptor<TContext>> interceptorDelegate)
			where TContext : InterceptionContext
		{
			return source.Add(interceptorDelegate(BuildRoutine.Interceptor<TContext>()));
		}

		public static TConfiguration Done<TConfiguration, TContext>(
			this MultipleInterceptor<TConfiguration, TContext> source,
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
		public static GenericSoaConfiguration Use(this GenericSoaConfiguration source, Func<PatternBuilder<GenericSoaConfiguration>, GenericSoaConfiguration> pattern)
		{
			return source.Merge(pattern(BuildRoutine.SoaPattern()));
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

		#region TFrom => object Extensions
		public static ReferenceValueExtractor<object, TData> ByPublicProperty<TData>(
			this ExtractorBuilder<object, TData> source, 
			Func<object, Func<PropertyInfo, bool>> propertyFilter)
		{
			return source.ByReference(o => BuildRoutine.Selector<TypeInfo, IMember>().ByPublicProperties(propertyFilter(o)));
		}

		public static ReferenceValueExtractor<object, TData> ByPublicProperty<TData>(
			this ExtractorBuilder<object, TData> source,
			Func<PropertyInfo, bool> propertyFilter)
		{
			return source.ByReference(BuildRoutine.Selector<TypeInfo, IMember>().ByPublicProperties(propertyFilter).UseCache());
		}

		public static ReferenceValueExtractor<object, TData> ByProperty<TData>(
			this ExtractorBuilder<object, TData> source,
			Func<object, Func<PropertyInfo, bool>> propertyFilter)
		{
			return source.ByReference(o => BuildRoutine.Selector<TypeInfo, IMember>().ByProperties(propertyFilter(o)));
		}

		public static ReferenceValueExtractor<object, TData> ByProperty<TData>(
			this ExtractorBuilder<object, TData> source,
			Func<PropertyInfo, bool> propertyFilter)
		{
			return source.ByReference(BuildRoutine.Selector<TypeInfo, IMember>().ByProperties(propertyFilter).UseCache());
		}

		public static ReferenceValueExtractor<object, TData> ByPublicMethod<TData>(
			this ExtractorBuilder<object, TData> source,
			Func<object, Func<MethodInfo, bool>> methodFilter)
		{
			return source.ByReference(o => BuildRoutine.Selector<TypeInfo, IMember>().ByPublicMethods(methodFilter(o)));
		}

		public static ReferenceValueExtractor<object, TData> ByPublicMethod<TData>(
			this ExtractorBuilder<object, TData> source,
			Func<MethodInfo, bool> methodFilter)
		{
			return source.ByReference(BuildRoutine.Selector<TypeInfo, IMember>().ByPublicMethods(methodFilter).UseCache());
		}

		public static ReferenceValueExtractor<object, TData> ByMethod<TData>(
			this ExtractorBuilder<object, TData> source,
			Func<object, Func<MethodInfo, bool>> methodFilter)
		{
			return source.ByReference(o => BuildRoutine.Selector<TypeInfo, IMember>().ByMethods(methodFilter(o)));
		}

		public static ReferenceValueExtractor<object, TData> ByMethod<TData>(
			this ExtractorBuilder<object, TData> source,
			Func<MethodInfo, bool> methodFilter)
		{
			return source.ByReference(BuildRoutine.Selector<TypeInfo, IMember>().ByMethods(methodFilter).UseCache());
		}
		#endregion

		#region TFrom => Tuple<object, TFrom> Extensions		
		public static ReferenceValueExtractor<Tuple<object, TFrom>, TData> ByPublicProperty<TFrom, TData>(
			this ExtractorBuilder<Tuple<object, TFrom>, TData> source, 
			Func<object, TFrom, Func<PropertyInfo, bool>> propertyFilter)
		{
			return source
					.ByReference(o => BuildRoutine.Selector<TypeInfo, IMember>().ByPublicProperties(propertyFilter(o.Item1, o.Item2)))
					.Using(o => o.Item1);
		}

		public static ReferenceValueExtractor<Tuple<object, TFrom>, TData> ByPublicProperty<TFrom, TData>(
			this ExtractorBuilder<Tuple<object, TFrom>, TData> source,
			Func<PropertyInfo, bool> propertyFilter)
		{
			return source
					.ByReference(BuildRoutine.Selector<TypeInfo, IMember>().ByPublicProperties(propertyFilter).UseCache())
					.Using(o => o.Item1);
		}

		public static ReferenceValueExtractor<Tuple<object, TFrom>, TData> ByProperty<TFrom, TData>(
			this ExtractorBuilder<Tuple<object, TFrom>, TData> source,
			Func<object, TFrom, Func<PropertyInfo, bool>> propertyFilter)
		{
			return source
					.ByReference(o => BuildRoutine.Selector<TypeInfo, IMember>().ByProperties(propertyFilter(o.Item1, o.Item2)))
					.Using(o => o.Item1);
		}

		public static ReferenceValueExtractor<Tuple<object, TFrom>, TData> ByProperty<TFrom, TData>(
			this ExtractorBuilder<Tuple<object, TFrom>, TData> source,
			Func<PropertyInfo, bool> propertyFilter)
		{
			return source
					.ByReference(BuildRoutine.Selector<TypeInfo, IMember>().ByProperties(propertyFilter).UseCache())
					.Using(o => o.Item1);
		}

		public static ReferenceValueExtractor<Tuple<object, TFrom>, TData> ByPublicMethod<TFrom, TData>(
			this ExtractorBuilder<Tuple<object, TFrom>, TData> source,
			Func<object, TFrom, Func<MethodInfo, bool>> methodFilter)
		{
			return source
					.ByReference(o => BuildRoutine.Selector<TypeInfo, IMember>().ByPublicMethods(methodFilter(o.Item1, o.Item2)))
					.Using(o => o.Item1);
		}

		public static ReferenceValueExtractor<Tuple<object, TFrom>, TData> ByPublicMethod<TFrom, TData>(
			this ExtractorBuilder<Tuple<object, TFrom>, TData> source,
			Func<MethodInfo, bool> methodFilter)
		{
			return source
					.ByReference(BuildRoutine.Selector<TypeInfo, IMember>().ByPublicMethods(methodFilter).UseCache())
					.Using(o => o.Item1);
		}

		public static ReferenceValueExtractor<Tuple<object, TFrom>, TData> ByMethod<TFrom, TData>(
			this ExtractorBuilder<Tuple<object, TFrom>, TData> source,
			Func<object, TFrom, Func<MethodInfo, bool>> methodFilter)
		{
			return source
					.ByReference(o => BuildRoutine.Selector<TypeInfo, IMember>().ByMethods(methodFilter(o.Item1, o.Item2)))
					.Using(o => o.Item1);
		}

		public static ReferenceValueExtractor<Tuple<object, TFrom>, TData> ByMethod<TFrom, TData>(
			this ExtractorBuilder<Tuple<object, TFrom>, TData> source,
			Func<MethodInfo, bool> methodFilter)
		{
			return source
					.ByReference(BuildRoutine.Selector<TypeInfo, IMember>().ByMethods(methodFilter).UseCache())
					.Using(o => o.Item1);
		}
		#endregion

		#region TFrom => Tuple<object, TFrom1, TFrom2> Extensions		
		public static ReferenceValueExtractor<Tuple<object, TFrom1, TFrom2>, TData> ByPublicProperty<TFrom1, TFrom2, TData>(
			this ExtractorBuilder<Tuple<object, TFrom1, TFrom2>, TData> source, 
			Func<object, TFrom1, TFrom2, Func<PropertyInfo, bool>> propertyFilter)
		{
			return source
					.ByReference(o => BuildRoutine.Selector<TypeInfo, IMember>().ByPublicProperties(propertyFilter(o.Item1, o.Item2, o.Item3)))
					.Using(o => o.Item1);
		}

		public static ReferenceValueExtractor<Tuple<object, TFrom1, TFrom2>, TData> ByPublicProperty<TFrom1, TFrom2, TData>(
			this ExtractorBuilder<Tuple<object, TFrom1, TFrom2>, TData> source,
			Func<PropertyInfo, bool> propertyFilter)
		{
			return source
					.ByReference(BuildRoutine.Selector<TypeInfo, IMember>().ByPublicProperties(propertyFilter).UseCache())
					.Using(o => o.Item1);
		}

		public static ReferenceValueExtractor<Tuple<object, TFrom1, TFrom2>, TData> ByProperty<TFrom1, TFrom2, TData>(
			this ExtractorBuilder<Tuple<object, TFrom1, TFrom2>, TData> source,
			Func<object, TFrom1, TFrom2, Func<PropertyInfo, bool>> propertyFilter)
		{
			return source
					.ByReference(o => BuildRoutine.Selector<TypeInfo, IMember>().ByProperties(propertyFilter(o.Item1, o.Item2, o.Item3)))
					.Using(o => o.Item1);
		}

		public static ReferenceValueExtractor<Tuple<object, TFrom1, TFrom2>, TData> ByProperty<TFrom1, TFrom2, TData>(
			this ExtractorBuilder<Tuple<object, TFrom1, TFrom2>, TData> source,
			Func<PropertyInfo, bool> propertyFilter)
		{
			return source
					.ByReference(BuildRoutine.Selector<TypeInfo, IMember>().ByProperties(propertyFilter).UseCache())
					.Using(o => o.Item1);
		}

		public static ReferenceValueExtractor<Tuple<object, TFrom1, TFrom2>, TData> ByPublicMethod<TFrom1, TFrom2, TData>(
			this ExtractorBuilder<Tuple<object, TFrom1, TFrom2>, TData> source,
			Func<object, TFrom1, TFrom2, Func<MethodInfo, bool>> methodFilter)
		{
			return source
					.ByReference(o => BuildRoutine.Selector<TypeInfo, IMember>().ByPublicMethods(methodFilter(o.Item1, o.Item2, o.Item3)))
					.Using(o => o.Item1);
		}

		public static ReferenceValueExtractor<Tuple<object, TFrom1, TFrom2>, TData> ByPublicMethod<TFrom1, TFrom2, TData>(
			this ExtractorBuilder<Tuple<object, TFrom1, TFrom2>, TData> source,
			Func<MethodInfo, bool> methodFilter)
		{
			return source
					.ByReference(BuildRoutine.Selector<TypeInfo, IMember>().ByPublicMethods(methodFilter).UseCache())
					.Using(o => o.Item1);
		}

		public static ReferenceValueExtractor<Tuple<object, TFrom1, TFrom2>, TData> ByMethod<TFrom1, TFrom2, TData>(
			this ExtractorBuilder<Tuple<object, TFrom1, TFrom2>, TData> source,
			Func<object, TFrom1, TFrom2, Func<MethodInfo, bool>> methodFilter)
		{
			return source
					.ByReference(o => BuildRoutine.Selector<TypeInfo, IMember>().ByMethods(methodFilter(o.Item1, o.Item2, o.Item3)))
					.Using(o => o.Item1);
		}

		public static ReferenceValueExtractor<Tuple<object, TFrom1, TFrom2>, TData> ByMethod<TFrom1, TFrom2, TData>(
			this ExtractorBuilder<Tuple<object, TFrom1, TFrom2>, TData> source,
			Func<MethodInfo, bool> methodFilter)
		{
			return source
					.ByReference(BuildRoutine.Selector<TypeInfo, IMember>().ByMethods(methodFilter).UseCache())
					.Using(o => o.Item1);
		}
		#endregion
	}

}
