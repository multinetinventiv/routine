using System;
using System.Linq;
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
					.ModelId.Done(s => s.SerializeBy(t => nullId).SerializeWhen(t => t == null)
										.DeserializeBy(id => (TypeInfo)null).DeserializeWhen(id => id == nullId))

					.Id.Done(e => e.Always(nullId).WhenNull())

					.Locator.Done(l => l.Always(null).WhenId(id => id == nullId))

					.DisplayValue.Done(e => e.Always(string.Empty).WhenNull());
		}

		public static GenericCodingStyle ParseableValueTypePattern(this PatternBuilder<GenericCodingStyle> source, string valueTypePrefix)
		{
			return source
					.FromEmpty()
					.ModelId.Done(s => s.SerializeBy(t => t.FullName.Prepend(valueTypePrefix))
										.SerializeWhen(t => t.CanBe<string>() || t.CanParse())
										.DeserializeBy(id => id.After(valueTypePrefix).ToType())
										.DeserializeWhen(id => id.StartsWith(valueTypePrefix)))

					.ModelIsValue.Done(e => e.Always(true).When(t => t.CanBe<string>() || t.CanParse()))

					.Id.Done(e => e.ByConverting(o => string.Format("{0}", o)).WhenType(t => t.CanBe<string>() || t.CanParse()))

					.Locator.Add(l => l.Directly().WhenTypeCanBe<string>())
							.Done(l => l.By((t, id) => t.Parse(id)).WhenType(t => t.CanParse()))

					.Member.Done(s => s.None().When(t => t.CanBe<string>() || t.CanParse()))
					.Operation.Done(s => s.None().When(t => t.CanBe<string>() || t.CanParse()))
					;
		}	

		public static GenericCodingStyle EnumPattern(this PatternBuilder<GenericCodingStyle> source)
		{
			return source
					.FromEmpty()
					.ModelIsValue.Done(e => e.Always(true).When(t => t.IsEnum))
					.AvailableIds.Done(e => e.ByConverting(t => Enum.GetNames(t.GetActualType()).ToList()).When(t => t.IsEnum))
					.Id.Done(e => e.ByConverting(o => o.ToString()).WhenType(t => t.IsEnum))
					.Locator.Done(l => l.By((t, id) => Enum.Parse(t.GetActualType(), id)).AcceptNullResult(false).WhenType(t => t.IsEnum))
					.Member.Done(s => s.None().When(t => t.IsEnum))
					.Operation.Done(s => s.None().When(t => t.IsEnum))
					;
		}

		public static GenericCodingStyle CommonDomainTypeRootNamespacePattern(this PatternBuilder<GenericCodingStyle> source, string rootNamespace)
		{
			return source.CommonDomainTypeRootNamespacePattern(rootNamespace, "-"); 
		}

		public static GenericCodingStyle CommonDomainTypeRootNamespacePattern(this PatternBuilder<GenericCodingStyle> source, string rootNamespace, string separator)
		{
			var prefix = rootNamespace + ".";
			return source
					.FromEmpty()
					.DomainTypeRootNamespacesAre(rootNamespace)
					.ModelId.Done(s => s.SerializeBy(t => t.FullName.After(prefix).Replace(".", separator))
						.SerializeWhen(t => t.FullName.StartsWith(prefix) && t.IsPublic)
						.DeserializeBy(str => str.Replace(separator, ".").Prepend(prefix).ToType())
						.DeserializeWhen(str => str.Contains(separator)));
		}
	}
}

