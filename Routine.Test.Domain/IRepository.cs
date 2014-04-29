using System;

namespace Routine.Test.Domain
{
	public interface IRepository<TObject>
	{
		TKey Insert<TKey>(TObject obj);
		void Update(TObject obj);
		void Delete(TObject obj);
	}
}

