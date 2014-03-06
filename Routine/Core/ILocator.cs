using System;

namespace Routine.Core
{
	public interface ILocator
	{
		object Locate(TypeInfo type, string id);
	}

	public interface IOptionalLocator : ILocator
	{
		bool CanLocate(TypeInfo type, string id);
		bool TryLocate(TypeInfo type, string id, out object result);
	}

	public class CannotLocateException : Exception
	{
		public CannotLocateException(TypeInfo type, string id)
			: this(type, id, null){}

		public CannotLocateException(TypeInfo type, string id, Exception innerException)
			: base("Id: " + id + ", Type: " + type, innerException){}
	}
}

