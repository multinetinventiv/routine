using Routine.Core.Configuration.Convention;

namespace Routine.Core.Configuration;

public class ConventionBasedConfiguration<TConfiguration, TFrom, TResult> where TConfiguration : ILayered
{
    private readonly TConfiguration _configuration;
    private readonly string _name;
    private readonly List<LayeredConvention<TFrom, TResult>> _conventions;
    private readonly Dictionary<TFrom, TResult> _cache;

    private Func<TFrom, ConfigurationException> _exceptionDelegate;

    public ConventionBasedConfiguration(TConfiguration configuration, string name) : this(configuration, name, false) { }
    public ConventionBasedConfiguration(TConfiguration configuration, string name, bool cacheResult)
    {
        _configuration = configuration;
        _name = name;

        _conventions = new();
        if (cacheResult)
        {
            _cache = new();
        }

        OnFailThrow(o => new ConfigurationException(name, o));
    }

    public TConfiguration OnFailThrow(ConfigurationException exception) => OnFailThrow(_ => exception);
    public TConfiguration OnFailThrow(Func<TFrom, ConfigurationException> exceptionDelegate) { _exceptionDelegate = exceptionDelegate; return _configuration; }

    public TConfiguration SetDefault() => Set(default(TResult));
    public TConfiguration SetDefault(TFrom obj) => Set(default, obj);
    public TConfiguration SetDefault(Func<TFrom, bool> whenDelegate) => Set(default, whenDelegate);

    public TConfiguration Set(TResult result) => Set(BuildRoutine.Convention<TFrom, TResult>().Constant(result));
    public TConfiguration Set(TResult result, TFrom obj) => Set(BuildRoutine.Convention<TFrom, TResult>().Constant(result).When(obj));
    public TConfiguration Set(TResult result, Func<TFrom, bool> whenDelegate) => Set(BuildRoutine.Convention<TFrom, TResult>().Constant(result).When(whenDelegate));
    public TConfiguration Set(IConvention<TFrom, TResult> convention)
    {
        Add(convention, _configuration.CurrentLayer);

        return _configuration;
    }

    private void Add(IConvention<TFrom, TResult> convention, Layer layer)
    {
        lock (_conventions)
        {
            _conventions.Add(new LayeredConvention<TFrom, TResult>(convention, layer));

            var layerGroups = _conventions.GroupBy(c => c.Layer);

            var newOrder = layerGroups.OrderByDescending(l => l.Key.Order).SelectMany(g => g).ToList();

            _conventions.Clear();
            _conventions.AddRange(newOrder);
        }
    }

    public TConfiguration Merge(ConventionBasedConfiguration<TConfiguration, TFrom, TResult> other)
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

    public virtual TResult Get(TFrom obj)
    {
        try
        {
            var result = default(TResult);
            if (_cache != null && !Equals(obj, null) && _cache.TryGetValue(obj, out result))
            {
                return result;
            }

            var found = false;
            foreach (var convention in _conventions.Select(c => c.Convention))
            {
                if (!convention.AppliesTo(obj)) { continue; }

                result = convention.Apply(obj);
                found = true;

                break;
            }

            if (!found) { throw _exceptionDelegate(obj); }

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
        catch (ConfigurationException) { throw; }
        catch (Exception ex) { throw new ConfigurationException(_name, obj, ex); }
    }
}
