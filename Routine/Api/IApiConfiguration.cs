using System;
using System.Collections.Generic;
using Routine.Client;

namespace Routine.Api
{
	public interface IApiConfiguration
	{
		string GetDefaultNamespace();
		bool GetInMemory();
		string GetOutputFileName();
		Version GetAssemblyVersion(ApplicationCodeModel application);
		Guid GetAssemblyGuid(ApplicationCodeModel application);

		List<string> GetFriendlyAssemblyNames();
		
		bool IsRendered(Rtype type);
		bool IsRendered(Rinitializer initializer);
		bool IsRendered(Rdata data);
		bool IsRendered(Roperation operation);

		List<int> GetModes(TypeCodeModel typeCodeModel);
		string GetNamespace(TypeCodeModel typeCodeModel, int mode);
		ITypeConversionTemplate GetRenderedTypeTemplate(TypeCodeModel typeCodeModel, int mode);

		string GetName(TypeCodeModel typeCodeModel, int mode);
		string GetName(OperationCodeModel operationCodeModel, int mode);
		string GetName(DataCodeModel dataCodeModel, int mode);
		string GetName(ParameterCodeModel parameterCodeModel, int mode);

		List<Type> GetAttributes(TypeCodeModel typeCodeModel, int mode);
		List<Type> GetAttributes(InitializerCodeModel parameterCodeModel, int mode);
		List<Type> GetAttributes(OperationCodeModel operationCodeModel, int mode);
		List<Type> GetAttributes(DataCodeModel dataCodeModel, int mode);
		List<Type> GetAttributes(ParameterCodeModel parameterCodeModel, int mode);

		Type GetReferencedType(Rtype type);
		ITypeConversionTemplate GetReferencedTypeTemplate(Type type);
	}
}
