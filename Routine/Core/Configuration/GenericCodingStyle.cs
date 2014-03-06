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

		public GenericCodingStyle DomainTypeRootNamespacesAre(params string[] rootNamespaces){TypeInfo.AddDomainTypeRootNamespace(rootNamespaces);return this;}

		public MultipleSerializer<GenericCodingStyle, TypeInfo> ModelId { get; private set;}

		public MultipleExtractor<GenericCodingStyle, TypeInfo, string> Module{ get; private set;}
		public MultipleExtractor<GenericCodingStyle, TypeInfo, bool> ModelIsValue{ get; private set;}
		public MultipleExtractor<GenericCodingStyle, TypeInfo, bool> ModelIsView{ get; private set;}

		public MultipleSelector<GenericCodingStyle, TypeInfo, IMember> Member { get; private set; }
		public MultipleSelector<GenericCodingStyle, TypeInfo, IOperation> Operation { get; private set; }

		public MultipleExtractor<GenericCodingStyle, TypeInfo, List<string>> AvailableIds { get; private set;}

		public MultipleExtractor<GenericCodingStyle, object, string> Id { get; private set; }

		public MultipleExtractor<GenericCodingStyle, IMember, bool> MemberIsHeavy {get; private set;}
		public MultipleExtractor<GenericCodingStyle, IOperation, bool> OperationIsHeavy {get; private set;}

		public MultipleLocator<GenericCodingStyle> Locator { get; private set; }

		public MultipleExtractor<GenericCodingStyle, Tuple<object, IOperation>, bool> OperationIsAvailable { get; private set; }
		public MultipleExtractor<GenericCodingStyle, object, string> DisplayValue { get; private set; }

		public GenericCodingStyle()
		{
			ModelId = new MultipleSerializer<GenericCodingStyle, TypeInfo>(this);

			Module = new MultipleExtractor<GenericCodingStyle, TypeInfo, string>(this, "Module");
			ModelIsValue = new MultipleExtractor<GenericCodingStyle, TypeInfo, bool>(this, "ModelIsValue");
			ModelIsView = new MultipleExtractor<GenericCodingStyle, TypeInfo, bool>(this, "ModelIsView");

			Member = new MultipleSelector<GenericCodingStyle, TypeInfo, IMember>(this);
			Operation = new MultipleSelector<GenericCodingStyle, TypeInfo, IOperation>(this);

			AvailableIds = new MultipleExtractor<GenericCodingStyle, TypeInfo, List<string>>(this, "AvailableIds");

			Id = new MultipleExtractor<GenericCodingStyle, object, string>(this, "Id");

			MemberIsHeavy = new MultipleExtractor<GenericCodingStyle, IMember, bool>(this, "MemberIsHeavy");
			OperationIsHeavy = new MultipleExtractor<GenericCodingStyle, IOperation, bool>(this, "OperationIsHeavy");

			Locator = new MultipleLocator<GenericCodingStyle>(this);

			OperationIsAvailable = new MultipleExtractor<GenericCodingStyle, Tuple<object, IOperation>, bool>(this, "OperationIsAvailable");
			DisplayValue = new MultipleExtractor<GenericCodingStyle, object, string>(this, "DisplayValue");
		}

		public GenericCodingStyle Merge(GenericCodingStyle other)
		{
			ModelId.Merge(other.ModelId);

			Module.Merge(other.Module);
			ModelIsValue.Merge(other.ModelIsValue);
			ModelIsView.Merge(other.ModelIsView);

			Member.Merge(other.Member);
			Operation.Merge(other.Operation);

			AvailableIds.Merge(other.AvailableIds);

			Id.Merge(other.Id);

			MemberIsHeavy.Merge(other.MemberIsHeavy);
			OperationIsHeavy.Merge(other.OperationIsHeavy);

			Locator.Merge(other.Locator);

			OperationIsAvailable.Merge(other.OperationIsAvailable);
			DisplayValue.Merge(other.DisplayValue);

			return this;
		}

		#region ICodingStyle implementation
		ISerializer<TypeInfo> ICodingStyle.ModelIdSerializer {get{return ModelId;}}

		IExtractor<TypeInfo, string> ICodingStyle.ModuleExtractor{get{return Module;}}
		IExtractor<TypeInfo, bool> ICodingStyle.ModelIsValueExtractor{get{return ModelIsValue;}}
		IExtractor<TypeInfo, bool> ICodingStyle.ModelIsViewExtractor{get{return ModelIsView;}}

		ISelector<TypeInfo, IMember> ICodingStyle.MemberSelector { get { return Member; } }
		ISelector<TypeInfo, IOperation> ICodingStyle.OperationSelector { get { return Operation; } }

		IExtractor<TypeInfo, List<string>> ICodingStyle.AvailableIdsExtractor {get{return AvailableIds;}}

		IExtractor<object, string> ICodingStyle.IdExtractor { get { return Id; } }

		IExtractor<IMember, bool> ICodingStyle.MemberIsHeavyExtractor {get{return MemberIsHeavy;}}
		IExtractor<IOperation, bool> ICodingStyle.OperationIsHeavyExtractor {get{return OperationIsHeavy;}}

		ILocator ICodingStyle.Locator { get { return Locator; } }

		IExtractor<Tuple<object, IOperation>, bool> ICodingStyle.OperationIsAvailableExtractor { get { return OperationIsAvailable; } }
		IExtractor<object, string> ICodingStyle.DisplayValueExtractor { get { return DisplayValue; } }
		#endregion
	}
}

