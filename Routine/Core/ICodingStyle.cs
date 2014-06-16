using System;
using System.Collections.Generic;

namespace Routine.Core
{
	public interface ICodingStyle
	{
		ISerializer<TypeInfo> ModelIdSerializer { get; }

		ISelector<TypeInfo, string> ModelMarkSelector { get; }
		IExtractor<TypeInfo, string> ModelModuleExtractor { get; }
		IExtractor<TypeInfo, bool> ModelIsValueExtractor { get; }
		IExtractor<TypeInfo, bool> ModelIsViewExtractor { get; }

		ISelector<TypeInfo, IInitializer> InitializerSelector { get; }
		ISelector<IInitializer, string> InitializerMarkSelector { get; }

		ISelector<TypeInfo, IMember> MemberSelector { get; }
		ISelector<IMember, string> MemberMarkSelector { get; }

		ISelector<TypeInfo, IOperation> OperationSelector { get; }
		ISelector<IOperation, string> OperationMarkSelector { get; }
		ISelector<IParameter, string> ParameterMarkSelector { get; }

		IExtractor<TypeInfo, List<string>> AvailableIdsExtractor { get; }
		IExtractor<IMember, bool> MemberFetchedEagerlyExtractor { get; }

		IExtractor<object, string> IdExtractor { get; }
		IExtractor<object, string> ValueExtractor { get; }

		ILocator Locator { get; }
	}
}
