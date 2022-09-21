var client = BuildRoutine.Context()
    .AsServiceClient(c => c.FromBasic()
        .ServiceUrlBase.Set("http://localhost:5000/service")
    )
    .Application;

var result = await HelloWorldService().PerformAsync("GetMessage", Var("name", Str("from client")));

Console.WriteLine(result.Object.Display);

Rvariable Var(string name, Robject value) => client.NewVar(name, value);
Robject HelloWorldService() => client["HelloWorld.HelloWorldService"].Get(string.Empty);
Robject Str(string value) => client["System.String"].Get(value);
