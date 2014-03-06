using System;

namespace Routine.Core.Locator
{
	public abstract class BaseOptionalLocator<TConcrete> : IOptionalLocator
		where TConcrete : BaseOptionalLocator<TConcrete>
	{
		private bool acceptNullResult;
		private Func<TypeInfo, string, bool> whenDelegate;

		protected BaseOptionalLocator()
		{
			When((t, id) => true);
			AcceptNullResult(true);
		}

		public TConcrete WhenTypeIs<TType>(){return WhenTypeIs(type.of<TType>());}
		public TConcrete WhenTypeIs(TypeInfo type){return WhenType(t => t == type);}

		public TConcrete WhenTypeCanBe<TType>(){return WhenTypeCanBe(type.of<TType>());}
		public TConcrete WhenTypeCanBe(TypeInfo type){return WhenType(t => t.CanBe(type));}

		public TConcrete WhenType(Func<TypeInfo, bool> whenDelegate) {return When((t, id) => whenDelegate(t));}

		public TConcrete WhenId(Func<string, bool> whenDelegate) {return When((t, id) => whenDelegate(id));}

		public TConcrete When(Func<TypeInfo, string, bool> whenDelegate) {this.whenDelegate = whenDelegate; return (TConcrete)this;}

		public TConcrete AcceptNullResult(bool acceptNullResult){this.acceptNullResult = acceptNullResult; return (TConcrete)this;}

		protected virtual bool CanLocate(TypeInfo type, string id)
		{
			return whenDelegate(type, id);
		}

		private object SafeLocate(TypeInfo type, string id)
		{
			if(!CanLocate(type, id)) { throw new CannotLocateException(type, id); }

			return LocateInner(type, id);
		}

		private bool TryLocate(TypeInfo type, string id, out object result)
		{
			if(!CanLocate(type, id))
			{
				result = null;
				return false;
			}

			result = LocateInner(type, id);
			return true;
		}

		private object LocateInner(TypeInfo type, string id)
		{
			var result = Locate(type, id);

			if(!acceptNullResult && result == null)
			{
				throw new CannotLocateException(type, id);
			}

			return result;
		}

		protected abstract object Locate(TypeInfo type, string id);

		#region IOptionalLocator implementation

		bool IOptionalLocator.CanLocate(TypeInfo type, string id)
		{
			return CanLocate(type, id);
		}

		bool IOptionalLocator.TryLocate(TypeInfo type, string id, out object result)
		{
			return TryLocate(type, id, out result);
		}

		#endregion

		#region ILocator implementation

		object ILocator.Locate(TypeInfo type, string id)
		{
			return SafeLocate(type, id);
		}

		#endregion
	}
	
}
