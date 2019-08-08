using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;

namespace Routine.Test.Domain.NHibernate
{
	internal class NHibernateLookup<T> : ILookup<T>
	{
		private readonly ISession session;

		public NHibernateLookup(ISession session)
		{
			this.session = session;
		}

		public T Get<TKey>(TKey key)
		{
			if(object.Equals(key, default(TKey)))
			{
				return default(T);
			}

			return session.Get<T>(key);
		}

		public List<T> All()
		{
			return List().ToList();
		}

		public IQueryable<T> List()
		{
			return session.Query<T>();
		}
	}
}

