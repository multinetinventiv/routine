namespace Routine.Core
{
	public interface IMember : IObjectItem
	{
		bool CanFetchFrom(object target);
		object FetchFrom(object target);
	}
}
