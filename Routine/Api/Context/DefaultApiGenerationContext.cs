namespace Routine.Api.Context
{
	public class DefaultApiGenerationContext : IApiGenerationContext
	{
		public IApiGenerationConfiguration ApiGenerationConfiguration { get; private set; }

		public DefaultApiGenerationContext(IApiGenerationConfiguration configuration) 
		{ 
			ApiGenerationConfiguration = configuration; 
		}

		public ApplicationCodeModel Application { get; set; }

		public ObjectCodeModel CreateObject()
		{
			return new ObjectCodeModel(this);
		}

		public MemberCodeModel CreateMember()
		{
			return new MemberCodeModel(this);
		}

		public OperationCodeModel CreateOperation()
		{
			return new OperationCodeModel(this);
		}

		public ParameterCodeModel CreateParameter()
		{
			return new ParameterCodeModel(this);
		}
	}
}
