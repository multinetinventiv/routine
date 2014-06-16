using System;
using System.Linq;

namespace Routine.Core.Extractor
{
	public class MemberValueExtractor<TFrom, TResult> : BaseOptionalExtractor<MemberValueExtractor<TFrom, TResult>, TFrom, TResult>
	{
		private readonly Func<object, IMember> member;

		private Func<object, TFrom, TResult> converter;

		public MemberValueExtractor(Func<object, IMember> memberDelegate)
		{
			this.member = memberDelegate;

			ReturnDirectly();
		}

		public MemberValueExtractor<TFrom, TResult> ReturnDirectly() { return Return(o => (TResult)o);}
		public MemberValueExtractor<TFrom, TResult> Return(Func<object, TResult> converterDelegate) { return Return((o, f) => converterDelegate(o)); }
		public MemberValueExtractor<TFrom, TResult> Return(Func<object, TFrom, TResult> converterDelegate) { this.converter = converterDelegate; return this; }
		
		protected override bool CanExtract(TFrom obj)
		{
			return base.CanExtract(obj) && obj != null && member(obj) != null;
		}

		protected override TResult Extract(TFrom obj)
		{
			var result = member(obj).FetchFrom(obj);

			return converter(result, obj);
		}
	}
}
