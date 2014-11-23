namespace Routine.Ui
{
	public interface IMvcContext
	{
		IMvcConfiguration Configuration { get; } 

		ApplicationViewModel Application { get; }
	}
}