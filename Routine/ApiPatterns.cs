using Routine.Api;
using Routine.Api.Configuration;
using Routine.Core.Configuration;

namespace Routine
{
	public static class ApiPatterns
	{
		public static ConventionalApiConfiguration FromEmpty(this PatternBuilder<ConventionalApiConfiguration> source) { return new ConventionalApiConfiguration(); }

		public static ConventionalApiConfiguration ReferencedTypeByShortModelIdPattern(this PatternBuilder<ConventionalApiConfiguration> source, string prefix, string shortPrefix)
		{
			return source
				.FromEmpty()
				.TypeIsRendered.Set(false, t => t.Id.StartsWith(shortPrefix + "-"))
				.ReferencedType.Set(c => c
					.By(t => t.Id.NormalizeModelId(prefix, shortPrefix).ToTypeInfo(true))
					.When(t => t.Id.StartsWith(shortPrefix + "-")))
			;
		}

		public static ConventionalApiConfiguration ParseableValueTypePattern(this PatternBuilder<ConventionalApiConfiguration> source)
		{
			return source
				.FromEmpty()

				.ReferencedTypeTemplate.Set(c => c
					.By(t => new ParseableValueTypeConversionTemplate())
					.When(t => t.CanParse())
				)
			;
		}

		private class ParseableValueTypeConversionTemplate : TypeConversionTemplateBase
		{
			public ParseableValueTypeConversionTemplate()
				: base("{type}.Parse({robject}.Id)", "{rtype}.Get({object}.ToString())") { }

			public override string RenderRobjectToObject(TypeCodeModel model, string robjectExpression, string rtypeExpression)
			{
				return RenderRobjectToObject(
					"type", model.ReferencedType.ToCSharpString(),
					"robject", robjectExpression
				);
			}

			public override string RenderObjectToRobject(TypeCodeModel model, string objectExpression, string rtypeExpression)
			{
				return RenderObjectToRobject(
					"rtype", rtypeExpression,
					"object", objectExpression
				);
			}
		}
	}
}
