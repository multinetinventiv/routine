using System;
using System.Collections.Generic;
using Routine.Client;
using Routine.Core.Configuration;
using Routine.Engine;

namespace Routine.Api.Configuration
{
	public class ConventionalApiConfiguration : LayeredBase<ConventionalApiConfiguration>, IApiConfiguration
	{
		public SingleConfiguration<ConventionalApiConfiguration, string> DefaultNamespace { get; private set; }
		public SingleConfiguration<ConventionalApiConfiguration, bool> InMemory { get; private set; }
		public SingleConfiguration<ConventionalApiConfiguration, string> OutputFileName { get; private set; }

		public ConventionalConfiguration<ConventionalApiConfiguration, ApplicationCodeModel, Version> AssemblyVersion { get; private set; }
		public ConventionalConfiguration<ConventionalApiConfiguration, ApplicationCodeModel, Guid> AssemblyGuid { get; private set; }

		public ListConfiguration<ConventionalApiConfiguration, string> FriendlyAssemblyNames { get; private set; }

		public ConventionalConfiguration<ConventionalApiConfiguration, Rtype, bool> TypeIsRendered { get; private set; }

		public ConventionalConfiguration<ConventionalApiConfiguration, Rinitializer, bool> InitializerIsRendered { get; private set; }
		public ConventionalConfiguration<ConventionalApiConfiguration, Rmember, bool> MemberIsRendered { get; private set; }
		public ConventionalConfiguration<ConventionalApiConfiguration, Roperation, bool> OperationIsRendered { get; private set; }

		public ConventionalListConfiguration<ConventionalApiConfiguration, TypeCodeModel, int> Modes { get; private set; }
		public ConventionalConfiguration<ConventionalApiConfiguration, ModedModel<TypeCodeModel>, string> RenderedTypeNamespace { get; private set; }
		public ConventionalConfiguration<ConventionalApiConfiguration, ModedModel<TypeCodeModel>, ITypeConversionTemplate> RenderedTypeTemplate { get; private set; }

		public ConventionalConfiguration<ConventionalApiConfiguration, ModedModel<TypeCodeModel>, string> RenderedTypeName { get; private set; }
		public ConventionalConfiguration<ConventionalApiConfiguration, ModedModel<MemberCodeModel>, string> RenderedMemberName { get; private set; }
		public ConventionalConfiguration<ConventionalApiConfiguration, ModedModel<OperationCodeModel>, string> RenderedOperationName { get; private set; }
		public ConventionalConfiguration<ConventionalApiConfiguration, ModedModel<ParameterCodeModel>, string> RenderedParameterName { get; private set; }

		public ConventionalConfiguration<ConventionalApiConfiguration, Rtype, IType> ReferencedType { get; private set; }
		public ConventionalConfiguration<ConventionalApiConfiguration, IType, ITypeConversionTemplate> ReferencedTypeTemplate { get; private set; }

		public ConventionalApiConfiguration()
		{
			DefaultNamespace = new SingleConfiguration<ConventionalApiConfiguration, string>(this, "DefaultNamespace", true);
			InMemory = new SingleConfiguration<ConventionalApiConfiguration, bool>(this, "InMemory");
			OutputFileName = new SingleConfiguration<ConventionalApiConfiguration, string>(this, "OutputFileName");

			AssemblyVersion = new ConventionalConfiguration<ConventionalApiConfiguration, ApplicationCodeModel, Version>(this, "AssemblyVersion", true);
			AssemblyGuid = new ConventionalConfiguration<ConventionalApiConfiguration, ApplicationCodeModel, Guid>(this, "AssemblyGuid", true);

			FriendlyAssemblyNames = new ListConfiguration<ConventionalApiConfiguration, string>(this, "FriendlyAssemblyNames");

			TypeIsRendered = new ConventionalConfiguration<ConventionalApiConfiguration, Rtype, bool>(this, "TypeIsRendered", true);
			InitializerIsRendered = new ConventionalConfiguration<ConventionalApiConfiguration, Rinitializer, bool>(this, "InitializerIsRendered", true);
			MemberIsRendered = new ConventionalConfiguration<ConventionalApiConfiguration, Rmember, bool>(this, "MemberIsRendered", true);
			OperationIsRendered = new ConventionalConfiguration<ConventionalApiConfiguration, Roperation, bool>(this, "OperationIsRendered", true);

			Modes = new ConventionalListConfiguration<ConventionalApiConfiguration, TypeCodeModel, int>(this, "Modes", true);
			RenderedTypeNamespace = new ConventionalConfiguration<ConventionalApiConfiguration, ModedModel<TypeCodeModel>, string>(this, "RenderedTypeNamespace", true);
			RenderedTypeTemplate = new ConventionalConfiguration<ConventionalApiConfiguration, ModedModel<TypeCodeModel>, ITypeConversionTemplate>(this, "RenderedTypeTemplate", true);

			RenderedTypeName = new ConventionalConfiguration<ConventionalApiConfiguration, ModedModel<TypeCodeModel>, string>(this, "RenderedTypeName", true);
			RenderedMemberName = new ConventionalConfiguration<ConventionalApiConfiguration, ModedModel<MemberCodeModel>, string>(this, "RenderedMemberName", true);
			RenderedOperationName = new ConventionalConfiguration<ConventionalApiConfiguration, ModedModel<OperationCodeModel>, string>(this, "RenderedOperationName", true);
			RenderedParameterName = new ConventionalConfiguration<ConventionalApiConfiguration, ModedModel<ParameterCodeModel>, string>(this, "RenderedParameterName", true);

			ReferencedType = new ConventionalConfiguration<ConventionalApiConfiguration, Rtype, IType>(this, "ReferencedType", true);
			ReferencedTypeTemplate = new ConventionalConfiguration<ConventionalApiConfiguration, IType, ITypeConversionTemplate>(this, "ReferencedTypeTemplate", true);
		}

