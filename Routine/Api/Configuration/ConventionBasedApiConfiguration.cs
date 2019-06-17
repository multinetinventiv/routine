using System;
using System.Collections.Generic;
using Routine.Client;
using Routine.Core.Configuration;

namespace Routine.Api.Configuration
{
	public class ConventionBasedApiConfiguration : LayeredBase<ConventionBasedApiConfiguration>, IApiConfiguration
	{
		public SingleConfiguration<ConventionBasedApiConfiguration, string> DefaultNamespace { get; private set; }
		public SingleConfiguration<ConventionBasedApiConfiguration, bool> InMemory { get; private set; }
		public SingleConfiguration<ConventionBasedApiConfiguration, string> OutputFileName { get; private set; }

		public ConventionBasedConfiguration<ConventionBasedApiConfiguration, ApplicationCodeModel, Version> AssemblyVersion { get; private set; }
		public ConventionBasedConfiguration<ConventionBasedApiConfiguration, ApplicationCodeModel, Guid> AssemblyGuid { get; private set; }

		public ListConfiguration<ConventionBasedApiConfiguration, string> FriendlyAssemblyNames { get; private set; }

		public ConventionBasedConfiguration<ConventionBasedApiConfiguration, Rtype, bool> TypeIsRendered { get; private set; }

		public ConventionBasedConfiguration<ConventionBasedApiConfiguration, Rinitializer, bool> InitializerIsRendered { get; private set; }
		public ConventionBasedConfiguration<ConventionBasedApiConfiguration, Rdata, bool> DataIsRendered { get; private set; }
		public ConventionBasedConfiguration<ConventionBasedApiConfiguration, Roperation, bool> OperationIsRendered { get; private set; }

		public ConventionBasedListConfiguration<ConventionBasedApiConfiguration, TypeCodeModel, int> Modes { get; private set; }
		public ConventionBasedConfiguration<ConventionBasedApiConfiguration, ModedModel<TypeCodeModel>, string> RenderedTypeNamespace { get; private set; }
		public ConventionBasedConfiguration<ConventionBasedApiConfiguration, ModedModel<TypeCodeModel>, ITypeConversionTemplate> RenderedTypeTemplate { get; private set; }

		public ConventionBasedConfiguration<ConventionBasedApiConfiguration, ModedModel<TypeCodeModel>, string> RenderedTypeName { get; private set; }
		public ConventionBasedConfiguration<ConventionBasedApiConfiguration, ModedModel<DataCodeModel>, string> RenderedDataName { get; private set; }
		public ConventionBasedConfiguration<ConventionBasedApiConfiguration, ModedModel<OperationCodeModel>, string> RenderedOperationName { get; private set; }
		public ConventionBasedConfiguration<ConventionBasedApiConfiguration, ModedModel<ParameterCodeModel>, string> RenderedParameterName { get; private set; }

		public ConventionBasedListConfiguration<ConventionBasedApiConfiguration, ModedModel<TypeCodeModel>, Type> RenderedTypeAttributes { get; private set; }
		public ConventionBasedListConfiguration<ConventionBasedApiConfiguration, ModedModel<InitializerCodeModel>, Type> RenderedInitializerAttributes { get; private set; }
		public ConventionBasedListConfiguration<ConventionBasedApiConfiguration, ModedModel<DataCodeModel>, Type> RenderedDataAttributes { get; private set; }
		public ConventionBasedListConfiguration<ConventionBasedApiConfiguration, ModedModel<OperationCodeModel>, Type> RenderedOperationAttributes { get; private set; }
		public ConventionBasedListConfiguration<ConventionBasedApiConfiguration, ModedModel<ParameterCodeModel>, Type> RenderedParameterAttributes { get; private set; }

		public ConventionBasedConfiguration<ConventionBasedApiConfiguration, Rtype, Type> ReferencedType { get; private set; }
		public ConventionBasedConfiguration<ConventionBasedApiConfiguration, Type, ITypeConversionTemplate> ReferencedTypeTemplate { get; private set; }

