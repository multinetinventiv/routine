using Routine.Core.Configuration.Convention;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Routine.Core.Configuration;

public class ConventionBasedConfiguration<TConfiguration, TFrom, TResult> where TConfiguration : ILayered
{
    private readonly TConfiguration configuration;
    private readonly string name;
    private readonly List<LayeredConvention<TFrom, TResult>> conventions;
    private readonly Dictionary<TFrom, TResult> cache;

    private Func<TFrom, ConfigurationException> exceptionDelegate;

    public ConventionBasedConfiguration(TConfiguration configuration, string name) : this(configuration, name, false) { }
    public ConventionBasedConfiguration(TConfiguration configuration, string name, bool cacheResult)
    {
        this.configuration = configuration;
        this.name = name;

        conventions = new List<LayeredConvention<TFrom, TResult>>();
        if (cacheResult)
        {
            cache = new Dictionary<TFrom, TResult>();
        }

        OnFailThrow(o => new ConfigurationException(name, o));
    }

    public TConfiguration OnFailThrow(ConfigurationException exception) => OnFailThrow(_ => exception);
    public TConfiguration OnFailThrow(Func<TFrom, ConfigurationException> exceptionDelegate) { this.exceptionDelegate = exceptionDelegate; return configuration; }

    public TConfiguration SetDefault() => Set(default(TResult));
    public TConfiguration SetDefault(TFrom obj) => Set(default, obj);
    public TConfiguration SetDefault(Func<TFrom, bool> whenDelegate) => Set(default, whenDelegate);

    public TConfiguration Set(TResult result) => Set(BuildRoutine.Convention<TFrom, TResult>().Constant(result));
    public TConfiguration Set(TResult result, TFrom obj) => Set(BuildRoutine.Convention<TFrom, TResult>().Constant(result).When(obj));
    public TConfiguration Set(TResult result, Func<TFrom, bool> whenDelegate) => Set(BuildRoutine.Convention<TFrom, TResult>().Constant(result).When(whenDelegate));
    public TConfiguration Set(IConvention<TFrom, TResult> convention)
    {
        Add(convention, configuration.CurrentLayer);

        return configuration;
    }

    private void Add(IConvention<TFrom, TResult> convention, Layer layer)
    {
        lock (conventions)
        {
            conventions.Add(new LayeredConvention<TFrom, TResult>(convention, layer));

            var layerGroups = conventions.GroupBy(c => c.Layer);

            var newOrder = layerGroups.OrderByDescending(l => l.Key.Order).SelectMany(g => g).ToList();

            conventions.Clear();
            conventions.AddRange(newOrder);
        }
    }

    public TConfiguration Merge(ConventionBasedConfiguration<TConfiguration, TFrom, TResult> other)
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
            var result = default(TResult);
            if (cache != null && !Equals(obj, null) && cache.TryGetValue(obj, out result))
            {
                return result;
            }

            var found = false;
            foreach (var convention in conventions.Select(c => c.Convention))
            {
                if (!convention.AppliesTo(obj)) { continue; }

                result = convention.Apply(obj);
                found = true;

                break;
            }

            if (!found) { throw exceptionDelegate(obj); }

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
