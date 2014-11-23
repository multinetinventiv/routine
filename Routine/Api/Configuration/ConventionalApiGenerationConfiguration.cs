using System.Collections.Generic;
using Routine.Client;
using Routine.Core.Configuration;
using Routine.Engine;

namespace Routine.Api.Configuration
{
	public class ConventionalApiGenerationConfiguration : IApiGenerationConfiguration
	{
		public SingleConfiguration<ConventionalApiGenerationConfiguration, string> ApiName { get; private set; }
		public SingleConfiguration<ConventionalApiGenerationConfiguration, string> DefaultNamespace { get; private set; }
		public SingleConfiguration<ConventionalApiGenerationConfiguration, bool> InMemory { get; private set; }
		public SingleConfiguration<ConventionalApiGenerationConfiguration, bool> IgnoreReferencedTypeNotFound { get; private set; }

		public ListConfiguration<ConventionalApiGenerationConfiguration, string> FriendlyAssemblyNames { get; private set; }

		public ConventionalConfiguration<ConventionalApiGenerationConfiguration, Rtype, IType> ReferencedType { get; private set; }

		public ConventionalConfiguration<ConventionalApiGenerationConfiguration, IType, bool> ReferencedTypeIsValueType { get; private set; }
		public ConventionalConfiguration<ConventionalApiGenerationConfiguration, IType, IType> TargetValueType { get; private set; }

		public ConventionalConfiguration<ConventionalApiGenerationConfiguration, IType, string> StringToValueCodeTemplate { get; private set; }
		public ConventionalConfiguration<ConventionalApiGenerationConfiguration, IType, string> ValueToStringCodeTemplate { get; private set; }

		public ConventionalListConfiguration<ConventionalApiGenerationConfiguration, ObjectCodeModel, string> StaticInstanceIds { get; private set; }

		public ConventionalConfiguration<ConventionalApiGenerationConfiguration, Rtype, bool> TypeIsRendered { get; private set; }
		public ConventionalConfiguration<ConventionalApiGenerationConfiguration, Rinitializer, bool> InitializerIsRendered { get; private set; }
		public ConventionalConfiguration<ConventionalApiGenerationConfiguration, Rmember, bool> MemberIsRendered { get; private set; }
		public ConventionalConfiguration<ConventionalApiGenerationConfiguration, Roperation, bool> OperationIsRendered { get; private set; }

		public ConventionalApiGenerationConfiguration()
		{
			ApiName = new SingleConfiguration<ConventionalApiGenerationConfiguration, string>(this, "ApiName");
			DefaultNamespace = new SingleConfiguration<ConventionalApiGenerationConfiguration, string>(this, "DefaultNamespace", true);
			InMemory = new SingleConfiguration<ConventionalApiGenerationConfiguration, bool>(this, "InMemory");
			IgnoreReferencedTypeNotFound = new SingleConfiguration<ConventionalApiGenerationConfiguration, bool>(this, "IgnoreReferencedTypeNotFound", false);

			FriendlyAssemblyNames = new ListConfiguration<ConventionalApiGenerationConfiguration, string>(this, "FriendlyAssemblyNames");

			ReferencedType = new ConventionalConfiguration<ConventionalApiGenerationConfiguration, Rtype, IType>(this, "ReferencedType", true);

			ReferencedTypeIsValueType = new ConventionalConfiguration<ConventionalApiGenerationConfiguration, IType, bool>(this, "ReferencedTypeIsValueType");
			TargetValueType = new ConventionalConfiguration<ConventionalApiGenerationConfiguration, IType, IType>(this, "TargetValueType");

			StringToValueCodeTemplate = new ConventionalConfiguration<ConventionalApiGenerationConfiguration, IType, string>(this, "StringToValueCodeTemplate");
			ValueToStringCodeTemplate = new ConventionalConfiguration<ConventionalApiGenerationConfiguration, IType, string>(this, "ValueToStringCodeTemplate");

			StaticInstanceIds = new ConventionalListConfiguration<ConventionalApiGenerationConfiguration, ObjectCodeModel, string>(this, "StaticInstanceIds");

			TypeIsRendered = new ConventionalConfiguration<ConventionalApiGenerationConfiguration, Rtype, bool>(this, "TypeIsRendered");
			InitializerIsRendered = new ConventionalConfiguration<ConventionalApiGenerationConfiguration, Rinitializer, bool>(this, "InitializerIsRendered");
			MemberIsRendered = new ConventionalConfiguration<ConventionalApiGenerationConfiguration, Rmember, bool>(this, "MemberIsRendered");
			OperationIsRendered = new ConventionalConfiguration<ConventionalApiGenerationConfiguration, Roperation, bool>(this, "OperationIsRendered");
		}

		public ConventionalApiGenerationConfiguration Merge(ConventionalApiGenerationConfiguration other)
		{
			FriendlyAssemblyNames.Merge(other.FriendlyAssemblyNames);

			ReferencedType.Merge(other.ReferencedType);

			ReferencedTypeIsValueType.Merge(other.ReferencedTypeIsValueType);
			TargetValueType.Merge(other.TargetValueType);

			StringToValueCodeTemplate.Merge(other.StringToValueCodeTemplate);
			ValueToStringCodeTemplate.Merge(other.ValueToStringCodeTemplate);

			StaticInstanceIds.Merge(other.StaticInstanceIds);

			TypeIsRendered.Merge(other.TypeIsRendered);
			InitializerIsRendered.Merge(other.InitializerIsRendered);
			MemberIsRendered.Merge(other.MemberIsRendered);
			OperationIsRendered.Merge(other.OperationIsRendered);

			return this;
		}

		#region IApiGenerationConfiguration implementation

		string IApiGenerationConfiguration.GetApiName() { return ApiName.Get(); }
		string IApiGenerationConfiguration.GetDefaultNamespace() { return DefaultNamespace.Get(); }
		bool IApiGenerationConfiguration.GetInMemory() { return InMemory.Get(); }
		bool IApiGenerationConfiguration.GetIgnoreReferencedTypeNotFound() { return IgnoreReferencedTypeNotFound.Get(); }

		List<string> IApiGenerationConfiguration.GetFriendlyAssemblyNames() { return FriendlyAssemblyNames.Get(); }

		IType IApiGenerationConfiguration.GetReferencedType(Rtype type) { return ReferencedType.Get(type); }
		bool IApiGenerationConfiguration.GetReferencedTypeIsValueType(IType type) { return ReferencedTypeIsValueType.Get(type); }
		IType IApiGenerationConfiguration.GetTargetValueType(IType type) { return TargetValueType.Get(type); }
		string IApiGenerationConfiguration.GetStringToValueCodeTemplate(IType type) { return StringToValueCodeTemplate.Get(type); }
		string IApiGenerationConfiguration.GetValueToStringCodeTemplate(IType type) { return ValueToStringCodeTemplate.Get(type); }
		List<string> IApiGenerationConfiguration.GetStaticInstanceIds(ObjectCodeModel objectCodeModel) { return StaticInstanceIds.Get(objectCodeModel); }

		bool IApiGenerationConfiguration.IsRendered(Rtype type) { return TypeIsRendered.Get(type); }
		bool IApiGenerationConfiguration.IsRendered(Rinitializer initializer) { return InitializerIsRendered.Get(initializer); }
		bool IApiGenerationConfiguration.IsRendered(Rmember member) { return MemberIsRendered.Get(member); }
		bool IApiGenerationConfiguration.IsRendered(Roperation operation) { return OperationIsRendered.Get(operation); }

		#endregion
	}
}
