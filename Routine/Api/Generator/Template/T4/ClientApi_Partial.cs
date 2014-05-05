using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Routine.Api.Generator.Template.T4
{
	public partial class ClientApi : IApiTemplate
	{
		private IApiGenerationContext context;

		public string Render(IApiGenerationContext context)
		{
			this.context = context;

			return TransformText();
		}

		public ApplicationCodeModel Application { get { return context.Application; } }
	}

	public static class ClientApi_ApiGeneratorExtension
	{
		public static Assembly GenerateClientApi(this ApiGenerator source) { return source.Generate(new ClientApi()); }
	}
}
