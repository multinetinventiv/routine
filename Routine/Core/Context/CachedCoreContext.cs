using Routine.Core.Service;

namespace Routine.Core.Context
{
	public class CachedCoreContext : ICoreContext
	{
		private readonly ICodingStyle codingStyle;
		private readonly ICache cache;

		public CachedCoreContext(ICodingStyle codingStyle, ICache cache)
		{
			this.codingStyle = codingStyle;
			this.cache = cache;
		}

		public ICodingStyle CodingStyle{ get{return codingStyle;}}

		public DomainType GetDomainType(string objectModelId)
		{
			if(!cache.Contains(objectModelId))
			{
				lock(cache)
				{
					if(!cache.Contains(objectModelId))
					{
						cache.Add(objectModelId, new DomainType(this).For(objectModelId));
					}
				}
			}

			return cache[objectModelId] as DomainType;
		}

		public DomainMember CreateDomainMember(DomainType domainType, IMember member)
		{
			return new DomainMember(this).For(domainType, member);
		}

		public DomainOperation CreateDomainOperation(DomainType domainType, IOperation operation)
		{
			return new DomainOperation(this).For(domainType, operation);
		}

		public DomainParameter CreateDomainParameter(DomainOperation domainOperation, IParameter parameter)
		{
			return new DomainParameter(this).For(domainOperation, parameter);
		}

		public DomainObject GetDomainObject(ObjectReferenceData reference)
		{
			return new DomainObject(this).For(reference);
		}
	}
}
