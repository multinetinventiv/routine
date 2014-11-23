using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core.Configuration;

namespace Routine.Engine.Configuration.Conventional
{
	public class ConventionalCodingStyle : ICodingStyle
	{
		private readonly List<IType> types;

		public ConventionalConfiguration<ConventionalCodingStyle, IType, string> TypeId { get; private set; }
		public ConventionalConfiguration<ConventionalCodingStyle, IType, string> Module { get; private set; }
		public ConventionalConfiguration<ConventionalCodingStyle, IType, bool> TypeIsValue { get; private set; }
		public ConventionalConfiguration<ConventionalCodingStyle, IType, bool> TypeIsView { get; private set; }
		public ConventionalConfiguration<ConventionalCodingStyle, IType, IIdExtractor> IdExtractor { get; private set; }
		public ConventionalConfiguration<ConventionalCodingStyle, IType, IValueExtractor> ValueExtractor { get; private set; }
		public ConventionalConfiguration<ConventionalCodingStyle, IType, ILocator> ObjectLocator { get; private set; }
		public ConventionalConfiguration<ConventionalCodingStyle, IType, List<object>> StaticInstances { get; private set; }
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

			TypeId = new ConventionalConfiguration<ConventionalCodingStyle, IType, string>(this, "TypeId", true);
			Module = new ConventionalConfiguration<ConventionalCodingStyle, IType, string>(this, "Module");
			TypeIsValue = new ConventionalConfiguration<ConventionalCodingStyle, IType, bool>(this, "TypeIsValue");
			TypeIsView = new ConventionalConfiguration<ConventionalCodingStyle, IType, bool>(this, "TypeIsView");
			IdExtractor = new ConventionalConfiguration<ConventionalCodingStyle, IType, IIdExtractor>(this, "IdExtractor");
			ValueExtractor = new ConventionalConfiguration<ConventionalCodingStyle, IType, IValueExtractor>(this, "ValueExtractor");
			ObjectLocator = new ConventionalConfiguration<ConventionalCodingStyle, IType, ILocator>(this, "ObjectLocator");
			StaticInstances = new ConventionalConfiguration<ConventionalCodingStyle, IType, List<object>>(this, "StaticInstances");
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
			Module.Merge(other.Module);
			TypeIsValue.Merge(other.TypeIsValue);
			TypeIsView.Merge(other.TypeIsView);
			IdExtractor.Merge(other.IdExtractor);
			ValueExtractor.Merge(other.ValueExtractor);
			ObjectLocator.Merge(other.ObjectLocator);
			StaticInstances.Merge(other.StaticInstances);
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

			return AddTypes(types.Select(t => TypeInfo.Get(t) as IType));
		}

		public ConventionalCodingStyle AddTypes(params IType[] types) { return AddTypes(types as IEnumerable<IType>); }
		public ConventionalCodingStyle AddTypes(IEnumerable<IType> types)
		{
			this.types.AddRange(types.Where(t => !this.types.Contains(t)));

			return this;
		}

		#region ICodingStyle implementation

		List<IType> ICodingStyle.GetTypes() { return types; }

		string ICodingStyle.GetTypeId(IType type) { return TypeId.Get(type); }
		string ICodingStyle.GetModuleName(IType type) { return Module.Get(type); }
		bool ICodingStyle.IsValue(IType type) { return TypeIsValue.Get(type); }
		bool ICodingStyle.IsView(IType type) { return TypeIsView.Get(type); }
		IIdExtractor ICodingStyle.GetIdExtractor(IType type) { return IdExtractor.Get(type); }
		IValueExtractor ICodingStyle.GetValueExtractor(IType type) { return ValueExtractor.Get(type); }
		ILocator ICodingStyle.GetObjectLocator(IType type) { return ObjectLocator.Get(type); }
		List<object> ICodingStyle.GetStaticInstances(IType type) { return StaticInstances.Get(type); }
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

