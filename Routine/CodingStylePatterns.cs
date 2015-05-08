using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Routine.Core.Configuration;
using Routine.Engine;
using Routine.Engine.Configuration.Conventional;
using Routine.Engine.Virtual;

namespace Routine
{
	public static class CodingStylePatterns
	{
		public static ConventionalCodingStyle FromEmpty(this PatternBuilder<ConventionalCodingStyle> source) { return new ConventionalCodingStyle(); }

		public static ConventionalCodingStyle ParseableValueTypePattern(this PatternBuilder<ConventionalCodingStyle> source)
		{
			return source
					.FromEmpty()

					.TypeIsValue.Set(true, t => t.CanParse() && t.IsValueType)

					.IdExtractor.Set(c => c.Id(e => e.By(o => string.Format("{0}", o))).When(t => t.CanParse() && t.IsValueType))
					.Locator.Set(c => c.Locator(l => l.SingleBy((t, id) => t.Parse(id))).When(t => t.CanParse() && t.IsValueType))

					.Members.AddNoneWhen(t => t.CanParse() && t.IsValueType)
					.Operations.AddNoneWhen(t => t.CanParse() && t.IsValueType)

					.StaticInstances.Set(c => c.Constant(true, false).When(t => t.CanBe<bool>()))
					;
		}

		public static ConventionalCodingStyle PolymorphismPattern(this PatternBuilder<ConventionalCodingStyle> source) { return source.PolymorphismPattern(t => true); }
		public static ConventionalCodingStyle PolymorphismPattern(this PatternBuilder<ConventionalCodingStyle> source, Func<IType, bool> viewTypePredicate)
		{
			return source
					.FromEmpty()

					.Converter.Set(c => c.ConverterByCasting().When(t => t.IsDomainType))
					.ViewTypes.Add(c => c.By(t => t.AssignableTypes.Where(viewTypePredicate).ToList()).When(t => t.IsDomainType))
					;
		}

		public static ConventionalCodingStyle EnumPattern(this PatternBuilder<ConventionalCodingStyle> source) { return source.EnumPattern(true); }
		public static ConventionalCodingStyle EnumPattern(this PatternBuilder<ConventionalCodingStyle> source, bool useName)
		{
			if (useName)
			{
				return source
					.FromEmpty()
					.TypeIsValue.Set(c => c.Constant(true).When(t => t.IsEnum))
					.StaticInstances.Set(c => c.By(t => t.GetEnumValues()).When(t => t.IsEnum))
					.IdExtractor.Set(c => c.Id(e => e.By(o => o.ToString())).When(t => t.IsEnum))
					.Locator.Set(c => c.Locator(l => l.SingleBy((t, id) => t.GetEnumValues()[t.GetEnumNames().IndexOf(id)]).AcceptNullResult(false)).When(t => t.IsEnum))
					.Members.AddNoneWhen(t => t.IsEnum)
					.Operations.AddNoneWhen(t => t.IsEnum)
					;
			}

			return source
					.FromEmpty()
					.TypeIsValue.Set(false, t => t.IsEnum)
					.StaticInstances.Set(c => c.By(t => t.GetEnumValues()).When(t => t.IsEnum))
					.IdExtractor.Set(c => c.Id(e => e.By(o => ((int)o).ToString(CultureInfo.InvariantCulture))).When(t => t.IsEnum))
					.ValueExtractor.Set(c => c.Value(e => e.By(o => o.ToString())).When(t => t.IsEnum))
					.Locator.Set(c => c.Locator(l => l.SingleBy((t, id) =>
					{
						var value = int.Parse(id);
						var type = t as TypeInfo;
						if (!Enum.IsDefined(type.GetActualType(), value))
						{
							throw new InvalidEnumArgumentException(id, value, type.GetActualType());
						}

						return Enum.ToObject(type.GetActualType(), value);
					}).AcceptNullResult(false)).When(t => t is TypeInfo && t.IsEnum))
					.Members.AddNoneWhen(t => t.IsEnum)
					.Operations.AddNoneWhen(t => t.IsEnum)
					;
		}

