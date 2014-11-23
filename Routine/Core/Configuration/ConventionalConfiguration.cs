using System;
using System.Collections.Generic;
using Routine.Core.Configuration.Convention;

namespace Routine.Core.Configuration
{
	public class ConventionalConfiguration<TConfiguration, TFrom, TResult>
	{
		private readonly TConfiguration configuration;
		private readonly string name;
		private readonly List<IConvention<TFrom, TResult>> conventions;
		private readonly Dictionary<TFrom, TResult> cache;

		private bool defaultIsSet;
		private TResult defaultResult;
		private Func<TFrom, ConfigurationException> exceptionDelegate;

		public ConventionalConfiguration(TConfiguration configuration, string name) : this(configuration, name, false) { }
		public ConventionalConfiguration(TConfiguration configuration, string name, bool cacheResult)
		{
			this.configuration = configuration;
			this.name = name;

			conventions = new List<IConvention<TFrom, TResult>>();
			if (cacheResult)
			{
				cache = new Dictionary<TFrom, TResult>();
			}

			OnFailThrow(o => new ConfigurationException(name, o));
		}

		public TConfiguration OnFailReturn(TResult defaultResult) { defaultIsSet = true; this.defaultResult = defaultResult; return configuration; }

		public TConfiguration OnFailThrow(ConfigurationException exception) { return OnFailThrow(o => exception); }
		public TConfiguration OnFailThrow(Func<TFrom, ConfigurationException> exceptionDelegate) { defaultIsSet = false; this.exceptionDelegate = exceptionDelegate; return configuration; }

		public TConfiguration Set(TResult result)
		{
			//TODO order -> specific
			return Set(BuildRoutine.Convention<TFrom, TResult>().Constant(result));
		}

		public TConfiguration Set(TResult result, TFrom obj)
		{
			//TODO order -> specific
			return Set(BuildRoutine.Convention<TFrom, TResult>().Constant(result).When(obj));
		}

		public TConfiguration Set(TResult result, Func<TFrom, bool> whenDelegate)
		{
			//TODO order -> specific
			return Set(BuildRoutine.Convention<TFrom, TResult>().Constant(result).When(whenDelegate));
		}

		public TConfiguration Set(IConvention<TFrom, TResult> convention)
		{
			//TODO order -> default
			conventions.Add(convention);

			return configuration;
		}

		public TConfiguration Merge(ConventionalConfiguration<TConfiguration, TFrom, TResult> other)
		{
			conventions.AddRange(other.conventions);

			return configuration;
		}

		public virtual TResult Get(TFrom obj)
		{
			try
			{
				TResult result = default(TResult);
				if (cache != null && !Equals(obj, null) && cache.TryGetValue(obj, out result))
				{
					return result;
				}

				var found = false;
				foreach (var convention in conventions)
				{
					if (convention.AppliesTo(obj))
					{
						result = convention.Apply(obj);
						found = true;
						break;
					}
				}

				if (!found && defaultIsSet)
				{
					result = defaultResult;
					found = true;
				}

				if (!found)
				{
					throw exceptionDelegate(obj);
				}

				if (cache != null)
				{
					lock (cache)
					{
						if (!Equals(obj, null) && !cache.ContainsKey(obj))
						{
							cache.Add(obj, result);
						}
					}
				}

				return result;
			}
			catch (ConfigurationException) { throw; }
			catch (Exception ex) { throw new ConfigurationException(name, obj, ex); }
		}
	}

}