		public ConventionBasedApiConfiguration()
		{
			DefaultNamespace = new SingleConfiguration<ConventionBasedApiConfiguration, string>(this, "DefaultNamespace", true);
			InMemory = new SingleConfiguration<ConventionBasedApiConfiguration, bool>(this, "InMemory");
			OutputFileName = new SingleConfiguration<ConventionBasedApiConfiguration, string>(this, "OutputFileName");

			AssemblyVersion = new ConventionBasedConfiguration<ConventionBasedApiConfiguration, ApplicationCodeModel, Version>(this, "AssemblyVersion", true);
			AssemblyGuid = new ConventionBasedConfiguration<ConventionBasedApiConfiguration, ApplicationCodeModel, Guid>(this, "AssemblyGuid", true);

			FriendlyAssemblyNames = new ListConfiguration<ConventionBasedApiConfiguration, string>(this, "FriendlyAssemblyNames");

			TypeIsRendered = new ConventionBasedConfiguration<ConventionBasedApiConfiguration, Rtype, bool>(this, "TypeIsRendered", true);
			InitializerIsRendered = new ConventionBasedConfiguration<ConventionBasedApiConfiguration, Rinitializer, bool>(this, "InitializerIsRendered", true);
			DataIsRendered = new ConventionBasedConfiguration<ConventionBasedApiConfiguration, Rdata, bool>(this, "DataIsRendered", true);
			OperationIsRendered = new ConventionBasedConfiguration<ConventionBasedApiConfiguration, Roperation, bool>(this, "OperationIsRendered", true);

			Modes = new ConventionBasedListConfiguration<ConventionBasedApiConfiguration, TypeCodeModel, int>(this, "Modes", true);
			RenderedTypeNamespace = new ConventionBasedConfiguration<ConventionBasedApiConfiguration, ModedModel<TypeCodeModel>, string>(this, "RenderedTypeNamespace", true);
			RenderedTypeTemplate = new ConventionBasedConfiguration<ConventionBasedApiConfiguration, ModedModel<TypeCodeModel>, ITypeConversionTemplate>(this, "RenderedTypeTemplate", true);

			RenderedTypeName = new ConventionBasedConfiguration<ConventionBasedApiConfiguration, ModedModel<TypeCodeModel>, string>(this, "RenderedTypeName", true);
			RenderedDataName = new ConventionBasedConfiguration<ConventionBasedApiConfiguration, ModedModel<DataCodeModel>, string>(this, "RenderedDataName", true);
			RenderedOperationName = new ConventionBasedConfiguration<ConventionBasedApiConfiguration, ModedModel<OperationCodeModel>, string>(this, "RenderedOperationName", true);
			RenderedParameterName = new ConventionBasedConfiguration<ConventionBasedApiConfiguration, ModedModel<ParameterCodeModel>, string>(this, "RenderedParameterName", true);

			RenderedTypeAttributes = new ConventionBasedListConfiguration<ConventionBasedApiConfiguration, ModedModel<TypeCodeModel>, Type>(this, "RenderedTypeAttributes", true);
			RenderedInitializerAttributes = new ConventionBasedListConfiguration<ConventionBasedApiConfiguration, ModedModel<InitializerCodeModel>, Type>(this, "RenderedInitializerAttributes", true);
			RenderedDataAttributes = new ConventionBasedListConfiguration<ConventionBasedApiConfiguration, ModedModel<DataCodeModel>, Type>(this, "RenderedDataAttributes", true);
			RenderedOperationAttributes = new ConventionBasedListConfiguration<ConventionBasedApiConfiguration, ModedModel<OperationCodeModel>, Type>(this, "RenderedOperationAttributes", true);
			RenderedParameterAttributes = new ConventionBasedListConfiguration<ConventionBasedApiConfiguration, ModedModel<ParameterCodeModel>, Type>(this, "RenderedParameterAttributes", true);

			ReferencedType = new ConventionBasedConfiguration<ConventionBasedApiConfiguration, Rtype, Type>(this, "ReferencedType", true);
			ReferencedTypeTemplate = new ConventionBasedConfiguration<ConventionBasedApiConfiguration, Type, ITypeConversionTemplate>(this, "ReferencedTypeTemplate", true);
		}

