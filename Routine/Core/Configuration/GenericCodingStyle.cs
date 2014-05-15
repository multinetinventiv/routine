using System;
using System.Collections.Generic;
using Routine.Core.Extractor;
using Routine.Core.Locator;
using Routine.Core.Selector;
using Routine.Core.Serializer;

namespace Routine.Core.Configuration
{
	public class GenericCodingStyle : ICodingStyle
	{
		public const string VOID_MODEL_ID = "_void";

		public GenericCodingStyle DomainTypeRootNamespacesAre(params string[] rootNamespaces) { TypeInfo.AddDomainTypeRootNamespace(rootNamespaces); return this; }
		public GenericCodingStyle RecognizeProxyTypesBy(Func<Type, bool> proxyMatcher, Func<Type, Type> actualTypeGetter) { TypeInfo.SetProxyMatcher(proxyMatcher, actualTypeGetter); return this; }

		public MultipleSerializer<GenericCodingStyle, TypeInfo> SerializeModelId { get; private set;}

		public MultipleSelector<GenericCodingStyle, TypeInfo, string> SelectModelMarks { get; private set; }
		public MultipleExtractor<GenericCodingStyle, TypeInfo, string> ExtractModelModule{ get; private set;}
		public MultipleExtractor<GenericCodingStyle, TypeInfo, bool> ExtractModelIsValue{ get; private set;}
		public MultipleExtractor<GenericCodingStyle, TypeInfo, bool> ExtractModelIsView{ get; private set;}

		public MultipleSelector<GenericCodingStyle, TypeInfo, IMember> SelectMembers { get; private set; }
		public MultipleSelector<GenericCodingStyle, IMember, string> SelectMemberMarks { get; private set; }
		public MultipleExtractor<GenericCodingStyle, IMember, bool> ExtractMemberIsHeavy { get; private set; }

		public MultipleSelector<GenericCodingStyle, TypeInfo, IOperation> SelectOperations { get; private set; }
		public MultipleSelector<GenericCodingStyle, IOperation, string> SelectOperationMarks { get; private set; }
		public MultipleExtractor<GenericCodingStyle, IOperation, bool> ExtractOperationIsHeavy { get; private set; }
		public MultipleSelector<GenericCodingStyle, IParameter, string> SelectParameterMarks { get; private set; }

		public MultipleExtractor<GenericCodingStyle, TypeInfo, List<string>> ExtractAvailableIds { get; private set;}

		public MultipleExtractor<GenericCodingStyle, object, string> ExtractId { get; private set; }
		public MultipleLocator<GenericCodingStyle> Locate { get; private set; }

		public MultipleExtractor<GenericCodingStyle, object, string> ExtractDisplayValue { get; private set; }

		public GenericCodingStyle()
		{
			SerializeModelId = new MultipleSerializer<GenericCodingStyle, TypeInfo>(this);

			SelectModelMarks = new MultipleSelector<GenericCodingStyle, TypeInfo, string>(this);
			ExtractModelModule = new MultipleExtractor<GenericCodingStyle, TypeInfo, string>(this, "ModelModule");
			ExtractModelIsValue = new MultipleExtractor<GenericCodingStyle, TypeInfo, bool>(this, "ModelIsValue");
			ExtractModelIsView = new MultipleExtractor<GenericCodingStyle, TypeInfo, bool>(this, "ModelIsView");

			SelectMembers = new MultipleSelector<GenericCodingStyle, TypeInfo, IMember>(this);
			SelectMemberMarks = new MultipleSelector<GenericCodingStyle, IMember, string>(this);
			ExtractMemberIsHeavy = new MultipleExtractor<GenericCodingStyle, IMember, bool>(this, "MemberIsHeavy");

			SelectOperations = new MultipleSelector<GenericCodingStyle, TypeInfo, IOperation>(this);
			SelectOperationMarks = new MultipleSelector<GenericCodingStyle, IOperation, string>(this);
			ExtractOperationIsHeavy = new MultipleExtractor<GenericCodingStyle, IOperation, bool>(this, "OperationIsHeavy");
			SelectParameterMarks = new MultipleSelector<GenericCodingStyle, IParameter, string>(this);

			ExtractAvailableIds = new MultipleExtractor<GenericCodingStyle, TypeInfo, List<string>>(this, "AvailableIds");

			ExtractId = new MultipleExtractor<GenericCodingStyle, object, string>(this, "Id");
			Locate = new MultipleLocator<GenericCodingStyle>(this);

			ExtractDisplayValue = new MultipleExtractor<GenericCodingStyle, object, string>(this, "DisplayValue");
		}