		public ConventionalApiConfiguration Merge(ConventionalApiConfiguration other)
		{
			AssemblyVersion.Merge(other.AssemblyVersion);
			AssemblyGuid.Merge(other.AssemblyGuid);

			FriendlyAssemblyNames.Merge(other.FriendlyAssemblyNames);

			TypeIsRendered.Merge(other.TypeIsRendered);
			InitializerIsRendered.Merge(other.InitializerIsRendered);
			MemberIsRendered.Merge(other.MemberIsRendered);
			OperationIsRendered.Merge(other.OperationIsRendered);

			Modes.Merge(other.Modes);
			RenderedTypeNamespace.Merge(other.RenderedTypeNamespace);
			RenderedTypeTemplate.Merge(other.RenderedTypeTemplate);

			RenderedTypeName.Merge(other.RenderedTypeName);
			RenderedMemberName.Merge(other.RenderedMemberName);
			RenderedOperationName.Merge(other.RenderedOperationName);
			RenderedParameterName.Merge(other.RenderedParameterName);

			ReferencedType.Merge(other.ReferencedType);
			ReferencedTypeTemplate.Merge(other.ReferencedTypeTemplate);

			return this;
		}

		#region IApiConfiguration implementation

		string IApiConfiguration.GetDefaultNamespace() { return DefaultNamespace.Get(); }
		bool IApiConfiguration.GetInMemory() { return InMemory.Get(); }
		string IApiConfiguration.GetOutputFileName() { return OutputFileName.Get(); }

		Version IApiConfiguration.GetAssemblyVersion(ApplicationCodeModel application) { return AssemblyVersion.Get(application); }
		Guid IApiConfiguration.GetAssemblyGuid(ApplicationCodeModel application) { return AssemblyGuid.Get(application); }

		List<string> IApiConfiguration.GetFriendlyAssemblyNames() { return FriendlyAssemblyNames.Get(); }

		bool IApiConfiguration.IsRendered(Rtype type) { return TypeIsRendered.Get(type); }
		bool IApiConfiguration.IsRendered(Rinitializer initializer) { return InitializerIsRendered.Get(initializer); }
		bool IApiConfiguration.IsRendered(Rmember member) { return MemberIsRendered.Get(member); }
		bool IApiConfiguration.IsRendered(Roperation operation) { return OperationIsRendered.Get(operation); }

		List<int> IApiConfiguration.GetModes(TypeCodeModel typeCodeModel) { return Modes.Get(typeCodeModel); }
		string IApiConfiguration.GetNamespace(TypeCodeModel typeCodeModel, int mode) { return RenderedTypeNamespace.Get(typeCodeModel.WithMode(mode)); }
		ITypeConversionTemplate IApiConfiguration.GetRenderedTypeTemplate(TypeCodeModel typeCodeModel, int mode) { return RenderedTypeTemplate.Get(typeCodeModel.WithMode(mode)); }

		string IApiConfiguration.GetName(TypeCodeModel typeCodeModel, int mode) { return RenderedTypeName.Get(typeCodeModel.WithMode(mode)); }
		string IApiConfiguration.GetName(MemberCodeModel memberCodeModel, int mode) { return RenderedMemberName.Get(memberCodeModel.WithMode(mode)); }
		string IApiConfiguration.GetName(OperationCodeModel operationCodeModel, int mode) { return RenderedOperationName.Get(operationCodeModel.WithMode(mode)); }
		string IApiConfiguration.GetName(ParameterCodeModel parameterCodeModel, int mode) { return RenderedParameterName.Get(parameterCodeModel.WithMode(mode)); }

		IType IApiConfiguration.GetReferencedType(Rtype type) { return ReferencedType.Get(type); }
		ITypeConversionTemplate IApiConfiguration.GetReferencedTypeTemplate(IType type) { return ReferencedTypeTemplate.Get(type); }

		#endregion
	}

	public class ModedModel<T>
	{
		public T Model { get; private set; }
		public int Mode { get; private set; }

		public ModedModel(T model, int mode)
		{
			Model = model;
			Mode = mode;
		}

		public override string ToString()
		{
			return string.Format("Model: {0}, Mode: {1}", Model, Mode);
		}
	}

	internal static class ModedModelExtensions
	{
		public static ModedModel<TModel> WithMode<TModel>(this TModel source, int mode)
		{
			return new ModedModel<TModel>(source, mode);
		}
	}
}
