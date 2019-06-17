using System;
using Routine.Test.Common;

namespace Routine.Test.Domain
{
	public interface IDomainContext
	{
		object Resolve(Type type);
		object Find(TypedGuid typedUid);
	}

	public static class DomainContextFacade
	{
		public static T New<T>(this IDomainContext source)
		{
			return (T)source.Resolve(typeof(T));
		}

		public static T Query<T>(this IDomainContext source) where T : IQuery
		{
			return (T)source.Resolve(typeof(T));
		}
	}
}

