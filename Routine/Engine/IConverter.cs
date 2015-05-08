using System;

namespace Routine.Engine
{
	public interface IConverter
	{
		object Convert(object @object, IType targetType);
	}

	public class CannotConvertException : Exception
	{
		public CannotConvertException(object @object, IType targetType)
			: base(string.Format("{0} ({1}) cannot be converted to {2}", @object, @object == null ? null : @object.GetType(), targetType)) { }
	}
}