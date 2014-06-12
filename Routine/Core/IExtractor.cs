using System;

namespace Routine.Core
{
	public interface IExtractor<TFrom, TResult>
	{
		TResult Extract(TFrom obj);
	}

	public interface IOptionalExtractor<TFrom, TResult> : IExtractor<TFrom, TResult>
	{
		bool CanExtract(TFrom obj);
		bool TryExtract(TFrom obj, out TResult result);
	}

	public class CannotExtractException : Exception 
	{
		private static string GetTypeOf(object obj) { return obj == null ? "null" : obj.GetType().FullName; }
		private static string BuildMessage(string valueToExtract, object obj) { return string.Format("Cannot extract '{0}' from object '{1}' of type '{2}'", valueToExtract??"?", obj, GetTypeOf(obj));}

		public CannotExtractException() {}
		public CannotExtractException(object obj) : this(null, obj) {}
		public CannotExtractException(string valueToExtract, object obj) : this(valueToExtract, obj, null) {}
		public CannotExtractException(string valueToExtract, object obj, Exception innerException) : base(BuildMessage(valueToExtract, obj), innerException) {}
	}
}
