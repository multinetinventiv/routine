using System;
using System.Collections.Generic;

namespace Routine.Core.Locator
{
	public class MultipleLocator<TConfigurator> : ILocator
	{
		private readonly TConfigurator configurator;
		private readonly List<IOptionalLocator> locators;

		private bool defaultIsSet;
		private object defaultResult;
		private Func<TypeInfo, string, CannotLocateException> exceptionDelegate;

		public MultipleLocator(TConfigurator configurator)
		{
			this.configurator = configurator;
			this.locators = new List<IOptionalLocator>();

			OnFailThrow((t, id) => new CannotLocateException(t, id));
		}

		public TConfigurator OnFailReturn(object defaultResult) { defaultIsSet = true; this.defaultResult = defaultResult; return configurator; }

		public TConfigurator OnFailThrow(CannotLocateException exception) { return OnFailThrow((t, id) => exception); }
		public TConfigurator OnFailThrow(Func<TypeInfo, string, CannotLocateException> exceptionDelegate) { defaultIsSet = false; this.exceptionDelegate = exceptionDelegate; return configurator; }

		public TConfigurator Done() { return configurator; }
		public TConfigurator Done(IOptionalLocator locator) { Add(locator); return configurator; }

		public MultipleLocator<TConfigurator> Add(IOptionalLocator locator)
		{
			this.locators.Add(locator);

			return this;
		}

		public MultipleLocator<TConfigurator> Merge(MultipleLocator<TConfigurator> other)
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
