namespace Routine.Api
{
	public interface IApiGenerationContext
	{
		IApiGenerationConfiguration Configuration { get; }
		ApplicationCodeModel Application { get; }
	}
}