		public static ConventionalCodingStyle ShortModelIdPattern(this PatternBuilder<ConventionalCodingStyle> source, string prefix, string shortPrefix) { return source.ShortModelIdPattern(prefix, shortPrefix, t => true); }
		public static ConventionalCodingStyle ShortModelIdPattern(this PatternBuilder<ConventionalCodingStyle> source, string prefix, string shortPrefix, Func<IType, bool> typeFilter)
		{
			return source
					.FromEmpty()
					.TypeId.Set(c => c
						.By(t => t.GetGenericArguments()[0].FullName.ShortenModelId(prefix, shortPrefix) + "?")
						.When(t => typeFilter(t) && t.IsGenericType && t.Name.StartsWith("Nullable`1")))
					.TypeId.Set(c => c
						.By(t => t.FullName.ShortenModelId(prefix, shortPrefix))
						.When(t => typeFilter(t) && t.FullName.StartsWith(prefix + ".") && t.IsPublic));
		}

		private static string ShortenModelId(this string source, string actualPrefix, string shortPrefix)
		{
			shortPrefix = shortPrefix.Append("-");
			actualPrefix = actualPrefix.Append(".");

			return shortPrefix.Append(source.After(actualPrefix).SplitCamelCase('-').Replace("-.-", "--").ToLowerInvariant());
		}

		public static ConventionalCodingStyle AutoMarkWithAttributesPattern(this PatternBuilder<ConventionalCodingStyle> source) { return source.AutoMarkWithAttributesPattern(t => true); }
		public static ConventionalCodingStyle AutoMarkWithAttributesPattern(this PatternBuilder<ConventionalCodingStyle> source, Func<object, bool> attributeFilter)
		{
			return source
				.FromEmpty()
				.TypeMarks.Add(c => c.By(t => t.GetCustomAttributes().Where(attributeFilter).Select(a => a.GetType().Name.BeforeLast("Attribute")).ToList()))
				.InitializerMarks.Add(s => s.By(i => i.GetCustomAttributes().Where(attributeFilter).Select(a => a.GetType().Name.BeforeLast("Attribute")).ToList()))
				.MemberMarks.Add(s => s.By(m => m.GetCustomAttributes().Where(attributeFilter).Select(a => a.GetType().Name.BeforeLast("Attribute")).ToList()))
				.OperationMarks.Add(s => s.By(o => o.GetCustomAttributes().Where(attributeFilter).Select(a => a.GetType().Name.BeforeLast("Attribute")).ToList()))
				.ParameterMarks.Add(s => s.By(p => p.GetCustomAttributes().Where(attributeFilter).Select(a => a.GetType().Name.BeforeLast("Attribute")).ToList()))
				;
		}

		public static ConventionalCodingStyle VirtualTypePattern(this PatternBuilder<ConventionalCodingStyle> source) { return source.VirtualTypePattern(Constants.DEFAULT_VIRTUAL_MARK); }
		public static ConventionalCodingStyle VirtualTypePattern(this PatternBuilder<ConventionalCodingStyle> source, string virtualMark)
		{
			return source
				.FromEmpty()
				.Type.Set(c => c.By(o => ((VirtualObject)o).Type).When(o => o is VirtualObject))
				.IdExtractor.Set(c => c.Id(e => e.By(o => (o as VirtualObject).Id)).When(t => t is VirtualType))
				.Locator.Set(c => c.Locator(l => l.SingleBy((t, id) => new VirtualObject(id, t as VirtualType))).When(t => t is VirtualType))
				.ValueExtractor.Set(c => c.Value(e => e.By(o => string.Format("{0}", o))).When(t => t is VirtualType))
				.TypeMarks.Add(virtualMark, t => t is VirtualType)
			;
		}
	}
}

