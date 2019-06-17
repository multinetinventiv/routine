using System;

namespace Routine.Api
{
	public interface ITypeConversionTemplate
	{
		string RenderRobjectToObject(TypeCodeModel model, string robjectExpression, string rtypeExpression);
		string RenderObjectToRobject(TypeCodeModel model, string objectExpression, string rtypeExpression);
	}

	public abstract class TypeConversionTemplateBase : ITypeConversionTemplate
	{
		private readonly string robjectToObjectTemplate;
		private readonly string objectToRobjectTemplate;

		protected TypeConversionTemplateBase(string robjectToObjectTemplate, string objectToRobjectTemplate)
		{
			this.robjectToObjectTemplate = robjectToObjectTemplate;
			this.objectToRobjectTemplate = objectToRobjectTemplate;
		}

		public abstract string RenderRobjectToObject(TypeCodeModel model, string robjectExpression, string rtypeExpression);
		public abstract string RenderObjectToRobject(TypeCodeModel model, string objectExpression, string rtypeExpression);

		protected string RenderRobjectToObject(params string[] variables)
		{
			return Render(robjectToObjectTemplate, variables);
		}

		protected string RenderObjectToRobject(params string[] variables)
		{
			return Render(objectToRobjectTemplate, variables);
		}

		private static string Render(string template, params string[] variables)
		{
			if (template == null)
			{
				throw new InvalidOperationException(string.Format("Template is not configured. Variables: {0}", string.Join(", ", variables)));
			}

			if (variables.Length % 2 != 0)
			{
				throw new ArgumentException(string.Format("Length of \"variables\" parameter is odd ({0}). It should be even and name-value pairs should be consecutive.", variables.Length), "variables");
			}

			var result = template;

			for (int i = 0; i < variables.Length; i += 2)
			{
				result = result.Replace("{" + variables[i] + "}", variables[i + 1]);
			}

			return result;
		}
	}
}