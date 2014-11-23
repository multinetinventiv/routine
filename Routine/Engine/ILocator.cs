using System;

namespace Routine.Engine
{
	public interface ILocator
	{
		object Locate(IType type, string id);
	}

	public class CannotLocateException : Exception
	{
		public CannotLocateException(IType type, string id)
			: this(type, id, null){}

		public CannotLocateException(IType type, string id, Exception innerException)
			: base(string.Format("Id: {0}, Type: {1}", type, id), innerException){}
	}
}

