using System.Collections.Generic;
using Routine.Client;
using Routine.Engine;

namespace Routine.Api
{
	public interface IApiGenerationConfiguration
	{
		string GetApiName();
		string GetDefaultNamespace();
		bool GetInMemory();
		bool GetIgnoreReferencedTypeNotFound();

		List<string> GetFriendlyAssemblyNames();

		IType GetReferencedType(Rtype type);
		bool GetReferencedTypeIsValueType(IType type);
		IType GetTargetValueType(IType type);
		string GetStringToValueCodeTemplate(IType type);
		string GetValueToStringCodeTemplate(IType type);
		List<string> GetStaticInstanceIds(ObjectCodeModel objectCodeModel);

		bool IsRendered(Rtype type);
		bool IsRendered(Rinitializer initializer);
		bool IsRendered(Rmember member);
		bool IsRendered(Roperation operation);
	}
}
