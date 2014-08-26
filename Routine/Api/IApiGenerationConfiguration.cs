using System.Collections.Generic;
using Routine.Core;

namespace Routine.Api
{
	public interface IApiGenerationConfiguration
	{
		string ApiName { get; }
		string DefaultNamespace { get; }
		bool InMemory { get; }
		ModuleFilter Modules { get; }
		List<string> FriendlyAssemblyNames { get; }

		ISerializer<TypeInfo> ReferencedModelIdSerializer { get; }

		IExtractor<TypeInfo, bool> ReferencedTypeIsValueTypeExtractor { get; }
		
		IExtractor<TypeInfo, TypeInfo> TargetValueTypeExtractor { get; }
		
		IExtractor<TypeInfo, string> StringToValueCodeTemplateExtractor { get; }
		IExtractor<TypeInfo, string> ValueToStringCodeTemplateExtractor { get; }

		ISelector<ObjectCodeModel, string> SingletonIdSelector { get; }
	}
}
