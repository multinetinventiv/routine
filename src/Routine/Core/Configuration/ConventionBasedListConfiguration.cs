using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core.Configuration.Convention;

namespace Routine.Core.Configuration
{
	public class ConventionBasedListConfiguration<TConfiguration, TFrom, TResultItem> where TConfiguration : ILayered
	{
		private readonly TConfiguration configuration;
		private readonly string name;
		private readonly List<LayeredConvention<TFrom, List<TResultItem>>> conventions;
		private readonly Dictionary<TFrom, List<TResultItem>> cache;

		public ConventionBasedListConfiguration(TConfiguration configuration, string name) : this(configuration, name, false) { }
		public ConventionBasedListConfiguration(TConfiguration configuration, string name, bool cacheResult)
		{
			this.configuration = configuration;
			this.name = name;

			conventions = new List<LayeredConvention<TFrom, List<TResultItem>>>();
			if (cacheResult)
			{
				cache = new Dictionary<TFrom, List<TResultItem>>();
			}
		}

		public TConfiguration AddNoneWhen(TFrom obj) => Add(new NoConventionShouldBeAppliedConvention<TFrom, List<TResultItem>>().When(obj));
        public TConfiguration AddNoneWhen(Func<TFrom, bool> whenDelegate) => Add(new NoConventionShouldBeAppliedConvention<TFrom, List<TResultItem>>().When(whenDelegate));

        public TConfiguration Add(TResultItem result) => Add(new List<TResultItem> { result });
        public TConfiguration Add(IEnumerable<TResultItem> result) => Add(new DelegateBasedConvention<TFrom, List<TResultItem>>().Return(_ => result.ToList()));

        public TConfiguration Add(TResultItem result, TFrom obj) => Add(new List<TResultItem> { result }, obj);
        public TConfiguration Add(IEnumerable<TResultItem> result, TFrom obj) => Add(new DelegateBasedConvention<TFrom, List<TResultItem>>().Return(_ => result.ToList()).When(obj));

        public TConfiguration Add(TResultItem result, Func<TFrom, bool> whenDelegate) => Add(new List<TResultItem> { result }, whenDelegate);
        public TConfiguration Add(IEnumerable<TResultItem> result, Func<TFrom, bool> whenDelegate) => Add(new DelegateBasedConvention<TFrom, List<TResultItem>>().Return(_ => result.ToList()).When(whenDelegate));

        public TConfiguration Add(IConvention<TFrom, List<TResultItem>> convention) => Add(convention, configuration.CurrentLayer);

        private TConfiguration Add(IConvention<TFrom, List<TResultItem>> convention, Layer layer)
		{
			lock (conventions)
			{
				conventions.Add(new LayeredConvention<TFrom, List<TResultItem>>(convention, layer));

				var layerGroups = conventions.GroupBy(c => c.Layer);

				var newOrder = layerGroups.OrderByDescending(l => l.Key.Order).SelectMany(g => g).ToList();

				conventions.Clear();
				conventions.AddRange(newOrder);
			}

			return configuration;
		}

		public TConfiguration Merge(ConventionBasedListConfiguration<TConfiguration, TFrom, TResultItem> other)
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

		public List<TResultItem> Get(TFrom obj)
		{
			try
			{
                if (cache != null && !Equals(obj, null) && cache.TryGetValue(obj, out var result))
				{
					return result;
				}

				result = new List<TResultItem>();

				foreach (var convention in conventions.Select(c => c.Convention))
				{
					if (convention.AppliesTo(obj))
					{
						result.AddRange(convention.Apply(obj));
					}
				}

				result = result.Distinct().ToList();

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
			catch (NoConventionShouldBeAppliedException) { return new List<TResultItem>(); }
			catch (ConfigurationException) { throw; }
			catch (Exception ex) { throw new ConfigurationException(name, obj, ex); }
		}

		internal class NoConventionShouldBeAppliedException : Exception { }

		internal class NoConventionShouldBeAppliedConvention<TFromInner, TResultInner> : ConventionBase<TFromInner, TResultInner>
		{
			protected override TResultInner Apply(TFromInner obj)
			{
				throw new NoConventionShouldBeAppliedException();
			}
		}
	}
}
