namespace Routine.Api
{
	public abstract class CodeModelBase
	{
		private readonly IApiGenerationContext context;

		protected CodeModelBase(IApiGenerationContext context) { this.context = context; }

		protected IApiGenerationConfiguration ApiGenConfig { get { return context.ApiGenerationConfiguration; } }

		public string DefaultNamespace { get { return ApiGenConfig.DefaultNamespace; } }

		protected virtual bool ModelCanBeUsed(string modelId) { return Application.ModelCanBeUsed(modelId); }

		public ApplicationCodeModel Application { get { return context.Application; } }

		protected ObjectCodeModel CreateObject() { return context.CreateObject(); }
		protected InitializerCodeModel CreateInitializer() { return context.CreateInitializer(); }
		protected MemberCodeModel CreateMember() { return context.CreateMember(); }
		protected OperationCodeModel CreateOperation() { return context.CreateOperation(); }
		protected ParameterCodeModel CreateParameter() { return context.CreateParameter(); }
	}
}
