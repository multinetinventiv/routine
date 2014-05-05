using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Routine.Core;

namespace Routine.Api.Generator
{
	public interface IApiGenerationConfiguration
	{
		string ApiName { get; }
		string DefaultNamespace { get; }
		bool InMemory { get; }
		ModuleFilter Modules { get; }
		List<string> FriendlyAssemblyNames { get; }

		ISerializer<TypeInfo> ReferencedModelIdSerializer { get; }
		IExtractor<TypeInfo, bool> ReferencedTypeIsClientTypeExtractor { get; }
		IExtractor<TypeInfo, string> StringToValueCodeTemplateExtractor { get; }
		IExtractor<TypeInfo, string> ValueToStringCodeTemplateExtractor { get; }

		ISelector<ObjectCodeModel, string> SingletonIdSelector { get; }
	}
}
