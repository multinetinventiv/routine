using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core.Configuration;
using Routine.Engine.Virtual;

namespace Routine.Engine.Configuration.Conventional
{
	public class ConventionalCodingStyle : LayeredBase<ConventionalCodingStyle>, ICodingStyle
	{
		private readonly List<IType> types;

		public SingleConfiguration<ConventionalCodingStyle, int> MaxFetchDepth { get; private set; }

		public ConventionalConfiguration<ConventionalCodingStyle, IType, string> TypeId { get; private set; }
		public ConventionalConfiguration<ConventionalCodingStyle, object, IType> Type { get; private set; }
		public ConventionalConfiguration<ConventionalCodingStyle, IType, string> Module { get; private set; }
		public ConventionalConfiguration<ConventionalCodingStyle, IType, bool> TypeIsValue { get; private set; }
		public ConventionalConfiguration<ConventionalCodingStyle, IType, bool> TypeIsView { get; private set; }
		public ConventionalConfiguration<ConventionalCodingStyle, IType, IIdExtractor> IdExtractor { get; private set; }
		public ConventionalConfiguration<ConventionalCodingStyle, IType, IValueExtractor> ValueExtractor { get; private set; }
		public ConventionalConfiguration<ConventionalCodingStyle, IType, ILocator> Locator { get; private set; }
		public ConventionalConfiguration<ConventionalCodingStyle, IType, IConverter> Converter { get; private set; }
		public ConventionalConfiguration<ConventionalCodingStyle, IType, List<object>> StaticInstances { get; private set; }
		public ConventionalListConfiguration<ConventionalCodingStyle, IType, IType> ViewTypes { get; private set; }
		public ConventionalListConfiguration<ConventionalCodingStyle, IType, IInitializer> Initializers { get; private set; }
		public ConventionalListConfiguration<ConventionalCodingStyle, IType, IMember> Members { get; private set; }
		public ConventionalListConfiguration<ConventionalCodingStyle, IType, IOperation> Operations { get; private set; }

		public ConventionalConfiguration<ConventionalCodingStyle, IMember, bool> MemberFetchedEagerly { get; private set; }

		public ConventionalListConfiguration<ConventionalCodingStyle, IType, string> TypeMarks { get; private set; }
		public ConventionalListConfiguration<ConventionalCodingStyle, IInitializer, string> InitializerMarks { get; private set; }
		public ConventionalListConfiguration<ConventionalCodingStyle, IMember, string> MemberMarks { get; private set; }
		public ConventionalListConfiguration<ConventionalCodingStyle, IOperation, string> OperationMarks { get; private set; }
		public ConventionalListConfiguration<ConventionalCodingStyle, IParameter, string> ParameterMarks { get; private set; }

		public ConventionalCodingStyle()
		{
			types = new List<IType>();

			MaxFetchDepth = new SingleConfiguration<ConventionalCodingStyle, int>(this, "MaxFetchDepth", true);

			TypeId = new ConventionalConfiguration<ConventionalCodingStyle, IType, string>(this, "TypeId", true);
			Type = new ConventionalConfiguration<ConventionalCodingStyle, object, IType>(this, "Type");
			Module = new ConventionalConfiguration<ConventionalCodingStyle, IType, string>(this, "Module");
			TypeIsValue = new ConventionalConfiguration<ConventionalCodingStyle, IType, bool>(this, "TypeIsValue");
			TypeIsView = new ConventionalConfiguration<ConventionalCodingStyle, IType, bool>(this, "TypeIsView");
			IdExtractor = new ConventionalConfiguration<ConventionalCodingStyle, IType, IIdExtractor>(this, "IdExtractor");
			ValueExtractor = new ConventionalConfiguration<ConventionalCodingStyle, IType, IValueExtractor>(this, "ValueExtractor");
			Locator = new ConventionalConfiguration<ConventionalCodingStyle, IType, ILocator>(this, "Locator");
			Converter = new ConventionalConfiguration<ConventionalCodingStyle, IType, IConverter>(this, "Converter");
			StaticInstances = new ConventionalConfiguration<ConventionalCodingStyle, IType, List<object>>(this, "StaticInstances");
			ViewTypes = new ConventionalListConfiguration<ConventionalCodingStyle, IType, IType>(this, "ViewTypes");
			Initializers = new ConventionalListConfiguration<ConventionalCodingStyle, IType, IInitializer>(this, "Initializers");
			Members = new ConventionalListConfiguration<ConventionalCodingStyle, IType, IMember>(this, "Members");
			Operations = new ConventionalListConfiguration<ConventionalCodingStyle, IType, IOperation>(this, "Operations");

			MemberFetchedEagerly = new ConventionalConfiguration<ConventionalCodingStyle, IMember, bool>(this, "MemberFetchedEagerly");

			TypeMarks = new ConventionalListConfiguration<ConventionalCodingStyle, IType, string>(this, "TypeMarks");
			InitializerMarks = new ConventionalListConfiguration<ConventionalCodingStyle, IInitializer, string>(this, "InitializerMarks");
			MemberMarks = new ConventionalListConfiguration<ConventionalCodingStyle, IMember, string>(this, "MemberMarks");
			OperationMarks = new ConventionalListConfiguration<ConventionalCodingStyle, IOperation, string>(this, "OperationMarks");
			ParameterMarks = new ConventionalListConfiguration<ConventionalCodingStyle, IParameter, string>(this, "ParameterMarks");
		}

