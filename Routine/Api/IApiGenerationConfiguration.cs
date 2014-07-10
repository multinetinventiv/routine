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

		//TODO refactor name -> ReferencedTypeIsValueTypeExtractor 
		//TODO (so that we don't extract if it is a client type generated via another api generator for a domain type rather we extract if it is a value model or not)
		//TODO in short the behavior will be the opposite
		IExtractor<TypeInfo, bool> ReferencedTypeIsClientTypeExtractor { get; }
		
		//TODO refactor name -> ValueTypeRemainsAsStringExtractor
		IExtractor<TypeInfo, bool> ValueTypeIsNotConvertedExtractor { get; }
		
		IExtractor<TypeInfo, string> StringToValueCodeTemplateExtractor { get; }
		IExtractor<TypeInfo, string> ValueToStringCodeTemplateExtractor { get; }

		ISelector<ObjectCodeModel, string> SingletonIdSelector { get; }
	}
}
