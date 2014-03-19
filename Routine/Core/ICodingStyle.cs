using System;
using System.Collections.Generic;

namespace Routine.Core
{
	public interface ICodingStyle
	{
		ISerializer<TypeInfo> ModelIdSerializer {get;}

		ISelector<TypeInfo, string> ModelMarkSelector { get; }
		IExtractor<TypeInfo, string> ModelModuleExtractor{get;}
		IExtractor<TypeInfo, bool> ModelIsValueExtractor{get;}
		IExtractor<TypeInfo, bool> ModelIsViewExtractor{get;}

		ISelector<TypeInfo, IMember> MemberSelector { get; }
		ISelector<IMember, string> MemberMarkSelector { get; }
		IExtractor<IMember, bool> MemberIsHeavyExtractor { get; }

		ISelector<TypeInfo, IOperation> OperationSelector { get; }
		ISelector<IOperation, string> OperationMarkSelector { get; }
		IExtractor<IOperation, bool> OperationIsHeavyExtractor { get; }
		ISelector<IParameter, string> ParameterMarkSelector { get; }

		IExtractor<TypeInfo, List<string>> AvailableIdsExtractor{get;}

		IExtractor<object, string> IdExtractor { get; }
		ILocator Locator { get; }

		//TODO move to mvcconfig
		IExtractor<object, string> DisplayValueExtractor { get; }
		IExtractor<Tuple<object, IOperation>, bool> OperationIsAvailableExtractor { get; }
	}
}
