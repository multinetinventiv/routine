using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Api.Configuration;
using Routine.Core.Configuration;
using Routine.Core.Configuration.Convention;
using Routine.Engine;
using Routine.Engine.Configuration;
using Routine.Engine.Configuration.Conventional;
using Routine.Engine.Extractor;
using Routine.Engine.Virtual;
using Routine.Interception;
using Routine.Interception.Configuration;
using Routine.Soa.Configuration;
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

		internal static PatternBuilder<ConventionalCodingStyle> CodingStylePattern()
		{
			return new PatternBuilder<ConventionalCodingStyle>();
		}

		public static InterceptionConfigurationBuilder InterceptionConfig()
		{
			return new InterceptionConfigurationBuilder();
		}

		internal static PatternBuilder<ConventionalInterceptionConfiguration> InterceptionPattern()
		{
			return new PatternBuilder<ConventionalInterceptionConfiguration>();
		}

		public static ApiConfigurationBuilder ApiConfig()
		{
			return new ApiConfigurationBuilder();
		}

		internal static PatternBuilder<ConventionalApiConfiguration> ApiGenerationPattern()
		{
			return new PatternBuilder<ConventionalApiConfiguration>();
		}

		public static MvcConfigurationBuilder MvcConfig()
		{
			return new MvcConfigurationBuilder();
		}

		internal static PatternBuilder<ConventionalMvcConfiguration> MvcPattern()
		{
			return new PatternBuilder<ConventionalMvcConfiguration>();
		}

		public static SoaConfigurationBuilder SoaConfig()
		{
			return new SoaConfigurationBuilder();
		}

		internal static PatternBuilder<ConventionalSoaConfiguration> SoaPattern()
		{
			return new PatternBuilder<ConventionalSoaConfiguration>();
		}

		public static SoaClientConfigurationBuilder SoaClientConfig()
		{
			return new SoaClientConfigurationBuilder();
		}

		internal static PatternBuilder<ConventionalSoaClientConfiguration> SoaClientPattern()
		{
			return new PatternBuilder<ConventionalSoaClientConfiguration>();
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

		public static InterceptorBuilder<TContext> Interceptor<TContext>()
			where TContext : InterceptionContext
		{
			return new InterceptorBuilder<TContext>();
		}

		public static VirtualTypeBuilder VirtualType()
		{
			return new VirtualTypeBuilder();
		}

		public static OperationBuilder Operation(IType parentType)
		{
			return new OperationBuilder(parentType);
		}

		public static ParameterBuilder Parameter(IParametric owner)
		{
			return new ParameterBuilder(owner);
		}
	}

	public static class BuildRoutineExtensions
	{
		#region Convention

		public static TConfiguration Set<TConfiguration, TFrom, TResult>(
			this ConventionalConfiguration<TConfiguration, TFrom, TResult> source,
			Func<ConventionBuilder<TFrom, TResult>, IConvention<TFrom, TResult>> conventionDelegate) where TConfiguration : ILayered
		{
			return source.Set(conventionDelegate(BuildRoutine.Convention<TFrom, TResult>()));
		}

		public static TConfiguration Add<TConfiguration, TFrom, TItem>(
			this ConventionalListConfiguration<TConfiguration, TFrom, TItem> source,
			Func<ConventionBuilder<TFrom, List<TItem>>, IConvention<TFrom, List<TItem>>> conventionDelegate)
		{
			return source.Add(conventionDelegate(BuildRoutine.Convention<TFrom, List<TItem>>()));
		}

		public static ConventionBase<IType, List<IInitializer>> Initializers(this ConventionBuilder<IType, List<IInitializer>> source) { return source.Initializers(i => true); }
		public static ConventionBase<IType, List<IInitializer>> Initializers(this ConventionBuilder<IType, List<IInitializer>> source, Func<IInitializer, bool> initializerFilter)
		{
			return source.By(t => t.Initializers.Where(initializerFilter).ToList());
		}

		public static ConventionBase<IType, List<IInitializer>> PublicInitializers(this ConventionBuilder<IType, List<IInitializer>> source) { return source.PublicInitializers(i => true); }
		public static ConventionBase<IType, List<IInitializer>> PublicInitializers(this ConventionBuilder<IType, List<IInitializer>> source, Func<IInitializer, bool> initializerFilter)
		{
			return source.Initializers(i => i.IsPublic && initializerFilter(i));
		}

		public static ConventionBase<IType, List<IMember>> Members(this ConventionBuilder<IType, List<IMember>> source) { return source.Members(m => true); }
		public static ConventionBase<IType, List<IMember>> Members(this ConventionBuilder<IType, List<IMember>> source, Func<IMember, bool> memberFilter)
		{
			return source.By(t => t.Members.Where(memberFilter).ToList());
		}

		public static ConventionBase<IType, List<IMember>> PublicMembers(this ConventionBuilder<IType, List<IMember>> source) { return source.PublicMembers(m => true); }
		public static ConventionBase<IType, List<IMember>> PublicMembers(this ConventionBuilder<IType, List<IMember>> source, Func<IMember, bool> memberFilter)
		{
			return source.Members(m => m.IsPublic && memberFilter(m));
		}

		public static ConventionBase<IType, List<IMember>> Operations(this ConventionBuilder<IType, List<IMember>> source, Func<IOperation, bool> operationFilter) { return source.Operations(string.Empty, operationFilter); }
		public static ConventionBase<IType, List<IMember>> Operations(this ConventionBuilder<IType, List<IMember>> source, string ignorePrefix, Func<IOperation, bool> operationFilter)
		{
			return source.By(t => t
				.Operations
				.Where(operationFilter)
				.Select(m => new OperationMember(m, ignorePrefix) as IMember)
				.ToList()
			);
		}

		public static ConventionBase<IType, List<IMember>> PublicOperations(this ConventionBuilder<IType, List<IMember>> source, Func<IOperation, bool> operationFilter) { return source.PublicOperations(string.Empty, operationFilter); }
		public static ConventionBase<IType, List<IMember>> PublicOperations(this ConventionBuilder<IType, List<IMember>> source, string ignorePrefix, Func<IOperation, bool> operationFilter)
		{
			return source.Operations(o => o.IsPublic && operationFilter(o));
		}

		public static ConventionBase<IType, List<IOperation>> Operations(this ConventionBuilder<IType, List<IOperation>> source) { return source.Operations(o => true); }
		public static ConventionBase<IType, List<IOperation>> Operations(this ConventionBuilder<IType, List<IOperation>> source, Func<IOperation, bool> operationFilter)
		{
			return source.By(t => t.Operations.Where(operationFilter).ToList());
		}

		public static ConventionBase<IType, List<IOperation>> PublicOperations(this ConventionBuilder<IType, List<IOperation>> source) { return source.PublicOperations(o => true); }
		public static ConventionBase<IType, List<IOperation>> PublicOperations(this ConventionBuilder<IType, List<IOperation>> source, Func<IOperation, bool> operationFilter)
		{
			return source.Operations(o => o.IsPublic && operationFilter(o));
		}

		public static ConventionBase<IType, List<IOperation>> Members(this ConventionBuilder<IType, List<IOperation>> source, Func<IMember, bool> memberFilter) { return source.Members(Constants.PROPERTY_OPERATION_DEFAULT_PREFIX, memberFilter); }
		public static ConventionBase<IType, List<IOperation>> Members(this ConventionBuilder<IType, List<IOperation>> source, string operationNamePrefix, Func<IMember, bool> memberFilter)
		{
			return source.By(t => t
				.Members
				.Where(memberFilter)
				.Select(m => new MemberOperation(m, operationNamePrefix) as IOperation)
				.ToList()
			);
		}

		public static ConventionBase<IType, List<IOperation>> PublicMembers(this ConventionBuilder<IType, List<IOperation>> source, Func<IMember, bool> memberFilter) { return source.PublicMembers(Constants.PROPERTY_OPERATION_DEFAULT_PREFIX, memberFilter); }
		public static ConventionBase<IType, List<IOperation>> PublicMembers(this ConventionBuilder<IType, List<IOperation>> source, string operationNamePrefix, Func<IMember, bool> memberFilter)
		{
			return source.Members(operationNamePrefix, m => m.IsPublic && memberFilter(m));
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

		public static ConventionBase<TFrom, List<IInterceptor<TContext>>> Interceptor<TFrom, TContext>(
			this ConventionBuilder<TFrom, List<IInterceptor<TContext>>> source,
			Func<InterceptorBuilder<TContext>, IInterceptor<TContext>> interceptorDelegate)
			where TContext : InterceptionContext
		{
			return source.Constant(interceptorDelegate(BuildRoutine.Interceptor<TContext>()));
		}

		#region IdByMember

		public static ConventionBase<IType, IIdExtractor> IdByPublicMember(
			this ConventionBuilder<IType, IIdExtractor> source, Func<IMember, bool> memberFilter)
		{
			return source.IdByPublicMember(memberFilter, e => e);
		}

		public static ConventionBase<IType, IIdExtractor> IdByPublicMember(
			this ConventionBuilder<IType, IIdExtractor> source, Func<IMember, bool> memberFilter, Func<MemberValueExtractor, MemberValueExtractor> configurationDelegate)
		{
			return source.IdByMember(memberFilter.And(m => m.IsPublic), configurationDelegate);
		}

		public static ConventionBase<IType, IIdExtractor> IdByMember(
			this ConventionBuilder<IType, IIdExtractor> source, Func<IMember, bool> memberFilter)
		{
			return source.IdByMember(memberFilter, e => e);
		}

		public static ConventionBase<IType, IIdExtractor> IdByMember(
			this ConventionBuilder<IType, IIdExtractor> source, Func<IMember, bool> memberFilter, Func<MemberValueExtractor, MemberValueExtractor> configurationDelegate)
		{
			return source
				.By(t => configurationDelegate(BuildRoutine.IdExtractor().ByMemberValue(t.Members.First(memberFilter))))
				.When(t => t.Members.Any(memberFilter));
		}

		#endregion

		#region IdByOperation

		public static ConventionBase<IType, IIdExtractor> IdByPublicOperation(
			this ConventionBuilder<IType, IIdExtractor> source, Func<IOperation, bool> operationFilter)
		{
			return source.IdByPublicOperation(operationFilter, e => e);
		}

		public static ConventionBase<IType, IIdExtractor> IdByPublicOperation(
			this ConventionBuilder<IType, IIdExtractor> source, Func<IOperation, bool> operationFilter, Func<MemberValueExtractor, MemberValueExtractor> configurationDelegate)
		{
			return source.IdByOperation(operationFilter.And(o => o.IsPublic), configurationDelegate);
		}

		public static ConventionBase<IType, IIdExtractor> IdByOperation(
			this ConventionBuilder<IType, IIdExtractor> source, Func<IOperation, bool> operationFilter)
		{
			return source.IdByOperation(operationFilter, e => e);
		}

		public static ConventionBase<IType, IIdExtractor> IdByOperation(
			this ConventionBuilder<IType, IIdExtractor> source, Func<IOperation, bool> operationFilter, Func<MemberValueExtractor, MemberValueExtractor> configurationDelegate)
		{
			return source
				.By(t => configurationDelegate(BuildRoutine.IdExtractor()
					.ByMemberValue(new OperationMember(t.Operations.First(operationFilter.And(o => o.HasNoParameters() && !o.ReturnsVoid()))))))
				.When(t => t.Operations.Any(operationFilter.And(o => o.HasNoParameters() && !o.ReturnsVoid()))) as DelegateBasedConvention<IType, IIdExtractor>;
		}

		#endregion

		#region ValueByMember

		public static ConventionBase<IType, IValueExtractor> ValueByPublicMember(
			this ConventionBuilder<IType, IValueExtractor> source, Func<IMember, bool> memberFilter)
		{
			return source.ValueByPublicMember(memberFilter, e => e);
		}

		public static ConventionBase<IType, IValueExtractor> ValueByPublicMember(
			this ConventionBuilder<IType, IValueExtractor> source, Func<IMember, bool> memberFilter, Func<MemberValueExtractor, MemberValueExtractor> configurationDelegate)
		{
			return source.ValueByMember(memberFilter.And(m => m.IsPublic), configurationDelegate);
		}

		public static ConventionBase<IType, IValueExtractor> ValueByMember(
			this ConventionBuilder<IType, IValueExtractor> source, Func<IMember, bool> memberFilter)
		{
			return source.ValueByMember(memberFilter, e => e);
		}

		public static ConventionBase<IType, IValueExtractor> ValueByMember(
			this ConventionBuilder<IType, IValueExtractor> source, Func<IMember, bool> memberFilter, Func<MemberValueExtractor, MemberValueExtractor> configurationDelegate)
		{
			return source
				.By(t => configurationDelegate(BuildRoutine.ValueExtractor().ByMemberValue(t.Members.First(memberFilter))))
				.When(t => t.Members.Any(memberFilter));
		}

		#endregion

		#region ValueByOperation

		public static ConventionBase<IType, IValueExtractor> ValueByPublicOperation(
			this ConventionBuilder<IType, IValueExtractor> source, Func<IOperation, bool> operationFilter)
		{
			return source.ValueByPublicOperation(operationFilter.And(o => o.IsPublic), e => e);
		}

		public static ConventionBase<IType, IValueExtractor> ValueByPublicOperation(
			this ConventionBuilder<IType, IValueExtractor> source, Func<IOperation, bool> operationFilter, Func<MemberValueExtractor, MemberValueExtractor> configurationDelegate)
		{
			return source.ValueByOperation(operationFilter.And(o => o.IsPublic), configurationDelegate);
		}

		public static ConventionBase<IType, IValueExtractor> ValueByOperation(
			this ConventionBuilder<IType, IValueExtractor> source, Func<IOperation, bool> operationFilter)
		{
			return source.ValueByOperation(operationFilter, e => e);
		}

		public static ConventionBase<IType, IValueExtractor> ValueByOperation(
			this ConventionBuilder<IType, IValueExtractor> source, Func<IOperation, bool> operationFilter, Func<MemberValueExtractor, MemberValueExtractor> configurationDelegate)
		{
			return source
				.By(t => configurationDelegate(BuildRoutine.ValueExtractor()
					.ByMemberValue(new OperationMember(t.Operations.First(operationFilter.And(o => o.HasNoParameters() && !o.ReturnsVoid()))))))
				.When(t => t.Operations.Any(operationFilter.And(o => o.HasNoParameters() && !o.ReturnsVoid())));
		}

		#endregion

		#endregion

		#region PatternBuilder

		public static ConventionalCodingStyle Use(this ConventionalCodingStyle source, Func<PatternBuilder<ConventionalCodingStyle>, ConventionalCodingStyle> pattern)
		{
			return source.Merge(pattern(BuildRoutine.CodingStylePattern()));
		}

		public static ConventionalInterceptionConfiguration Use(this ConventionalInterceptionConfiguration source, Func<PatternBuilder<ConventionalInterceptionConfiguration>, ConventionalInterceptionConfiguration> pattern)
		{
			return source.Merge(pattern(BuildRoutine.InterceptionPattern()));
		}

		public static ConventionalSoaConfiguration Use(this ConventionalSoaConfiguration source, Func<PatternBuilder<ConventionalSoaConfiguration>, ConventionalSoaConfiguration> pattern)
		{
			return source.Merge(pattern(BuildRoutine.SoaPattern()));
		}

		public static ConventionalApiConfiguration Use(this ConventionalApiConfiguration source, Func<PatternBuilder<ConventionalApiConfiguration>, ConventionalApiConfiguration> pattern)
		{
			return source.Merge(pattern(BuildRoutine.ApiGenerationPattern()));
		}

		public static ConventionalMvcConfiguration Use(this ConventionalMvcConfiguration source, Func<PatternBuilder<ConventionalMvcConfiguration>, ConventionalMvcConfiguration> pattern)
		{
			return source.Merge(pattern(BuildRoutine.MvcPattern()));
		}

		public static ConventionalSoaClientConfiguration Use(this ConventionalSoaClientConfiguration source, Func<PatternBuilder<ConventionalSoaClientConfiguration>, ConventionalSoaClientConfiguration> pattern)
		{
			return source.Merge(pattern(BuildRoutine.SoaClientPattern()));
		}

		#endregion

		#region Virtual

		public static ConventionalCodingStyle AddTypes(this ConventionalCodingStyle source, params Func<VirtualTypeBuilder, VirtualType>[] typeBuilders)
		{
			return source.AddTypes(typeBuilders.Select(builder => builder(BuildRoutine.VirtualType())));
		}

		public static TConfiguration Add<TConfiguration>(this ListConfiguration<TConfiguration, IOperation> source, Func<OperationBuilder, IEnumerable<IOperation>> builder)
			where TConfiguration : IType
		{
			return source.Add(t => builder(BuildRoutine.Operation(t)));
		}

		public static TConfiguration Add<TConfiguration>(this ListConfiguration<TConfiguration, IOperation> source, Func<OperationBuilder, IOperation> builder)
			where TConfiguration : IType
		{
			return source.Add(t => builder(BuildRoutine.Operation(t)));
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

		public static ConventionBase<IType, List<IOperation>> Build(this ConventionBuilder<IType, List<IOperation>> source, Func<OperationBuilder, IEnumerable<IOperation>> builder)
		{
			return source.By(t => builder(BuildRoutine.Operation(t)).ToList());
		}

		public static ConventionBase<IType, List<IOperation>> Build(this ConventionBuilder<IType, List<IOperation>> source, Func<OperationBuilder, IOperation> builder)
		{
			return source.By(t => new List<IOperation> { builder(BuildRoutine.Operation(t)) });
		}

		#endregion
	}
}
