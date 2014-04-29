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
		DomainObject CreateDomainObject(object @object, string viewModelId);

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

		internal static object Locate(this ICoreContext source, ObjectReferenceData aReference)
		{
			if (aReference.IsNull)
			{
				aReference.ActualModelId = source.CodingStyle.ModelIdSerializer.Serialize(null);
				aReference.Id = source.CodingStyle.IdExtractor.Extract(null);
			}

			return source.CodingStyle.Locator.Locate(source.CodingStyle.ModelIdSerializer.Deserialize(aReference.ActualModelId), aReference.Id);
		}

		internal static ValueData CreateValueData(this ICoreContext source, object anObject, bool isList, string viewModelId, bool eager)
		{			
			var result = new ValueData();
			result.IsList = isList;
			if(isList)
			{
				var list = anObject as ICollection;
				foreach(var item in list)
				{
					if (eager)
					{
						result.Values.Add(source.CreateDomainObject(item, viewModelId).GetObject());
					}
					else
					{
						result.Values.Add(source.CreateSingleValueData(item, viewModelId));
					}
				}
			}
			else
			{
				if (eager)
				{
					result.Values.Add(source.CreateDomainObject(anObject, viewModelId).GetObject());
				}
				else
				{
					result.Values.Add(source.CreateSingleValueData(anObject, viewModelId));
				}
			}
			return result;
		}

		internal static ObjectData CreateSingleValueData(this ICoreContext source, object anObject, string viewModelId)
		{
			var result = new ObjectData();

			var resultDomainType = source.GetDomainType(viewModelId);

			result.Reference = source.CreateReferenceData(anObject, viewModelId);
			result.Value = source.GetValue(anObject, resultDomainType, result.Reference.Id);

			return result;
		}

		internal static string GetValue(this ICoreContext source, object anObject, DomainType itsDomainType, string itsReferenceId)
		{
			if(itsDomainType.IsValueModel)
			{
				return itsReferenceId;
			}

			return source.CodingStyle.DisplayValueExtractor.Extract(anObject);
		}

		internal static ObjectReferenceData CreateReferenceData(this ICoreContext source, object anObject) { return source.CreateReferenceData(null); }
		internal static ObjectReferenceData CreateReferenceData(this ICoreContext source, object anObject, string viewModelId)
		{
			var result = new ObjectReferenceData();

			var type = (anObject == null) ? null : anObject.GetTypeInfo();
			var actualModelId = source.CodingStyle.ModelIdSerializer.Serialize(type);

			result.IsNull = anObject == null;
			result.ActualModelId = actualModelId;
			result.ViewModelId = viewModelId ?? actualModelId;
			result.Id = source.CodingStyle.IdExtractor.Extract(anObject);

			return result;
		}
	}
}

