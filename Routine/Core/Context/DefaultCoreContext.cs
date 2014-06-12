namespace Routine.Core.Context
{
	public class DefaultCoreContext : ICoreContext
	{
		private readonly ICodingStyle codingStyle;
		private readonly ICache cache;

		public DefaultCoreContext(ICodingStyle codingStyle, ICache cache)
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

		public DomainObjectInitializer CreateDomainObjectInitializer(DomainType domainType, IInitializer initializer)
		{
			return new DomainObjectInitializer(this).For(domainType, initializer);
		}

		public DomainMember CreateDomainMember(DomainType domainType, IMember member)
		{
			return new DomainMember(this).For(domainType, member);
		}

		public DomainOperation CreateDomainOperation(DomainType domainType, IOperation operation)
		{
			return new DomainOperation(this).For(domainType, operation);
		}

		public DomainParameter CreateDomainParameter(IParameter parameter, int initialGroupIndex)
		{
			return new DomainParameter(this).For(parameter, initialGroupIndex);
		}

		public DomainObject CreateDomainObject(object @object, string viewModelId)
		{
			return new DomainObject(this).For(@object, viewModelId);
		}

		public DomainObject GetDomainObject(ObjectReferenceData reference)
		{
			return new DomainObject(this).For(reference);
		}
	}
}
