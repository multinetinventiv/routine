using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRoutine(o => o.DevelopmentMode = builder.Environment.IsDevelopment());

var app = builder.Build();
app.UseRoutine(
    codingStyle: cs => cs.FromBasic()
        .AddTypes(Assembly.GetExecutingAssembly(), t => t.IsPublic)

        .Module.Set(c => c.By(t => t.Namespace.After("Routine.Samples.Basic.")))

        //Service Configuration
        .ValueExtractor.Set(c => c.Value(v => v.By(obj => obj.GetType().Name.SplitCamelCase(' '))).When(t => t.Name.EndsWith("Service")))
        .Locator.Set(c => c.Locator(l => l.Singleton(t => t.CreateInstance())).When(t => t.Name.EndsWith("Service")))
        .StaticInstances.Add(c => c.By(t => t.CreateInstance()).When(t => t.Name.EndsWith("Service")))
        .Operations.Add(c => c.PublicMethods(m => !m.IsInherited(true, true)).When(t => t.Name.EndsWith("Service")))

        //Dto Configuration
        .Initializers.Add(c => c.PublicConstructors().When(t => t.Name.EndsWith("Dto")))
        .Datas.Add(c => c.PublicProperties().When(t => t.Name.EndsWith("Dto")))
        .IdExtractor.Set(c => c.Id(id => id.Constant("Dto")).When(t => t.Name.EndsWith("Dto")))
        .ValueExtractor.Set(c => c.ValueByPublicProperty(p => p.Returns<string>()).When(t => t.Name.EndsWith("Dto"))),
);

app.Run();
