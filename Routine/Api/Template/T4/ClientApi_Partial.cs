using System.Reflection;

namespace Routine.Api.Template.T4
{
	public partial class ClientApi : IApiTemplate
	{
		private ApplicationCodeModel applicationCodeModel;

		public string Render(ApplicationCodeModel applicationCodeModel)
		{
			this.applicationCodeModel = applicationCodeModel;

			return TransformText();
		}

		public ApplicationCodeModel Application { get { return applicationCodeModel; } }
	}

	public static class ClientApi_ApiGeneratorExtension
	{
		public static Assembly GenerateClientApi(this ApiGenerator source) { return source.Generate(new ClientApi()); }
	}
}
