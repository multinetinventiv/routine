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
		public GenericCodingStyle DomainTypeRootNamespacesAre(params string[] rootNamespaces) { TypeInfo.AddDomainTypeRootNamespace(rootNamespaces); return this; }
		public GenericCodingStyle RecognizeProxyTypesBy(Func<Type, bool> proxyMatcher, Func<Type, Type> actualTypeGetter) { TypeInfo.SetProxyMatcher(proxyMatcher, actualTypeGetter); return this; }

		public MultipleSerializer<GenericCodingStyle, TypeInfo> SerializeModelId { get; private set;}

		public MultipleSelector<GenericCodingStyle, TypeInfo, string> SelectModelMarks { get; private set; }
		public MultipleExtractor<GenericCodingStyle, TypeInfo, string> ExtractModelModule { get; private set; }
		public MultipleExtractor<GenericCodingStyle, TypeInfo, bool> ExtractModelIsValue { get; private set; }
		public MultipleExtractor<GenericCodingStyle, TypeInfo, bool> ExtractModelIsView { get; private set; }

		public MultipleSelector<GenericCodingStyle, TypeInfo, IInitializer> SelectInitializers { get; private set; }
		public MultipleSelector<GenericCodingStyle, IInitializer, string> SelectInitializerMarks { get; private set; }

		public MultipleSelector<GenericCodingStyle, TypeInfo, IMember> SelectMembers { get; private set; }
		public MultipleSelector<GenericCodingStyle, IMember, string> SelectMemberMarks { get; private set; }

		public MultipleSelector<GenericCodingStyle, TypeInfo, IOperation> SelectOperations { get; private set; }
		public MultipleSelector<GenericCodingStyle, IOperation, string> SelectOperationMarks { get; private set; }

		public MultipleSelector<GenericCodingStyle, IParameter, string> SelectParameterMarks { get; private set; }

		public MultipleExtractor<GenericCodingStyle, TypeInfo, List<string>> ExtractAvailableIds { get; private set; }
		public MultipleExtractor<GenericCodingStyle, IMember, bool> ExtractMemberFetchedEagerly { get; private set; }

		public MultipleExtractor<GenericCodingStyle, object, string> ExtractId { get; private set; }
		public MultipleLocator<GenericCodingStyle> Locate { get; private set; }

		public MultipleExtractor<GenericCodingStyle, object, string> ExtractValue { get; private set; }

		public GenericCodingStyle()
		{
			SerializeModelId = new MultipleSerializer<GenericCodingStyle, TypeInfo>(this);

			SelectModelMarks = new MultipleSelector<GenericCodingStyle, TypeInfo, string>(this);
			ExtractModelModule = new MultipleExtractor<GenericCodingStyle, TypeInfo, string>(this, "ModelModule");
			ExtractModelIsValue = new MultipleExtractor<GenericCodingStyle, TypeInfo, bool>(this, "ModelIsValue");
			ExtractModelIsView = new MultipleExtractor<GenericCodingStyle, TypeInfo, bool>(this, "ModelIsView");

			SelectInitializers = new MultipleSelector<GenericCodingStyle, TypeInfo, IInitializer>(this);
			SelectInitializerMarks = new MultipleSelector<GenericCodingStyle, IInitializer, string>(this);

			SelectMembers = new MultipleSelector<GenericCodingStyle, TypeInfo, IMember>(this);
			SelectMemberMarks = new MultipleSelector<GenericCodingStyle, IMember, string>(this);

			SelectOperations = new MultipleSelector<GenericCodingStyle, TypeInfo, IOperation>(this);
			SelectOperationMarks = new MultipleSelector<GenericCodingStyle, IOperation, string>(this);

			SelectParameterMarks = new MultipleSelector<GenericCodingStyle, IParameter, string>(this);

			ExtractAvailableIds = new MultipleExtractor<GenericCodingStyle, TypeInfo, List<string>>(this, "AvailableIds");
			ExtractMemberFetchedEagerly = new MultipleExtractor<GenericCodingStyle, IMember, bool>(this, "MemberFetchedEagerly");

			ExtractId = new MultipleExtractor<GenericCodingStyle, object, string>(this, "Id");
			Locate = new MultipleLocator<GenericCodingStyle>(this);

			ExtractValue = new MultipleExtractor<GenericCodingStyle, object, string>(this, "Value");
		}

		public GenericCodingStyle Merge(GenericCodingStyle other)
		{
			SerializeModelId.Merge(other.SerializeModelId);

			SelectModelMarks.Merge(other.SelectModelMarks);
			ExtractModelModule.Merge(other.ExtractModelModule);
			ExtractModelIsValue.Merge(other.ExtractModelIsValue);
			ExtractModelIsView.Merge(other.ExtractModelIsView);

			SelectInitializers.Merge(other.SelectInitializers);
			SelectInitializerMarks.Merge(other.SelectInitializerMarks);

			SelectMembers.Merge(other.SelectMembers);
			SelectMemberMarks.Merge(other.SelectMemberMarks);
			
			SelectOperations.Merge(other.SelectOperations);
			SelectOperationMarks.Merge(other.SelectOperationMarks);

			SelectParameterMarks.Merge(other.SelectParameterMarks);

			ExtractAvailableIds.Merge(other.ExtractAvailableIds);
			ExtractMemberFetchedEagerly.Merge(other.ExtractMemberFetchedEagerly);

			ExtractId.Merge(other.ExtractId);
			Locate.Merge(other.Locate);

			ExtractValue.Merge(other.ExtractValue);

			return this;
		}

		#region ICodingStyle implementation

		ISerializer<TypeInfo> ICodingStyle.ModelIdSerializer { get { return SerializeModelId; } }

		ISelector<TypeInfo, string> ICodingStyle.ModelMarkSelector { get { return SelectModelMarks; } }
		IExtractor<TypeInfo, string> ICodingStyle.ModelModuleExtractor { get { return ExtractModelModule; } }
		IExtractor<TypeInfo, bool> ICodingStyle.ModelIsValueExtractor { get { return ExtractModelIsValue; } }
		IExtractor<TypeInfo, bool> ICodingStyle.ModelIsViewExtractor { get { return ExtractModelIsView; } }

		ISelector<TypeInfo, IInitializer> ICodingStyle.InitializerSelector { get { return SelectInitializers; } }
		ISelector<IInitializer, string> ICodingStyle.InitializerMarkSelector { get { return SelectInitializerMarks; } }

		ISelector<TypeInfo, IMember> ICodingStyle.MemberSelector { get { return SelectMembers; } }
		ISelector<IMember, string> ICodingStyle.MemberMarkSelector { get { return SelectMemberMarks; } }

		ISelector<TypeInfo, IOperation> ICodingStyle.OperationSelector { get { return SelectOperations; } }
		ISelector<IOperation, string> ICodingStyle.OperationMarkSelector { get { return SelectOperationMarks; } }

		ISelector<IParameter, string> ICodingStyle.ParameterMarkSelector { get { return SelectParameterMarks; } }

		IExtractor<TypeInfo, List<string>> ICodingStyle.AvailableIdsExtractor { get { return ExtractAvailableIds; } }
		IExtractor<IMember, bool> ICodingStyle.MemberFetchedEagerlyExtractor { get { return ExtractMemberFetchedEagerly; } }

		IExtractor<object, string> ICodingStyle.IdExtractor { get { return ExtractId; } }
		ILocator ICodingStyle.Locator { get { return Locate; } }

		IExtractor<object, string> ICodingStyle.ValueExtractor { get { return ExtractValue; } }

		#endregion
	}
}

