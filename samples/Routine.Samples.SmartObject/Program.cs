using Routine.Samples.SmartObject.HelloWorld;
using System.Reflection;

using TypeInfo = Routine.TypeInfo;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRoutine(o => o.DevelopmentMode = builder.Environment.IsDevelopment());

var app = builder.Build();
app.UseRoutine(
    codingStyle: cs => cs.FromBasic()
        .AddTypes(Assembly.GetExecutingAssembly(), t => t.IsPublic)

        .Module.Set(c => c.By(t => t.Namespace.After("Routine.Samples.SmartObject.")))
        .Datas.Add(c => c.PublicProperties())
        .Operations.Add(c => c.PublicMethods(m => !m.IsInherited(true, true)))
        .IdExtractor.Set(c => c.IdByPublicProperty(p => p.Returns<string>()))
        .ValueExtractor.Set(c => c.ValueByPublicProperty(p => p.Returns<string>()))
        .ValueExtractor.Set(c => c.Value(v => v.By(o => o.GetType().Name)).When(t => t.CanBe<IFinder>()))
        .Locator.Set(c => c
            .Locator(l => l.SingleBy((t, id) => FindSmartObject(t as TypeInfo, id)))
            .When(t => t.Constructors.Any(ctor => ctor.Parameters.Any()))
        )
        .Locator.Set(c => c.Locator(l => l.Singleton(t => t.CreateInstance())))
        .StaticInstances.Add(c => c.By(t => t.CreateInstance()).When(t => !t.IsInterface && !t.IsAbstract && t.CanBe<IFinder>()))

        .Override(c => c.Operations.AddNoneWhen(t => t.IsValueType))
);

app.Run();

static object FindSmartObject(TypeInfo type, string name)
{
    if (type == null) { return null; }

    var finderType = Assembly.GetExecutingAssembly().GetType($"{type.FullName}s") ?? throw new InvalidOperationException($"Finder not found for {type.Name}");
    var finder = (IFinder)Activator.CreateInstance(finderType);

    return finder.Find(name);
}