		public ConventionalCodingStyle Merge(ConventionalCodingStyle other)
		{
			AddTypes(other.types);

			TypeId.Merge(other.TypeId);
			Type.Merge(other.Type);
			Module.Merge(other.Module);
			TypeIsValue.Merge(other.TypeIsValue);
			TypeIsView.Merge(other.TypeIsView);
			IdExtractor.Merge(other.IdExtractor);
			ValueExtractor.Merge(other.ValueExtractor);
			Locator.Merge(other.Locator);
			Converter.Merge(other.Converter);
			StaticInstances.Merge(other.StaticInstances);
			ViewTypes.Merge(other.ViewTypes);
			Initializers.Merge(other.Initializers);
			Members.Merge(other.Members);
			Operations.Merge(other.Operations);

			MemberFetchedEagerly.Merge(other.MemberFetchedEagerly);

			TypeMarks.Merge(other.TypeMarks);
			InitializerMarks.Merge(other.InitializerMarks);
			MemberMarks.Merge(other.MemberMarks);
			OperationMarks.Merge(other.OperationMarks);
			ParameterMarks.Merge(other.ParameterMarks);

			return this;
		}

		public ConventionalCodingStyle RecognizeProxyTypesBy(Func<Type, bool> proxyMatcher, Func<Type, Type> actualTypeGetter)
		{
			TypeInfo.SetProxyMatcher(proxyMatcher, actualTypeGetter);

			return this;
		}

		public ConventionalCodingStyle AddTypes(IEnumerable<System.Reflection.Assembly> assemblies) { return AddTypes(assemblies, t => true); }
		public ConventionalCodingStyle AddTypes(IEnumerable<System.Reflection.Assembly> assemblies, Func<Type, bool> typeFilter)
		{
			foreach (var assembly in assemblies)
			{
				AddTypes(assembly, typeFilter);
			}

			return this;
		}

		public ConventionalCodingStyle AddTypes(System.Reflection.Assembly assembly) { return AddTypes(assembly, t => true); }
		public ConventionalCodingStyle AddTypes(System.Reflection.Assembly assembly, Func<Type, bool> typeFilter) { return AddTypes(assembly.GetTypes().Where(typeFilter)); }
		public ConventionalCodingStyle AddTypes(IEnumerable<Type> types) { return AddTypes(types.ToArray()); }
		public ConventionalCodingStyle AddTypes(params Type[] types)
		{
			TypeInfo.AddDomainTypes(types);

			//TODO refactor - TypeInfo should handle this by itself. Proxy instances should be given, so that domain type changes affects immediately
			//refresh previously cached type info instances
			for (int i = 0; i < this.types.Count; i++)
			{
				var type = this.types[i];

				var typeInfo = type as TypeInfo;
				if (typeInfo == null) { continue; }
				
				this.types[i] = TypeInfo.Get(typeInfo.GetActualType());
			}
			return AddTypes(types.Select(t => TypeInfo.Get(t) as IType));
		}

		public ConventionalCodingStyle AddTypes(params IType[] types) { return AddTypes(types.AsEnumerable()); }
		public ConventionalCodingStyle AddTypes(IEnumerable<IType> types)
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

		#region ICodingStyle implementation

		int ICodingStyle.GetMaxFetchDepth() { return MaxFetchDepth.Get(); }

		List<IType> ICodingStyle.GetTypes() { return types; }

		IType ICodingStyle.GetType(object @object) { return Type.Get(@object); }

		string ICodingStyle.GetTypeId(IType type) { return TypeId.Get(type); }
		string ICodingStyle.GetModuleName(IType type) { return Module.Get(type); }
		bool ICodingStyle.IsValue(IType type) { return TypeIsValue.Get(type); }
		bool ICodingStyle.IsView(IType type) { return TypeIsView.Get(type); }
		IIdExtractor ICodingStyle.GetIdExtractor(IType type) { return IdExtractor.Get(type); }
		IValueExtractor ICodingStyle.GetValueExtractor(IType type) { return ValueExtractor.Get(type); }
		ILocator ICodingStyle.GetLocator(IType type) { return Locator.Get(type); }
		IConverter ICodingStyle.GetConverter(IType type) { return Converter.Get(type); }
		List<object> ICodingStyle.GetStaticInstances(IType type) { return StaticInstances.Get(type); }
		List<IType> ICodingStyle.GetViewTypes(IType type) { return ViewTypes.Get(type); }
		List<IInitializer> ICodingStyle.GetInitializers(IType type) { return Initializers.Get(type); }
		List<IMember> ICodingStyle.GetMembers(IType type) { return Members.Get(type); }
		List<IOperation> ICodingStyle.GetOperations(IType type) { return Operations.Get(type); }

		bool ICodingStyle.IsFetchedEagerly(IMember member) { return MemberFetchedEagerly.Get(member); }

		List<string> ICodingStyle.GetMarks(IType type) { return TypeMarks.Get(type); }
		List<string> ICodingStyle.GetMarks(IInitializer initializer) { return InitializerMarks.Get(initializer); }
		List<string> ICodingStyle.GetMarks(IMember member) { return MemberMarks.Get(member); }
		List<string> ICodingStyle.GetMarks(IOperation operation) { return OperationMarks.Get(operation); }
		List<string> ICodingStyle.GetMarks(IParameter parameter) { return ParameterMarks.Get(parameter); }

		#endregion
	}
}

