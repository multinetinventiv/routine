namespace Routine.Api
{
	public interface IApiGenerationContext
	{
		IApiGenerationConfiguration ApiGenerationConfiguration { get; }

		ApplicationCodeModel Application { get; }

		ObjectCodeModel CreateObject();
		InitializerCodeModel CreateInitializer();
		MemberCodeModel CreateMember();
		OperationCodeModel CreateOperation();
		ParameterCodeModel CreateParameter();
	}
}
