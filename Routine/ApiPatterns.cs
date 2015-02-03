using System;
using Routine.Api;
using Routine.Api.Configuration;
using Routine.Client;
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
					.By(t => typeof(Nullable<>).MakeGenericType(t.Id.BeforeLast("?").NormalizeModelId(prefix, shortPrefix).ToTypeInfo(true).GetActualType()).ToTypeInfo())
					.When(t => t.Id.StartsWith(shortPrefix + "-") && t.Id.EndsWith("?")))
				.ReferencedType.Set(c => c
					.By(t => t.Id.NormalizeModelId(prefix, shortPrefix).ToTypeInfo(true))
					.When(t => t.Id.StartsWith(shortPrefix + "-")))
			;
		}

		private static string NormalizeModelId(this string source, string actualPrefix, string shortPrefix)
		{
			shortPrefix = shortPrefix.Append("-");
			actualPrefix = actualPrefix.Append(".");

			return actualPrefix.Append(source.After(shortPrefix).Replace("--", "-.-").SnakeCaseToCamelCase('-').ToUpperInitial());
		}

		public static ConventionalApiConfiguration ParseableValueTypePattern(this PatternBuilder<ConventionalApiConfiguration> source)
		{
			return source
				.FromEmpty()

				.ReferencedTypeTemplate.Set(c => c
					.By(t => new NullableTypeConversionTemplate())
					.When(t => t.IsGenericType && t.Name.StartsWith("Nullable`1") && t.GetGenericArguments()[0].CanParse()))
				.ReferencedTypeTemplate.Set(c => c
					.By(t => new ParseableValueTypeConversionTemplate())
					.When(t => t.CanParse())
				)
			;
		}
		
		private class NullableTypeConversionTemplate : TypeConversionTemplateBase
		{
			public NullableTypeConversionTemplate()
				: base("({robject}.IsNull ? null : new {nullable-type}({type}.Parse({robject}.Id)))", "({object}.HasValue ? {rtype}.Get({object}.Value.ToString()) : new {robject-type}())") { }

			public override string RenderRobjectToObject(TypeCodeModel model, string robjectExpression, string rtypeExpression)
			{
				return RenderRobjectToObject(
					"robject", robjectExpression,
					"nullable-type", model.ReferencedType.ToCSharpString(),
					"type", model.ReferencedType.GetGenericArguments()[0].ToCSharpString()
				);
			}

			public override string RenderObjectToRobject(TypeCodeModel model, string objectExpression, string rtypeExpression)
			{
				return RenderObjectToRobject(
					"object", objectExpression,
					"robject-type", typeof(Robject).ToCSharpString(),
					"rtype", rtypeExpression
				);
			}
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
