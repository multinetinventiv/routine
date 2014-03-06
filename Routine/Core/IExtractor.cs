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

	public static class ExtractorTupleHelperExtensions
	{
		public static TResult Extract<TFrom1, TFrom2, TResult>(this IExtractor<Tuple<TFrom1, TFrom2>, TResult> source, TFrom1 arg1, TFrom2 arg2)
		{
			return source.Extract(new Tuple<TFrom1, TFrom2>(arg1, arg2));
		}

		public static TResult Extract<TFrom1, TFrom2, TFrom3, TResult>(this IExtractor<Tuple<TFrom1, TFrom2, TFrom3>, TResult> source, TFrom1 arg1, TFrom2 arg2, TFrom3 arg3)
		{
			return source.Extract(new Tuple<TFrom1, TFrom2, TFrom3>(arg1, arg2, arg3));
		}

		public static bool CanExtract<TFrom1, TFrom2, TResult>(this IOptionalExtractor<Tuple<TFrom1, TFrom2>, TResult> source, TFrom1 arg1, TFrom2 arg2)
		{
			return source.CanExtract(new Tuple<TFrom1, TFrom2>(arg1, arg2));
		}

		public static bool CanExtract<TFrom1, TFrom2, TFrom3, TResult>(this IOptionalExtractor<Tuple<TFrom1, TFrom2, TFrom3>, TResult> source, TFrom1 arg1, TFrom2 arg2, TFrom3 arg3)
		{
			return source.CanExtract(new Tuple<TFrom1, TFrom2, TFrom3>(arg1, arg2, arg3));
		}
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
