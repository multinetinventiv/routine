namespace Routine.Core.Cache
{
    public interface ICache
    {
        object this[string key] { get; }
        bool Contains(string key);
        void Add(string key, object value);
        void Remove(string key);
    }
}
