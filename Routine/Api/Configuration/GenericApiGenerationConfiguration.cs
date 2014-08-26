using System.Collections.Generic;
using Routine.Core;
using Routine.Core.Extractor;
using Routine.Core.Selector;
using Routine.Core.Serializer;

namespace Routine.Api.Configuration
{
	public class GenericApiGenerationConfiguration : IApiGenerationConfiguration
	{
		private string ApiName { get; set; }
		public GenericApiGenerationConfiguration ApiNameIs(string apiName) { ApiName = apiName; return this; }

		private string DefaultNamespace { get; set; }
		public GenericApiGenerationConfiguration DefaultNamespaceIs(string defaultNamespace) { DefaultNamespace = defaultNamespace; return this; }

		private bool InMemory { get; set; }
		public GenericApiGenerationConfiguration GenerateInMemory() { return GenerateInMemory(true); }
		public GenericApiGenerationConfiguration GenerateInMemory(bool inMemory) { InMemory = inMemory; return this; }

		private List<string> FriendlyAssemblyNames { get; set; }
		public GenericApiGenerationConfiguration FriendlyAssemblyNamesAre(params string[] friendlyAssemblyNames) { FriendlyAssemblyNames.AddRange(friendlyAssemblyNames); return this; }

		private ModuleFilter Modules { get; set; }
		public GenericApiGenerationConfiguration IncludeModule(string includeFilter) { Modules.Include(includeFilter); return this; }
		public GenericApiGenerationConfiguration ExcludeModule(string excludeFilter) { Modules.Exclude(excludeFilter); return this; }

		public MultipleSerializer<GenericApiGenerationConfiguration, TypeInfo> SerializeReferencedModelId { get; private set; }

		public MultipleExtractor<GenericApiGenerationConfiguration, TypeInfo, bool> ExtractReferencedTypeIsValueType { get; private set; }
		public MultipleExtractor<GenericApiGenerationConfiguration, TypeInfo, TypeInfo> ExtractTargetValueType { get; private set; }

		public MultipleExtractor<GenericApiGenerationConfiguration, TypeInfo, string> ExtractStringToValueCodeTemplate { get; private set; }
		public MultipleExtractor<GenericApiGenerationConfiguration, TypeInfo, string> ExtractValueToStringCodeTemplate { get; private set; }

		public MultipleSelector<GenericApiGenerationConfiguration, ObjectCodeModel, string> SelectSingletonId { get; private set; }

		public GenericApiGenerationConfiguration()
		{
			FriendlyAssemblyNames = new List<string>();
			Modules = new ModuleFilter();

			SerializeReferencedModelId = new MultipleSerializer<GenericApiGenerationConfiguration, TypeInfo>(this);

			ExtractReferencedTypeIsValueType = new MultipleExtractor<GenericApiGenerationConfiguration,TypeInfo,bool>(this, "ReferencedTypeIsValueType");
			ExtractTargetValueType = new MultipleExtractor<GenericApiGenerationConfiguration, TypeInfo, TypeInfo>(this, "TargetValueType");

			ExtractStringToValueCodeTemplate = new MultipleExtractor<GenericApiGenerationConfiguration, TypeInfo, string>(this, "StringToValueCodeTemplate");
			ExtractValueToStringCodeTemplate = new MultipleExtractor<GenericApiGenerationConfiguration, TypeInfo, string>(this, "ValueToStringCodeTemplate");

			SelectSingletonId = new MultipleSelector<GenericApiGenerationConfiguration, ObjectCodeModel, string>(this);
		}

		public GenericApiGenerationConfiguration Merge(GenericApiGenerationConfiguration other)
		{
			FriendlyAssemblyNames.AddRange(other.FriendlyAssemblyNames);
			Modules.Merge(other.Modules);

			SerializeReferencedModelId.Merge(other.SerializeReferencedModelId);

			ExtractReferencedTypeIsValueType.Merge(other.ExtractReferencedTypeIsValueType);
			ExtractTargetValueType.Merge(other.ExtractTargetValueType);

			ExtractStringToValueCodeTemplate.Merge(other.ExtractStringToValueCodeTemplate);
			ExtractValueToStringCodeTemplate.Merge(other.ExtractValueToStringCodeTemplate);

			SelectSingletonId.Merge(other.SelectSingletonId);

			return this;
		}

		#region IApiGenerationConfiguration implementation

		string IApiGenerationConfiguration.ApiName { get { return ApiName; } }
		string IApiGenerationConfiguration.DefaultNamespace { get { return DefaultNamespace; } }
		bool IApiGenerationConfiguration.InMemory { get { return InMemory; } }
		ModuleFilter IApiGenerationConfiguration.Modules { get { return Modules; } }
		List<string> IApiGenerationConfiguration.FriendlyAssemblyNames { get { return FriendlyAssemblyNames; } }

		ISerializer<TypeInfo> IApiGenerationConfiguration.ReferencedModelIdSerializer { get { return SerializeReferencedModelId; } }

		IExtractor<TypeInfo, bool> IApiGenerationConfiguration.ReferencedTypeIsValueTypeExtractor { get { return ExtractReferencedTypeIsValueType; } }
		IExtractor<TypeInfo, TypeInfo> IApiGenerationConfiguration.TargetValueTypeExtractor { get { return ExtractTargetValueType; } }

		IExtractor<TypeInfo, string> IApiGenerationConfiguration.StringToValueCodeTemplateExtractor { get { return ExtractStringToValueCodeTemplate; } }
		IExtractor<TypeInfo, string> IApiGenerationConfiguration.ValueToStringCodeTemplateExtractor { get { return ExtractValueToStringCodeTemplate; } }

		ISelector<ObjectCodeModel, string> IApiGenerationConfiguration.SingletonIdSelector { get { return SelectSingletonId; } }

		#endregion
	}
}
