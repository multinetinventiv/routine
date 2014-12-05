using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core;
using Routine.Core.Cache;

namespace Routine.Engine.Context
{
	public class DefaultCoreContext : ICoreContext
	{
		private readonly ICodingStyle codingStyle;
		private readonly ICache cache;

		private Dictionary<string, DomainType> DomainTypes
		{
			get { return cache[Constants.DOMAIN_TYPES_CACHE_KEY] as Dictionary<string, DomainType>; }
			set
			{
				lock (cache)
				{
					cache.Add(Constants.DOMAIN_TYPES_CACHE_KEY, value);
				}
			}
		}

		public DefaultCoreContext(ICodingStyle codingStyle, ICache cache)
		{
			this.codingStyle = codingStyle;
			this.cache = cache;

			DomainTypes = new Dictionary<string, DomainType>();
		}

		public ICodingStyle CodingStyle { get { return codingStyle; } }

		public DomainType GetDomainType(string typeId)
		{
			DomainType result;
			if (!DomainTypes.TryGetValue(typeId, out result))
			{
				throw new TypeNotFoundException(typeId);
			}

			return result;
		}

		public DomainType GetDomainType(IType type)
		{
			var typeId = codingStyle.GetTypeId(type);

			DomainType result;
			if (!DomainTypes.TryGetValue(typeId, out result))
			{
				lock (DomainTypes)
				{
					if (!DomainTypes.ContainsKey(typeId))
					{
						result = new DomainType(this, type);

						DomainTypes.Add(typeId, result);
					}
					else
					{
						result = DomainTypes[typeId];
					}
				}
			}

			return result;
		}

		public List<DomainType> GetDomainTypes()
		{
			return DomainTypes.Values.ToList();
		}

		public DomainObject CreateDomainObject(object @object, DomainType viewDomainType)
		{
			var type = (@object == null) ? null : CodingStyle.GetType(@object);
			var actualDomainType = GetDomainType(type);

			viewDomainType = viewDomainType ?? actualDomainType;

			return new DomainObject(@object, actualDomainType, viewDomainType);
		}

		public DomainObject CreateDomainObject(ObjectReferenceData objectReferenceData)
		{
			var actualDomainType = GetActualDomainType(objectReferenceData);
			var viewDomainType = objectReferenceData.ActualModelId != objectReferenceData.ViewModelId
				? GetDomainType(objectReferenceData.ViewModelId)
				: actualDomainType;

			return new DomainObject(actualDomainType.Locate(objectReferenceData), actualDomainType, viewDomainType);
		}

		private DomainType GetActualDomainType(ObjectReferenceData aReference)
		{
			if (aReference.IsNull)
			{
				return GetDomainType((IType)null);
			}

			return GetDomainType(aReference.ActualModelId);
		}

		public object GetObject(ObjectReferenceData aReference)
		{
			return GetActualDomainType(aReference).Locate(aReference);
		}
	}

	public class TypeNotFoundException : Exception
	{
		public TypeNotFoundException(string typeId)
			: base(string.Format("Type could not be found with given type id: '{0}'. Make sure type id is correct and configured. " +
								 "Also make sure that ObjectService.GetApplicationModel is called before any other ObjectService methods are called." +
								 "(This is because domain type of the expected type should be accessed via IType before trying to access via type id.)", typeId)) { }
	}
}
