using System.Collections;
using System.Collections.Generic;
using Routine.Core;

namespace Routine.Engine
{
	public interface IDomainObjectFactory
	{
		DomainObject CreateDomainObject(DomainType type, string id);
	}

	public interface ICoreContext
	{
		ICodingStyle CodingStyle { get; }
		DomainType GetDomainType(string typeId);
		DomainType GetDomainType(IType type);
		List<DomainType> GetDomainTypes();

		object GetObject(ReferenceData aReference);

		DomainObject CreateDomainObject(object @object, DomainType viewDomainType);
		DomainObject CreateDomainObject(ReferenceData reference);
	}

	public static class CoreContextFacade
	{
		public static DomainObject CreateDomainObject(this ICoreContext source, string id, string modelId)
		{
			return source.CreateDomainObject(id, modelId, modelId);
		}

		public static DomainObject CreateDomainObject(this ICoreContext source, string id, string modelId, string viewModelId)
		{
			return source.CreateDomainObject(new ReferenceData
			{
				Id = id,
				ModelId = modelId,
				ViewModelId = viewModelId
			});
		}

		public static DomainObject CreateDomainObject(this ICoreContext source, object anObject)
		{
			return source.CreateDomainObject(anObject, null);
		}

		internal static VariableData CreateValueData(this ICoreContext source, object anObject, bool isList, DomainType viewDomainType, bool eager) { return source.CreateValueData(anObject, isList, viewDomainType, Constants.FIRST_DEPTH, eager); }
		internal static VariableData CreateValueData(this ICoreContext source, object anObject, bool isList, DomainType viewDomainType, int currentDepth, bool eager)
		{
			var result = new VariableData { IsList = isList };

			if (anObject == null) { return result; }

			if (isList)
			{
				var list = anObject as ICollection;

				if (list == null) { return result; }

				foreach (var item in list)
				{
					result.Values.Add(source.CreateDomainObject(item, viewDomainType).GetObjectData(currentDepth, eager));
				}
			}
			else
			{
				result.Values.Add(source.CreateDomainObject(anObject, viewDomainType).GetObjectData(currentDepth, eager));
			}

			return result;
		}

		internal static string BuildTypeId(this ICoreContext source, string module, string name)
		{
			if (string.IsNullOrEmpty(module))
			{
				return name;
			}

			return $"{module}.{name}";
		}
	}
}