		public GenericCodingStyle Merge(GenericCodingStyle other)
		{
			SerializeModelId.Merge(other.SerializeModelId);

			SelectModelMarks.Merge(other.SelectModelMarks);
			ExtractModelModule.Merge(other.ExtractModelModule);
			ExtractModelIsValue.Merge(other.ExtractModelIsValue);
			ExtractModelIsView.Merge(other.ExtractModelIsView);

			SelectMembers.Merge(other.SelectMembers);
			SelectMemberMarks.Merge(other.SelectMemberMarks);
			ExtractMemberIsHeavy.Merge(other.ExtractMemberIsHeavy);
			
			SelectOperations.Merge(other.SelectOperations);
			SelectOperationMarks.Merge(other.SelectOperationMarks);
			ExtractOperationIsHeavy.Merge(other.ExtractOperationIsHeavy);
			SelectParameterMarks.Merge(other.SelectParameterMarks);

			ExtractAvailableIds.Merge(other.ExtractAvailableIds);

			ExtractId.Merge(other.ExtractId);
			Locate.Merge(other.Locate);

			ExtractDisplayValue.Merge(other.ExtractDisplayValue);

			return this;
		}

		#region ICodingStyle implementation
		ISerializer<TypeInfo> ICodingStyle.ModelIdSerializer { get { return SerializeModelId; } }

		ISelector<TypeInfo, string> ICodingStyle.ModelMarkSelector { get { return SelectModelMarks; } }
		IExtractor<TypeInfo, string> ICodingStyle.ModelModuleExtractor { get { return ExtractModelModule; } }
		IExtractor<TypeInfo, bool> ICodingStyle.ModelIsValueExtractor { get { return ExtractModelIsValue; } }
		IExtractor<TypeInfo, bool> ICodingStyle.ModelIsViewExtractor { get { return ExtractModelIsView; } }

		ISelector<TypeInfo, IMember> ICodingStyle.MemberSelector { get { return SelectMembers; } }
		ISelector<IMember, string> ICodingStyle.MemberMarkSelector { get { return SelectMemberMarks; } }
		IExtractor<IMember, bool> ICodingStyle.MemberIsHeavyExtractor { get { return ExtractMemberIsHeavy; } }

		ISelector<TypeInfo, IOperation> ICodingStyle.OperationSelector { get { return SelectOperations; } }
		ISelector<IOperation, string> ICodingStyle.OperationMarkSelector { get { return SelectOperationMarks; } }
		IExtractor<IOperation, bool> ICodingStyle.OperationIsHeavyExtractor { get { return ExtractOperationIsHeavy; } }
		ISelector<IParameter, string> ICodingStyle.ParameterMarkSelector { get { return SelectParameterMarks; } }

		IExtractor<TypeInfo, List<string>> ICodingStyle.AvailableIdsExtractor {get{return ExtractAvailableIds;}}

		IExtractor<object, string> ICodingStyle.IdExtractor { get { return ExtractId; } }
		ILocator ICodingStyle.Locator { get { return Locate; } }

		IExtractor<object, string> ICodingStyle.DisplayValueExtractor { get { return ExtractDisplayValue; } }
		#endregion
	}
}

