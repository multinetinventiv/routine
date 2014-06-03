using System;
using System.Linq;
using Routine.Core;
using Routine.Core.Builder;
using Routine.Core.Configuration;

namespace Routine
{
	public static class CodingStylePatterns
	{
		public static GenericCodingStyle FromEmpty(this PatternBuilder<GenericCodingStyle> source) { return new GenericCodingStyle(); }

		public static GenericCodingStyle NullPattern(this PatternBuilder<GenericCodingStyle> source, string nullId)
		{
			return source
					.FromEmpty()
					.SerializeModelId.Done(s => s.SerializeBy(t => nullId).SerializeWhen(t => t == null)
										.DeserializeBy(id => (TypeInfo)null).DeserializeWhen(id => id == nullId))

					.ExtractId.Done(e => e.Always(nullId).WhenDefault())

					.Locate.Done(l => l.Always(null).WhenId(id => id == nullId))

					.ExtractValue.Done(e => e.Always(string.Empty).WhenDefault());
		}

		public static GenericCodingStyle ParseableValueTypePattern(this PatternBuilder<GenericCodingStyle> source)
		{
			return source
					.FromEmpty()

					.ExtractModelIsValue.Done(e => e.Always(true).When(t => t.CanBe<string>() || t.CanParse()))

					.ExtractAvailableIds.Done(e => e.Always(true.ToString(), false.ToString()).When(t => t.CanBe<bool>()))

					.ExtractId.Done(e => e.ByConverting(o => string.Format("{0}", o)).WhenType(t => t.CanBe<string>() || t.CanParse()))

					.Locate.Add(l => l.Directly().WhenTypeCanBe<string>())
							.Done(l => l.By((t, id) => t.Parse(id)).WhenType(t => t.CanParse()))

					.SelectMembers.Done(s => s.None().When(t => t.CanBe<string>() || t.CanParse()))
					.SelectOperations.Done(s => s.None().When(t => t.CanBe<string>() || t.CanParse()))
					;
		}	

		public static GenericCodingStyle EnumPattern(this PatternBuilder<GenericCodingStyle> source)
		{
			return source
					.FromEmpty()
					.ExtractModelIsValue.Done(e => e.Always(true).When(t => t.IsEnum))
					.ExtractAvailableIds.Done(e => e.ByConverting(t => Enum.GetNames(t.GetActualType()).ToList()).When(t => t.IsEnum))
					.ExtractId.Done(e => e.ByConverting(o => o.ToString()).WhenType(t => t.IsEnum))
					.Locate.Done(l => l.By((t, id) => Enum.Parse(t.GetActualType(), id)).AcceptNullResult(false).WhenType(t => t.IsEnum))
					.SelectMembers.Done(s => s.None().When(t => t.IsEnum))
					.SelectOperations.Done(s => s.None().When(t => t.IsEnum))
					;
		}

		public static GenericCodingStyle ShortModelIdPattern(this PatternBuilder<GenericCodingStyle> source, string prefix, string shortPrefix)
		{
			return source
					.FromEmpty()
					.SerializeModelId.Done(s => s
						.SerializeBy(t => t.FullName.ShortenModelId(prefix, shortPrefix))
						.SerializeWhen(t => t.FullName.StartsWith(prefix + ".") && t.IsPublic)
						.DeserializeBy(str => str.NormalizeModelId(prefix, shortPrefix).ToType())
						.DeserializeWhen(str => str.StartsWith(shortPrefix + "-")));
		}

		public static string ShortenModelId(this string source, string actualPrefix, string shortPrefix)
		{
			shortPrefix = shortPrefix.Append("-");
			actualPrefix = actualPrefix.Append(".");

			return shortPrefix.Append(source.After(actualPrefix).SplitCamelCase('-').Replace("-.-", "--").ToLowerInvariant());
		}

		public static string NormalizeModelId(this string source, string actualPrefix, string shortPrefix)
		{
			shortPrefix = shortPrefix.Append("-");
			actualPrefix = actualPrefix.Append(".");

			return actualPrefix.Append(source.After(shortPrefix).Replace("--", "-.-").SnakeCaseToCamelCase('-').ToUpperInitial());
		}

		public static GenericCodingStyle AutoMarkWithAttributesPattern(this PatternBuilder<GenericCodingStyle> source)
		{
			return source
				.FromEmpty()
				.SelectModelMarks.Done(s => s.By(t => t.GetCustomAttributes().Select(a => a.GetType().Name.BeforeLast("Attribute"))))
				.SelectOperationMarks.Done(s => s.By(o => o.GetCustomAttributes().Select(a => a.GetType().Name.BeforeLast("Attribute"))))
				.SelectMemberMarks.Done(s => s.By(m => m.GetCustomAttributes().Select(a => a.GetType().Name.BeforeLast("Attribute"))))
				.SelectParameterMarks.Done(s => s.By(p => p.GetCustomAttributes().Select(a => a.GetType().Name.BeforeLast("Attribute"))))
				;
		}
	}
}

