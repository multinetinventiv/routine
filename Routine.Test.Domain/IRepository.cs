using System;

namespace Routine.Test.Domain
{
	public interface IRepository<TObject>
	{
		void Insert(TObject obj);
		void Update(TObject obj);
		void Delete(TObject obj);
	}
}

