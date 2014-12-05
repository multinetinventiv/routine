using System.Collections.Generic;

namespace Routine.Engine
{
	public interface ICodingStyle
	{
		List<IType> GetTypes();
		IType GetType(object @object);
		string GetTypeId(IType type);
		string GetModuleName(IType type);
		bool IsValue(IType type);
		bool IsView(IType type);
		IIdExtractor GetIdExtractor(IType type);
		IValueExtractor GetValueExtractor(IType type);
		ILocator GetObjectLocator(IType type);
		List<object> GetStaticInstances(IType type);
		List<IInitializer> GetInitializers(IType type);
		List<IMember> GetMembers(IType type);
		List<IOperation> GetOperations(IType type);

		bool IsFetchedEagerly(IMember member);

		List<string> GetMarks(IType type);
		List<string> GetMarks(IInitializer initializer);
		List<string> GetMarks(IMember member);
		List<string> GetMarks(IOperation operation);
		List<string> GetMarks(IParameter parameter);
	}
}
