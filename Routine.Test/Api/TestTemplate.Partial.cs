using Routine.Api;
using Routine.Api.Configuration;

namespace Routine.Test.Api
{
	public partial class TestTemplate : IApiTemplate
	{
		private ApplicationCodeModel applicationCodeModel;

		public string Render(ApplicationCodeModel applicationCodeModel)
		{
			this.applicationCodeModel = applicationCodeModel;

			return TransformText();
		}

		public ApplicationCodeModel Application { get { return applicationCodeModel; } }

		public class Mode
		{
			public static readonly int Default = 0;
			public static readonly int Mode1 = 1;
			public static readonly int Mode2 = 2;
		}
	}

	public static class TestTemplateExtensions
	{
		public static ConventionalApiConfiguration TestTemplate(this ApiConfigurationBuilder source)
		{
			return source.FromBasic()
				.RenderedTypeTemplate.Set(c => c.By(mm => new TestTypeConversionTemplate(mm.Mode)))

				.NextLayer()
				;
		}
	}

	public class TestTypeConversionTemplate : TypeConversionTemplateBase
	{
		private readonly int mode;

		public TestTypeConversionTemplate(int mode)
			: base("new {type}({robject})", "{object}.Robject")
		{
			this.mode = mode;
		}

		public override string RenderRobjectToObject(TypeCodeModel model, string robjectExpression, string rtypeExpression)
		{
			return RenderRobjectToObject(
				"type", model.GetFullName(mode),
				"robject", robjectExpression
			);
		}

		public override string RenderObjectToRobject(TypeCodeModel model, string objectExpression, string rtypeExpression)
		{
			return RenderObjectToRobject(
				"object", objectExpression
			);
		}
	}
}