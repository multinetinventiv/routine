using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core.Configuration.Convention;

namespace Routine.Core.Configuration
{
	public class ConventionalConfiguration<TConfiguration, TFrom, TResult> where TConfiguration : ILayered
	{
		private readonly TConfiguration configuration;
		private readonly string name;
		private readonly List<LayeredConvention> conventions;
		private readonly Dictionary<TFrom, TResult> cache;

		private Func<TFrom, ConfigurationException> exceptionDelegate;

		public ConventionalConfiguration(TConfiguration configuration, string name) : this(configuration, name, false) { }
		public ConventionalConfiguration(TConfiguration configuration, string name, bool cacheResult)
		{
			this.configuration = configuration;
			this.name = name;

			conventions = new List<LayeredConvention>();
			if (cacheResult)
			{
				cache = new Dictionary<TFrom, TResult>();
			}

			OnFailThrow(o => new ConfigurationException(name, o));
		}

		public TConfiguration OnFailThrow(ConfigurationException exception) { return OnFailThrow(o => exception); }
		public TConfiguration OnFailThrow(Func<TFrom, ConfigurationException> exceptionDelegate) { this.exceptionDelegate = exceptionDelegate; return configuration; }

		public TConfiguration SetDefault() { return Set(default(TResult)); }
		public TConfiguration SetDefault(TFrom obj) { return Set(default(TResult), obj); }
		public TConfiguration SetDefault(Func<TFrom, bool> whenDelegate) { return Set(default(TResult), whenDelegate); }

		public TConfiguration Set(TResult result)
		{
			return Set(BuildRoutine.Convention<TFrom, TResult>().Constant(result));
		}

		public TConfiguration Set(TResult result, TFrom obj)
		{
			return Set(BuildRoutine.Convention<TFrom, TResult>().Constant(result).When(obj));
		}

		public TConfiguration Set(TResult result, Func<TFrom, bool> whenDelegate)
		{
			return Set(BuildRoutine.Convention<TFrom, TResult>().Constant(result).When(whenDelegate));
		}

		public TConfiguration Set(IConvention<TFrom, TResult> convention)
		{
			Add(convention, configuration.CurrentLayer);

			return configuration;
		}

		private void Add(IConvention<TFrom, TResult> convention, Layer layer)
		{
			lock (conventions)
			{
				conventions.Add(new LayeredConvention(convention, layer));

				var layerGroups = conventions.GroupBy(c => c.Layer);

				var newOrder = layerGroups.OrderByDescending(l => l.Key.Order).SelectMany(g => g).ToList();

				conventions.Clear();
				conventions.AddRange(newOrder);
			}
		}

		public TConfiguration Merge(ConventionalConfiguration<TConfiguration, TFrom, TResult> other)
		{
			foreach (var convention in other.conventions)
			{
				var layer = convention.Layer;

				if (!Equals(configuration, other.configuration))
				{
					if (layer == Layer.LeastSpecific && configuration.CurrentLayer == Layer.LeastSpecific)
					{
						layer = Layer.LeastSpecific;
					}
					else if (configuration.CurrentLayer == Layer.MostSpecific)
					{
						layer = Layer.MostSpecific;
					}
					else
					{
						layer = new Layer(layer.Order + configuration.CurrentLayer.Order);
					}
				}

				Add(convention.Convention, layer);
			}

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
				foreach (var convention in conventions.Select(c => c.Convention))
				{
					if (convention.AppliesTo(obj))
					{
						result = convention.Apply(obj);
						found = true;
						break;
					}
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

		private class LayeredConvention
		{
			public IConvention<TFrom, TResult> Convention { get; private set; }
			public Layer Layer { get; private set; }

			public LayeredConvention(IConvention<TFrom, TResult> convention, Layer layer)
			{
				Convention = convention;
				Layer = layer;
			}
		}
	}
}
