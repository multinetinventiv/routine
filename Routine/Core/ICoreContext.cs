using Routine.Core.Service;
using System.Collections;

namespace Routine.Core
{
	public interface ICoreContext
	{
		ICodingStyle CodingStyle{get;}
		DomainType GetDomainType(string objectModelId);

		DomainMember CreateDomainMember(DomainType domainType, IMember member);
		DomainOperation CreateDomainOperation(DomainType domainType, IOperation operation);
		DomainParameter CreateDomainParameter(DomainOperation domainOperation, IParameter parameter);

		DomainObject GetDomainObject(ObjectReferenceData objectReference);
	}

	public static class ICoreContextFacade
	{
		public static DomainObject GetDomainObject(this ICoreContext source, string id, string modelId)
		{
			return source.GetDomainObject(id, modelId, modelId);
		}

		public static DomainObject GetDomainObject(this ICoreContext source, string id, string actualModelId, string viewModelId)
		{
			return source.GetDomainObject(new ObjectReferenceData {
				Id = id,
				ActualModelId = actualModelId,
				ViewModelId = viewModelId
			});
		}

		public static object Locate(this ICoreContext source, ObjectReferenceData aReference)
		{
			if(aReference.IsNull)
			{
				aReference.ActualModelId = source.CodingStyle.ModelIdSerializer.Serialize(null);
				aReference.Id = source.CodingStyle.IdExtractor.Extract(null);
			}

			return source.CodingStyle.Locator.Locate(source.CodingStyle.ModelIdSerializer.Deserialize(aReference.ActualModelId), aReference.Id);
		}

		public static ValueData CreateValueData(this ICoreContext source, object anObject, bool isList, string viewModelId)
		{			
			var result = new ValueData();
			result.IsList = isList;
			if(isList)
			{
				var list = anObject as ICollection;
				foreach(var item in list)
				{
					result.Values.Add(source.CreateSingleValueData(item, viewModelId));
				}
			}
			else
			{
				result.Values.Add(source.CreateSingleValueData(anObject, viewModelId));
			}
			return result;
		}

		public static SingleValueData CreateSingleValueData(this ICoreContext source, object anObject, string viewModelId)
		{
			var result = new SingleValueData();

			var type = (anObject == null) ?null:anObject.GetTypeInfo();
			var actualModelId = source.CodingStyle.ModelIdSerializer.Serialize(type);
			var resultDomainType = source.GetDomainType(viewModelId);

			result.Reference.IsNull = anObject == null;
			result.Reference.ActualModelId = actualModelId;
			result.Reference.ViewModelId = viewModelId;
			result.Reference.Id = source.CodingStyle.IdExtractor.Extract(anObject);
			result.Value = source.GetValue(anObject, resultDomainType, result.Reference.Id);

			return result;
		}

		public static string GetValue(this ICoreContext source, object anObject, DomainType itsDomainType, string itsReferenceId)
		{
			if(itsDomainType.IsValueModel)
			{
				return itsReferenceId;
			}

			return source.CodingStyle.DisplayValueExtractor.Extract(anObject);
		}
	}
}

