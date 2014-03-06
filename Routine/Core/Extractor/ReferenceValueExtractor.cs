using System;
using System.Linq;

namespace Routine.Core.Extractor
{
	public class ReferenceValueExtractor<TFrom, TResult> : BaseOptionalExtractor<ReferenceValueExtractor<TFrom, TResult>, TFrom, TResult>
	{
		private readonly ISelector<TypeInfo, IMember> selector;
		private readonly Func<TFrom, ISelector<TypeInfo, IMember>> selectorDelegate;

		private Func<TFrom, object> useDelegate;
		private Func<object, TFrom, TResult> converterDelegate;

		public ReferenceValueExtractor(ISelector<TypeInfo, IMember> selector)
		{
			this.selector = selector;

			SetDefaults();
		}

		public ReferenceValueExtractor(Func<TFrom, ISelector<TypeInfo, IMember>> selectorDelegate)
		{
			this.selectorDelegate = selectorDelegate;

			SetDefaults();
		}

		private void SetDefaults()
		{
			Using(o => o);
			ReturnDirectly();
		}

		public ReferenceValueExtractor<TFrom, TResult> Using(Func<TFrom, object> useDelegate) {this.useDelegate = useDelegate; return this;}

		public ReferenceValueExtractor<TFrom, TResult> ReturnDirectly() { return Return(o => (TResult)o);}
		public ReferenceValueExtractor<TFrom, TResult> Return(Func<object, TResult> converterDelegate) { return Return((o, f) => converterDelegate(o)); }
		public ReferenceValueExtractor<TFrom, TResult> Return(Func<object, TFrom, TResult> converterDelegate) {this.converterDelegate = converterDelegate; return this;}

		protected ISelector<TypeInfo, IMember> Selector(TFrom obj) 
		{
			if(selector != null)
			{
				return selector;
			}

			return selectorDelegate(obj); 
		}

		protected object Target(TFrom obj) { return useDelegate(obj); }
		protected IMember Member(TFrom obj) { return Selector(obj).Select(Target(obj).GetTypeInfo()).FirstOrDefault(d => IsValid(d, obj)); }
		protected bool IsValid(IMember member, TFrom obj) { return member != null && member.CanFetchFrom(obj);}

		protected override bool CanExtract(TFrom obj)
		{
			return base.CanExtract(obj) && Target(obj) != null && IsValid(Member(obj), obj);
		}

		protected override TResult Extract(TFrom obj)
		{
			var result = Member(obj).FetchFrom(Target(obj));

			return converterDelegate(result, obj);
		}
	}
}
