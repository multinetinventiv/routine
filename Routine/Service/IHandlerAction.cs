namespace Routine.Service
{
	public interface IHandlerAction
	{
		void WriteResponse();
	}

	public interface IIndexHandlerAction : IHandlerAction { }
}