		public ConventionBasedApiConfiguration Merge(ConventionBasedApiConfiguration other)
		{
			AssemblyVersion.Merge(other.AssemblyVersion);
			AssemblyGuid.Merge(other.AssemblyGuid);

			FriendlyAssemblyNames.Merge(other.FriendlyAssemblyNames);

			TypeIsRendered.Merge(other.TypeIsRendered);
			InitializerIsRendered.Merge(other.InitializerIsRendered);
			DataIsRendered.Merge(other.DataIsRendered);
			OperationIsRendered.Merge(other.OperationIsRendered);

			Modes.Merge(other.Modes);
			RenderedTypeNamespace.Merge(other.RenderedTypeNamespace);
			RenderedTypeTemplate.Merge(other.RenderedTypeTemplate);

			RenderedTypeName.Merge(other.RenderedTypeName);
			RenderedDataName.Merge(other.RenderedDataName);
			RenderedOperationName.Merge(other.RenderedOperationName);
			RenderedParameterName.Merge(other.RenderedParameterName);

			RenderedTypeAttributes.Merge(other.RenderedTypeAttributes);
			RenderedInitializerAttributes.Merge(other.RenderedInitializerAttributes);
			RenderedDataAttributes.Merge(other.RenderedDataAttributes);
			RenderedOperationAttributes.Merge(other.RenderedOperationAttributes);
			RenderedParameterAttributes.Merge(other.RenderedParameterAttributes);

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
		bool IApiConfiguration.IsRendered(Rdata data) { return DataIsRendered.Get(data); }
		bool IApiConfiguration.IsRendered(Roperation operation) { return OperationIsRendered.Get(operation); }

		List<int> IApiConfiguration.GetModes(TypeCodeModel typeCodeModel) { return Modes.Get(typeCodeModel); }
		string IApiConfiguration.GetNamespace(TypeCodeModel typeCodeModel, int mode) { return RenderedTypeNamespace.Get(typeCodeModel.WithMode(mode)); }
		ITypeConversionTemplate IApiConfiguration.GetRenderedTypeTemplate(TypeCodeModel typeCodeModel, int mode) { return RenderedTypeTemplate.Get(typeCodeModel.WithMode(mode)); }

		string IApiConfiguration.GetName(TypeCodeModel typeCodeModel, int mode) { return RenderedTypeName.Get(typeCodeModel.WithMode(mode)); }
		string IApiConfiguration.GetName(DataCodeModel dataCodeModel, int mode) { return RenderedDataName.Get(dataCodeModel.WithMode(mode)); }
		string IApiConfiguration.GetName(OperationCodeModel operationCodeModel, int mode) { return RenderedOperationName.Get(operationCodeModel.WithMode(mode)); }
		string IApiConfiguration.GetName(ParameterCodeModel parameterCodeModel, int mode) { return RenderedParameterName.Get(parameterCodeModel.WithMode(mode)); }

		List<Type> IApiConfiguration.GetAttributes(TypeCodeModel typeCodeModel, int mode) { return RenderedTypeAttributes.Get(typeCodeModel.WithMode(mode)); }
		List<Type> IApiConfiguration.GetAttributes(InitializerCodeModel initializerCodeModel, int mode) { return RenderedInitializerAttributes.Get(initializerCodeModel.WithMode(mode)); }
		List<Type> IApiConfiguration.GetAttributes(DataCodeModel dataCodeModel, int mode) { return RenderedDataAttributes.Get(dataCodeModel.WithMode(mode)); }
		List<Type> IApiConfiguration.GetAttributes(OperationCodeModel operationCodeModel, int mode) { return RenderedOperationAttributes.Get(operationCodeModel.WithMode(mode)); }
		List<Type> IApiConfiguration.GetAttributes(ParameterCodeModel parameterCodeModel, int mode) { return RenderedParameterAttributes.Get(parameterCodeModel.WithMode(mode)); }

		Type IApiConfiguration.GetReferencedType(Rtype type) { return ReferencedType.Get(type); }
		ITypeConversionTemplate IApiConfiguration.GetReferencedTypeTemplate(Type type) { return ReferencedTypeTemplate.Get(type); }

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
