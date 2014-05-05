namespace Routine.Api
{
	public interface IApiTemplate
	{
		string Render(IApiGenerationContext context);
	}
}
