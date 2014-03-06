namespace Routine.Core
{
	public interface ICache
	{
		object this[string key]{get;}
		bool Contains(string key);
		void Add(string key, object value);
		void Remove(string key);
	}
}
