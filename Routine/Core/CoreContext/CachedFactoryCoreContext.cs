using Routine.Core.Service;

namespace Routine.Core.CoreContext
{
	public class CachedFactoryCoreContext : ICoreContext
	{
		private readonly IFactory factory;
		private readonly ICodingStyle codingStyle;
		private readonly ICache cache;

		public CachedFactoryCoreContext(IFactory factory, ICodingStyle codingStyle, ICache cache)
		{
			this.factory = factory;
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
						cache.Add(objectModelId, factory.Create<DomainType>().For(objectModelId));
					}
				}
			}

			return cache[objectModelId] as DomainType;
		}

		public DomainMember CreateDomainMember(DomainType domainType, IMember member)
		{
			return factory.Create<DomainMember>().For(domainType, member);
		}

		public DomainOperation CreateDomainOperation(DomainType domainType, IOperation operation)
		{
			return factory.Create<DomainOperation>().For(domainType, operation);
		}

		public DomainParameter CreateDomainParameter(DomainOperation domainOperation, IParameter parameter)
		{
			return factory.Create<DomainParameter>().For(domainOperation, parameter);
		}

		public DomainObject GetDomainObject(ObjectReferenceData reference)
		{
			return factory.Create<DomainObject>().For(reference);
		}
	}
}
