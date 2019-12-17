namespace Routine.Service
{
	public interface IRequestHandler
	{
		void WriteResponse();
	}

	public interface IIndexRequestHandler : IRequestHandler { }
}