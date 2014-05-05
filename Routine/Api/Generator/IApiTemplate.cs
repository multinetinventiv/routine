namespace Routine.Api.Generator
{
	public interface IApiTemplate
	{
		string Render(IApiGenerationContext context);
	}
}
