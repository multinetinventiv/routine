using System;
using System.Collections.Generic;

namespace Routine.Core
{
	public interface ICodingStyle
	{
		ISerializer<TypeInfo> ModelIdSerializer {get;}

		IExtractor<TypeInfo, string> ModuleExtractor{get;}
		IExtractor<TypeInfo, bool> ModelIsValueExtractor{get;}
		IExtractor<TypeInfo, bool> ModelIsViewExtractor{get;}

		ISelector<TypeInfo, IMember> MemberSelector { get; }
		ISelector<TypeInfo, IOperation> OperationSelector { get; }

		IExtractor<TypeInfo, List<string>> AvailableIdsExtractor{get;}

		IExtractor<object, string> IdExtractor { get; }

		IExtractor<IMember, bool> MemberIsHeavyExtractor {get;}
		IExtractor<IOperation, bool> OperationIsHeavyExtractor {get;}

		ILocator Locator { get; }

		//TODO move to mvcconfig
		IExtractor<Tuple<object, IOperation>, bool> OperationIsAvailableExtractor { get; }
		IExtractor<object, string> DisplayValueExtractor { get; }
	}
}
