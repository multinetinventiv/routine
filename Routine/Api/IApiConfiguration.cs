using System;
using System.Collections.Generic;
using Routine.Client;
using Routine.Engine;

namespace Routine.Api
{
	public interface IApiConfiguration
	{
		string GetDefaultNamespace();
		bool GetInMemory();
		string GetOutputFileName();
		Version GetVersion(ApplicationCodeModel application);

		List<string> GetFriendlyAssemblyNames();
		
		bool IsRendered(Rtype type);
		bool IsRendered(Rinitializer initializer);
		bool IsRendered(Rmember member);
		bool IsRendered(Roperation operation);

		List<int> GetModes(TypeCodeModel typeCodeModel);
		string GetNamespace(TypeCodeModel typeCodeModel, int mode);
		ITypeConversionTemplate GetRenderedTypeTemplate(TypeCodeModel typeCodeModel, int mode);

		string GetName(TypeCodeModel typeCodeModel, int mode);
		string GetName(OperationCodeModel operationCodeModel, int mode);
		string GetName(MemberCodeModel memberCodeModel, int mode);
		string GetName(ParameterCodeModel parameterCodeModel, int mode);
		
		IType GetReferencedType(Rtype type);
		ITypeConversionTemplate GetReferencedTypeTemplate(IType type);
	}
}
