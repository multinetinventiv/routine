using System;
using System.Collections.Generic;
using System.Linq;

namespace Routine.Test.Domain
{
	public interface ILookup<TObject>
	{
		TObject Get<TKey>(TKey key);
		List<TObject> All();
		IQueryable<TObject> List();
	}
}

