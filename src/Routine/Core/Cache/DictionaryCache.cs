namespace Routine.Core.Cache;

public class DictionaryCache : ICache
{
    private readonly Dictionary<string, object> _dictionary;

    public DictionaryCache()
    {
        _dictionary = new();
    }

    public object this[string key]
    {
        get
        {
            _dictionary.TryGetValue(key, out var result);

            return result;
        }
    }

    public bool Contains(string key) => _dictionary.ContainsKey(key);

    public void Add(string key, object value)
    {
        if (Contains(key))
        {
            _dictionary[key] = value;
            return;
        }

        _dictionary.Add(key, value);
    }

    public void Remove(string key)
    {
        if (Contains(key))
        {
            _dictionary.Remove(key);
        }
    }
}
