using System;
using System.Linq;
using System.Reflection;
using Routine.Api;
using Routine.Api.Configuration;
using Routine.Client;
using Routine.Core.Configuration;

namespace Routine
{
	public static class ApiPatterns
	{
		public static ConventionBasedApiConfiguration FromEmpty(this PatternBuilder<ConventionBasedApiConfiguration> source) { return new ConventionBasedApiConfiguration(); }

		public static ConventionBasedApiConfiguration ReferencedTypesPattern(this PatternBuilder<ConventionBasedApiConfiguration> source, Assembly assembly) { return source.ReferencedTypesPattern(t => t.FullName, assembly.GetTypes()); }
		public static ConventionBasedApiConfiguration ReferencedTypesPattern(this PatternBuilder<ConventionBasedApiConfiguration> source, Func<Type, string> typeToTypeId, Assembly assembly) { return source.ReferencedTypesPattern(typeToTypeId, assembly.GetTypes()); }
		public static ConventionBasedApiConfiguration ReferencedTypesPattern(this PatternBuilder<ConventionBasedApiConfiguration> source, Assembly assembly, Func<Type, bool> typeFilter) { return source.ReferencedTypesPattern(t => t.FullName, assembly, typeFilter); }
		public static ConventionBasedApiConfiguration ReferencedTypesPattern(this PatternBuilder<ConventionBasedApiConfiguration> source, Func<Type, string> typeToTypeId, Assembly assembly, Func<Type, bool> typeFilter) { return source.ReferencedTypesPattern(typeToTypeId, assembly.GetTypes().Where(typeFilter).ToArray()); }
		public static ConventionBasedApiConfiguration ReferencedTypesPattern(this PatternBuilder<ConventionBasedApiConfiguration> source, params Type[] types) { return source.ReferencedTypesPattern(t => t.FullName, types); }
		public static ConventionBasedApiConfiguration ReferencedTypesPattern(this PatternBuilder<ConventionBasedApiConfiguration> source, Func<Type, string> typeToTypeId, params Type[] types)
		{
			return source
				.FromEmpty()
				.TypeIsRendered.Set(false, rt => types.Any(t => typeToTypeId(t) == rt.Id))
				.TypeIsRendered.Set(false, rt => rt.Id.EndsWith("?") && types.Where(t => t.IsValueType && t != typeof(void) && !t.IsGenericType).Any(t => typeToTypeId(t) == rt.Id.BeforeLast("?")))
				.ReferencedType.Set(c => c
					.By(rt => typeof(Nullable<>).MakeGenericType(types.Where(t => t.IsValueType && t != typeof(void) && !t.IsGenericType).First(t => typeToTypeId(t) == rt.Id.BeforeLast("?"))))
					.When(rt => rt.Id.EndsWith("?") && types.Where(t => t.IsValueType && t != typeof(void) && !t.IsGenericType).Any(t => typeToTypeId(t) == rt.Id.BeforeLast("?"))))
				.ReferencedType.Set(c => c
					.By(rt => types.First(t => typeToTypeId(t) == rt.Id))
					.When(rt => types.Any(t => typeToTypeId(t) == rt.Id)))
			;
		}

		public static ConventionBasedApiConfiguration ReferenceSystemTypesPattern(this PatternBuilder<ConventionBasedApiConfiguration> source) { return source.ReferenceSystemTypesPattern(t => t.FullName); }
		public static ConventionBasedApiConfiguration ReferenceSystemTypesPattern(this PatternBuilder<ConventionBasedApiConfiguration> source, Func<Type, string> typeToTypeId)
		{
			return source
				.ReferencedTypesPattern(typeToTypeId,
					typeof(char), typeof(int), typeof(string), typeof(double),
					typeof(decimal), typeof(float), typeof(TimeSpan), typeof(DateTime),
					typeof(bool), typeof(Guid), typeof(long)
				)
			;
		}

		public static ConventionBasedApiConfiguration ObsoletePattern(this PatternBuilder<ConventionBasedApiConfiguration> source, string obsoleteMark)
		{
			return source
				.FromEmpty()
				.RenderedTypeAttributes.Add(typeof(ObsoleteAttribute), mm => mm.Model.MarkedAs(obsoleteMark))
				.RenderedInitializerAttributes.Add(typeof(ObsoleteAttribute), mm => mm.Model.MarkedAs(obsoleteMark))
				.RenderedDataAttributes.Add(typeof(ObsoleteAttribute), mm => mm.Model.MarkedAs(obsoleteMark))
				.RenderedOperationAttributes.Add(typeof(ObsoleteAttribute), mm => mm.Model.MarkedAs(obsoleteMark))
			;
		}

		public static ConventionBasedApiConfiguration ParseableValueTypePattern(this PatternBuilder<ConventionBasedApiConfiguration> source)
		{
			return source
				.FromEmpty()

				.ReferencedTypeTemplate.Set(c => c
					.By(t => new NullableTypeConversionTemplate())
					.When(t => t.IsNullable() && t.GetGenericArguments()[0].CanParse()))
				.ReferencedTypeTemplate.Set(c => c
					.By(t => new ParseableValueTypeConversionTemplate())
					.When(t => t.IsValueType && t.CanParse())
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
