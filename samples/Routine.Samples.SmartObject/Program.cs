using Routine.Engine;
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
        .Locator.Set(c => c.Locator(l => l.SingleBy(FindSmartObject)).When(t => t.Constructors.Any(ctor => ctor.Parameters.Any())))
        .Locator.Set(c => c.Locator(l => l.Singleton(t => t.CreateInstance())))
        .Override(c => c.Operations.AddNoneWhen(t => t.IsValueType))
);

app.Run();

static object FindSmartObject(IType type, string name)
{
    var repoTypeName = type.Name + "s";
    var ti = (TypeInfo)type;

    var queryType = ti.GetActualType().Assembly.GetType($"{ti.FullName.BeforeLast(type.Name)}{repoTypeName}");
    var methodInfo = queryType?.GetMethod("Find");
    if (methodInfo == null)
    {
        throw new MissingMethodException(queryType?.Name, "Find");
    }

    return methodInfo.Invoke(Activator.CreateInstance(queryType), new object[] { name });
}
