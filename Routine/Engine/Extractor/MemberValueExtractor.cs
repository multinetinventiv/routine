using System;

namespace Routine.Engine.Extractor
{
	public class MemberValueExtractor : ExtractorBase
	{
		private readonly IMember member;

		private Func<object, object, string> converterDelegate;

		public MemberValueExtractor(IMember member)
		{
			if (member == null) { throw new ArgumentNullException("member"); }

			this.member = member;

			Return(result => result == null ? null : result.ToString());
		}

		public MemberValueExtractor Return(Func<object, string> converterDelegate) { return Return((o, f) => converterDelegate(o)); }
		public MemberValueExtractor Return(Func<object, object, string> converterDelegate) { this.converterDelegate = converterDelegate; return this; }

		protected override string Extract(object obj)
		{
			if (obj == null)
			{
				return null;
			}

			var result = member.FetchFrom(obj);

			return converterDelegate(result, obj);
		}
	}
}
