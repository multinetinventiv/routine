using System;

namespace Routine.Test.Common.Domain
{
	public interface IRepository<TObject>
	{
		TKey Insert<TKey>(TObject obj);
		void Update(TObject obj);
		void Delete(TObject obj);
	}
}

