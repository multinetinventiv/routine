using System;

namespace Routine
{
	public interface IFactory
	{
		T Create<T>();
		object Create(Type type);
	}
}

