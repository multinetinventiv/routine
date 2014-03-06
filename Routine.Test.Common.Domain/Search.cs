using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Routine.Test.Common.Domain
{
	public abstract class Search<T>
	{
		protected IDomainContext Context{get;private set;}

		protected Search(IDomainContext context)
		{
			Context = context;
		}

		protected ILookup<T> Lookup{get{return Context.Get<ILookup<T>>();}}

		public virtual T Get(Guid key)
		{
			return Lookup.Get(key);
		}

		public virtual List<T> All()
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

