using Routine.Core.Configuration;
using Routine.Engine.Configuration.ConventionBased;
using Routine.Engine.Virtual;
using System.ComponentModel;
using System.Globalization;

namespace Routine;

public static class CodingStylePatterns
{
    public static ConventionBasedCodingStyle FromEmpty(this PatternBuilder<ConventionBasedCodingStyle> source) => new();

    public static ConventionBasedCodingStyle ParseableValueTypePattern(
        this PatternBuilder<ConventionBasedCodingStyle> source
    ) => source
        .FromEmpty()

        .TypeIsValue.Set(true, t => t.CanParse() && t.IsValueType)

        .IdExtractor.Set(c => c.Id(i => i.By(o => $"{o}")).When(t => t.CanParse() && t.IsValueType))
        .Locator.Set(c =>
            c.Locator(l => l.SingleBy((t, id) => t.Parse(id))).When(t => t.CanParse() && t.IsValueType))

        .Datas.AddNoneWhen(t => t.CanParse() && t.IsValueType)
        .Operations.AddNoneWhen(t => t.CanParse() && t.IsValueType)

        .StaticInstances.Add(c => c.Constant(true, false).When(t => t.CanBe<bool>()));

    public static ConventionBasedCodingStyle EnumPattern(this PatternBuilder<ConventionBasedCodingStyle> source) => source.EnumPattern(true);

    public static ConventionBasedCodingStyle EnumPattern(this PatternBuilder<ConventionBasedCodingStyle> source,
        bool useName
    ) => useName
        ? source.FromEmpty()
            .TypeIsValue.Set(c => c.Constant(true).When(t => t.IsEnum))
            .StaticInstances.Add(c => c.By(t => t.GetEnumValues()).When(t => t.IsEnum))
            .IdExtractor.Set(c => c.Id(e => e.By(o => o.ToString())).When(t => t.IsEnum))
            .Locator.Set(c => c.Locator(l => l.SingleBy((t, id) => t.GetEnumValues()[t.GetEnumNames().IndexOf(id)]).AcceptNullResult(false)).When(t => t.IsEnum))
            .Datas.AddNoneWhen(t => t.IsEnum)
            .Operations.AddNoneWhen(t => t.IsEnum)
        : source.FromEmpty()
            .TypeIsValue.Set(false, t => t.IsEnum)
            .StaticInstances.Add(c => c.By(t => t.GetEnumValues()).When(t => t.IsEnum))
            .IdExtractor.Set(c => c.Id(e => e.By(o => ((int)o).ToString(CultureInfo.InvariantCulture))).When(t => t.IsEnum))
            .ValueExtractor.Set(c => c.Value(e => e.By(o => o.ToString())).When(t => t.IsEnum))
            .Locator.Set(c => c
                .Locator(l => l
                    .SingleBy((t, id) =>
                    {
                        var value = int.Parse(id);
                        var type = t as TypeInfo;
                        if (!Enum.IsDefined(type.GetActualType(), value))
                        {
                            throw new InvalidEnumArgumentException(id, value, type.GetActualType());
                        }

                        return Enum.ToObject(type.GetActualType(), value);
                    }).AcceptNullResult(false)
                ).When(t => t is TypeInfo && t.IsEnum))
            .Datas.AddNoneWhen(t => t.IsEnum)
            .Operations.AddNoneWhen(t => t.IsEnum);

    public static ConventionBasedCodingStyle AutoMarkWithAttributesPattern(this PatternBuilder<ConventionBasedCodingStyle> source) => source.AutoMarkWithAttributesPattern(_ => true);
    public static ConventionBasedCodingStyle AutoMarkWithAttributesPattern(this PatternBuilder<ConventionBasedCodingStyle> source, Func<object, bool> attributeFilter) =>
        source
            .FromEmpty()
            .TypeMarks.Add(c => c.By(t => t.GetCustomAttributes().Where(attributeFilter).Select(a => a.GetType().Name.BeforeLast("Attribute")).ToList()))
            .InitializerMarks.Add(s => s.By(i => i.GetCustomAttributes().Where(attributeFilter).Select(a => a.GetType().Name.BeforeLast("Attribute")).ToList()))
            .DataMarks.Add(s => s.By(m => m.GetCustomAttributes().Where(attributeFilter).Select(a => a.GetType().Name.BeforeLast("Attribute")).ToList()))
            .OperationMarks.Add(s => s.By(o => o.GetCustomAttributes().Where(attributeFilter).Select(a => a.GetType().Name.BeforeLast("Attribute")).ToList()))
            .ParameterMarks.Add(s => s.By(p => p.GetCustomAttributes().Where(attributeFilter).Select(a => a.GetType().Name.BeforeLast("Attribute")).ToList()));

    public static ConventionBasedCodingStyle VirtualTypePattern(this PatternBuilder<ConventionBasedCodingStyle> source) => source.VirtualTypePattern(Constants.DEFAULT_VIRTUAL_MARK);
    public static ConventionBasedCodingStyle VirtualTypePattern(this PatternBuilder<ConventionBasedCodingStyle> source, string virtualMark) =>
        source
            .FromEmpty()
            .Type.Set(c => c.By(o => ((VirtualObject)o).Type).When(o => o is VirtualObject))
            .IdExtractor.Set(c => c.Id(e => e.By(o => (o as VirtualObject).Id)).When(t => t is VirtualType))
            .Locator.Set(c => c.Locator(l => l.SingleBy((t, id) => new VirtualObject(id, t as VirtualType))).When(t => t is VirtualType))
            .ValueExtractor.Set(c => c.Value(e => e.By(o => $"{o}")).When(t => t is VirtualType))
            .TypeMarks.Add(virtualMark, t => t is VirtualType);
}
