﻿namespace Routine.Engine.Reflection;

internal static class MemberIndex
{
    public static MemberIndex<TKey, TItem> Build<TKey, TItem>(IEnumerable<TItem> list, Func<TItem, TKey> keyFunction)
    {
        var result = new MemberIndex<TKey, TItem>();

        foreach (var item in list)
        {
            result.Add(keyFunction(item), item);
        }

        return result;
    }
}

internal class MemberIndex<TKey, TItem>
{
    private readonly Dictionary<TKey, List<TItem>> _index;

    public MemberIndex()
    {
        _index = new();
    }

    public void Add(TKey key, TItem item)
    {
        if (!_index.ContainsKey(key))
        {
            _index.Add(key, new List<TItem>());
        }

        _index[key].Add(item);
    }

    public TItem GetFirstOrDefault(TKey key) =>
        _index.TryGetValue(key, out var result) && result.Count > 0
            ? result[0]
            : default;

    public List<TItem> GetAll(TKey key) =>
        _index.TryGetValue(key, out var result)
            ? result
            : new List<TItem>();
}
