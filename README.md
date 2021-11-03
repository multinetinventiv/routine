# Routine

Routine is a service framework for building efficient, scalable .NET
server-side applications. It enables you to quickly create HTTP APIs from your
existing code base.

Consider below example;

```csharp
public class GreetingService
{
    public string Greet(PersonDto person)
    {
        return $"Hello {person}!";
    }
}

public record PersonDto(string Name, string Surname);
```

To expose `Greet` method as a service, below configuration is sufficient;

```csharp
app.UseRoutine(
    serviceConfiguration: sc => sc.FromBasic(),
    codingStyle: cs => cs.FromBasic()
        .AddTypes(typeof(Startup).Assembly, t => t.IsPublic)

        //Service Configuration
        .Locator.Set(c => c
            .Locator(l => l.Singleton(t => t.CreateInstance()))
            .When(t => t.Name.EndsWith("Service"))
        )
        .Operations.Add(c => c
            .PublicMethods()
            .When(t => t.Name.EndsWith("Service"))
        )

        //Dto Configuration
        .Initializers.Add(c => c
            .PublicConstructors()
            .When(t => t.Name.EndsWith("Dto"))
        )
        .Datas.Add(c => c
            .PublicProperties()
            .When(t => t.Name.EndsWith("Dto"))
        )
);
```

You can navigate through sample projects in `samples` folder to see example
usages.

Routine was developed in [inventiv](https://www.inventiv.com.tr/) for internal
use and has been used in production since 2014. Any contribution is welcome and
appreciated.

Have fun!
