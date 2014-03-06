using System;

namespace Routine
{
	//TODO will be removed, no IoC container
	public interface IFactory
	{
		T Create<T>();
		object Create(Type type);
	}
}

