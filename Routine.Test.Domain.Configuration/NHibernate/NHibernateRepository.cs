using NHibernate;

namespace Routine.Test.Domain.NHibernate
{
	internal class NHibernateRepository<T> : IRepository<T>
	{
		private readonly ISession session;

		public NHibernateRepository(ISession session)
		{
			this.session = session;
		}

		public TKey Insert<TKey>(T obj)
		{
			var result = (TKey)session.Save(obj);
			session.Flush();
			return result;
		}

		public void Update(T obj)
		{
			session.Update(obj);
			session.Flush();
		}

		public void Delete(T obj)
		{
			session.Delete(obj);
			session.Flush();
		}
	}
}

