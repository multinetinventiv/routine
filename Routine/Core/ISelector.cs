using System;
using System.Collections.Generic;

namespace Routine.Core
{
	public interface ISelector<TFrom, TItem>
	{
		List<TItem> Select(TFrom obj);
	}

	public interface IOptionalSelector<TFrom, TItem> : ISelector<TFrom, TItem>
	{
		bool CanSelect(TFrom obj);
		bool TrySelect(TFrom obj, out List<TItem> result);
	}

	public class CannotSelectException : Exception
	{
		public CannotSelectException(object obj) : base("Cannot select from '" + obj + "'") {}
	}
}

