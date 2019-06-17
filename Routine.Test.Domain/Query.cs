using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Routine.Test.Domain
{
	public interface IQuery { }
	public abstract class Query<T> : IQuery
	{
		protected IDomainContext Context{get;private set;}

		protected Query(IDomainContext context)
		{
			Context = context;
		}

		protected ILookup<T> Lookup{get{return (ILookup<T>)Context.Resolve(typeof(ILookup<T>));}}

		public virtual T ByUid(Guid key)
		{
			return Lookup.Get(key);
		}

		protected virtual List<T> All()
		{
			return Lookup.All();
		}

		protected virtual T SingleBy(Expression<Func<T, bool>> whereClause)
		{
			return Lookup.List().Where(whereClause).SingleOrDefault();
		}

		protected virtual List<T> By(Expression<Func<T, bool>> whereClause)
		{
			return Lookup.List().Where(whereClause).ToList();
		}
	}
}

