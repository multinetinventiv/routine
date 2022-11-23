using Routine.Core.Configuration.Convention;

namespace Routine.Core.Configuration;

public class ConventionBasedListConfiguration<TConfiguration, TFrom, TResultItem> where TConfiguration : ILayered
{
    private readonly TConfiguration _configuration;
    private readonly string _name;
    private readonly List<LayeredConvention<TFrom, List<TResultItem>>> _conventions;
    private readonly Dictionary<TFrom, List<TResultItem>> _cache;

    public ConventionBasedListConfiguration(TConfiguration configuration, string name) : this(configuration, name, false) { }
    public ConventionBasedListConfiguration(TConfiguration configuration, string name, bool cacheResult)
    {
        _configuration = configuration;
        _name = name;

        _conventions = new();
        if (cacheResult)
        {
            _cache = new();
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

    public TConfiguration Add(IConvention<TFrom, List<TResultItem>> convention) => Add(convention, _configuration.CurrentLayer);

    private TConfiguration Add(IConvention<TFrom, List<TResultItem>> convention, Layer layer)
    {
        lock (_conventions)
        {
            _conventions.Add(new(convention, layer));

            var layerGroups = _conventions.GroupBy(c => c.Layer);

            var newOrder = layerGroups.OrderByDescending(l => l.Key.Order).SelectMany(g => g).ToList();

            _conventions.Clear();
            _conventions.AddRange(newOrder);
        }

        return _configuration;
    }

    public TConfiguration Merge(ConventionBasedListConfiguration<TConfiguration, TFrom, TResultItem> other)
    {
        foreach (var convention in other._conventions)
        {
            var layer = convention.Layer;

            if (!Equals(_configuration, other._configuration))
            {
                if (layer == Layer.LeastSpecific && _configuration.CurrentLayer == Layer.LeastSpecific)
                {
                    layer = Layer.LeastSpecific;
                }
                else if (_configuration.CurrentLayer == Layer.MostSpecific)
                {
                    layer = Layer.MostSpecific;
                }
                else
                {
                    layer = new(layer.Order + _configuration.CurrentLayer.Order);
                }
            }

            Add(convention.Convention, layer);
        }

        return _configuration;
    }

    public List<TResultItem> Get(TFrom obj)
    {
        try
        {
            if (_cache != null && !Equals(obj, null) && _cache.TryGetValue(obj, out var result))
            {
                return result;
            }

            result = new();

            foreach (var convention in _conventions.Select(c => c.Convention))
            {
                if (convention.AppliesTo(obj))
                {
                    result.AddRange(convention.Apply(obj));
                }
            }

            result = result.Distinct().ToList();

            if (_cache != null)
            {
                lock (_cache)
                {
                    if (!Equals(obj, null) && !_cache.ContainsKey(obj))
                    {
                        _cache.Add(obj, result);
                    }
                }
            }

            return result;
        }
        catch (NoConventionShouldBeAppliedException) { return new List<TResultItem>(); }
        catch (ConfigurationException) { throw; }
        catch (Exception ex) { throw new ConfigurationException(_name, obj, ex); }
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
