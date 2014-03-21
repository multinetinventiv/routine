using System;
using System.Collections.Generic;

namespace Routine.Core.Locator
{
	public class MultipleLocator<TConfiguration> : ILocator
	{
		private readonly TConfiguration configuration;
		private readonly List<IOptionalLocator> locators;

		private bool defaultIsSet;
		private object defaultResult;
		private Func<TypeInfo, string, CannotLocateException> exceptionDelegate;

		public MultipleLocator(TConfiguration configuration)
		{
			this.configuration = configuration;
			this.locators = new List<IOptionalLocator>();

			OnFailThrow((t, id) => new CannotLocateException(t, id));
		}

		public TConfiguration OnFailReturn(object defaultResult) { defaultIsSet = true; this.defaultResult = defaultResult; return configuration; }

		public TConfiguration OnFailThrow(CannotLocateException exception) { return OnFailThrow((t, id) => exception); }
		public TConfiguration OnFailThrow(Func<TypeInfo, string, CannotLocateException> exceptionDelegate) { defaultIsSet = false; this.exceptionDelegate = exceptionDelegate; return configuration; }

		public TConfiguration Done() { return configuration; }
		public TConfiguration Done(IOptionalLocator locator) { Add(locator); return configuration; }

		public MultipleLocator<TConfiguration> Add(IOptionalLocator locator)
		{
			this.locators.Add(locator);

			return this;
		}

		public MultipleLocator<TConfiguration> Merge(MultipleLocator<TConfiguration> other)
		{
			locators.AddRange(other.locators);

			return this;
		}

		protected virtual object Locate(TypeInfo type, string id)
		{
			try
			{
				foreach(var locator in locators) 
				{
					object result;
					if(locator.TryLocate(type, id, out result))
					{
						return result;
					}
				}
			}
			catch(CannotLocateException) {throw;}
			catch(Exception ex) { throw new CannotLocateException(type, id, ex); }

			if(defaultIsSet)
			{
				return defaultResult;
			}

			throw exceptionDelegate(type, id);
		}

		#region ILocator implementation

		object ILocator.Locate(TypeInfo type, string id)
		{
			return Locate(type, id);
		}

		#endregion
	}
	
}
