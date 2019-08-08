namespace Routine.Api
{
	public interface IApiContext
	{
		IApiConfiguration Configuration { get; }
		ApplicationCodeModel Application { get; }
	}
}