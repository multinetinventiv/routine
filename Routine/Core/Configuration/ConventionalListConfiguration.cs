using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core.Configuration.Convention;

namespace Routine.Core.Configuration
{
	public class ConventionalListConfiguration<TConfiguration, TFrom, TResultItem>
	{
		private readonly TConfiguration configuration;
		private readonly string name;
		private readonly List<IConvention<TFrom, List<TResultItem>>> conventions;

		public ConventionalListConfiguration(TConfiguration configuration, string name)
		{
			this.configuration = configuration;
			this.name = name;

			conventions = new List<IConvention<TFrom, List<TResultItem>>>();
		}

		public TConfiguration AddNoneWhen(TFrom obj)
		{
			return Add(new NoConventionShouldBeAppliedConvention<TFrom, List<TResultItem>>().When(obj));
		}

		public TConfiguration AddNoneWhen(Func<TFrom, bool> whenDelegate)
		{
			return Add(new NoConventionShouldBeAppliedConvention<TFrom, List<TResultItem>>().When(whenDelegate));
		}

		public TConfiguration Add(TResultItem result) { return Add(new List<TResultItem> { result }); }
		public TConfiguration Add(IEnumerable<TResultItem> result)
		{
			//TODO -> specific
			return Add(new DelegateBasedConvention<TFrom, List<TResultItem>>().Return(o => result.ToList()));
		}

		public TConfiguration Add(TResultItem result, TFrom obj) { return Add(new List<TResultItem> { result }, obj); }
		public TConfiguration Add(IEnumerable<TResultItem> result, TFrom obj)
		{
			//TODO -> specific
			return Add(new DelegateBasedConvention<TFrom, List<TResultItem>>().Return(o => result.ToList()).When(obj));
		}

		public TConfiguration Add(TResultItem result, Func<TFrom, bool> whenDelegate) { return Add(new List<TResultItem> { result }, whenDelegate); }
		public TConfiguration Add(IEnumerable<TResultItem> result, Func<TFrom, bool> whenDelegate)
		{
			//TODO -> specific
			return Add(new DelegateBasedConvention<TFrom, List<TResultItem>>().Return(o => result.ToList()).When(whenDelegate));
		}

		public TConfiguration Add(IConvention<TFrom, List<TResultItem>> convention)
		{
			//TODO -> default
			conventions.Add(convention);

			return configuration;
		}

		public TConfiguration Merge(ConventionalListConfiguration<TConfiguration, TFrom, TResultItem> other)
		{
			conventions.AddRange(other.conventions);

			return configuration;
		}

		public List<TResultItem> Get(TFrom obj)
		{
			var result = new List<TResultItem>();

			try
			{
				foreach (var convention in conventions)
				{
					if (convention.AppliesTo(obj))
					{
						result.AddRange(convention.Apply(obj));
					}
				}
			}
			catch (NoConventionShouldBeAppliedException)
			{
				return new List<TResultItem>();
			}
			catch (ConfigurationException) { throw; }
			catch (Exception ex) { throw new ConfigurationException(name, obj, ex); }

			return result.Distinct().ToList();
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
