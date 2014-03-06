using System;

namespace Routine.Test.Common.Domain
{
	public interface IDomainContext
	{
		T New<T>();
		T Get<T>();

		object New(Type type);
		object Get(Type type);
	}
}

