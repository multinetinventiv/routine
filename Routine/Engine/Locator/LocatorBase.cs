namespace Routine.Engine.Locator
{
	public abstract class BaseLocator<TConcrete> : ILocator
		where TConcrete : BaseLocator<TConcrete>
	{
		private bool acceptNullResult;

		protected BaseLocator()
		{
			AcceptNullResult(true);
		}

		public TConcrete AcceptNullResult(bool acceptNullResult) { this.acceptNullResult = acceptNullResult; return (TConcrete)this; }

		private object LocateInner(IType type, string id)
		{
			var result = Locate(type, id);

			if (!acceptNullResult && result == null)
			{
				throw new CannotLocateException(type, id);
			}

			return result;
		}

		protected abstract object Locate(IType type, string id);

		#region ILocator implementation

		object ILocator.Locate(IType type, string id) { return LocateInner(type, id); }

		#endregion
	}

}
