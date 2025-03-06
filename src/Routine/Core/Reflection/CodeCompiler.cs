using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;
using System.Text;

namespace Routine.Core.Reflection;

internal class CodeCompiler
{
    private readonly StringBuilder _code = new();
    private readonly Dictionary<string, MetadataReference> _references = new();
    private HashSet<Type> _visits = new();

    internal bool HasCode => _code.Length > 0;

    internal void AddCode(string code)
    {
        _code.AppendLine(code);
    }

    internal void AddReference(Assembly assembly)
    {
        if (_references.ContainsKey(assembly.Location)) { return; }

        _references.Add(assembly.Location, MetadataReference.CreateFromFile(assembly.Location));

        foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
        {
            AddReference(Assembly.Load(referencedAssembly));
        }
    }

    internal void AddReferenceFrom<T>() => AddReferenceFrom(typeof(T));

    internal void AddReferenceFrom(Type type) => RecursiveAddReferenceFrom(type, ref _visits);

    internal Assembly Compile()
    {
        var compilation = CSharpCompilation.Create(
            assemblyName: Path.GetRandomFileName(),
            syntaxTrees: new[]
            {
                CSharpSyntaxTree.ParseText(text: $"{_code}")
            },
            references: _references.Values,
            options: new CSharpCompilationOptions(
                outputKind: OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release,
                platform: Platform.AnyCpu
            )
        );

        using var ms = new MemoryStream();
        var results = compilation.Emit(peStream: ms);

        ValidateCompilerResults(result: results);
        ms.Position = 0;
        _visits.Clear();
        return Assembly.Load(rawAssembly: ms.ToArray());
    }

    private void RecursiveAddReferenceFrom(Type type, ref HashSet<Type> visits)
    {
        if (type == null) { return; }
        if (visits.Contains(type)) { return; }

        visits.Add(type);

        AddReference(type.Assembly);

        if (type.IsGenericType)
        {
            foreach (var genericArg in type.GetGenericArguments())
            {
                RecursiveAddReferenceFrom(genericArg, ref visits);
            }
        }

        RecursiveAddReferenceFrom(type.BaseType, ref visits);

        foreach (var interfaceType in type.GetInterfaces())
        {
            RecursiveAddReferenceFrom(interfaceType, ref visits);
        }
    }

    private void ValidateCompilerResults(EmitResult result)
    {
        if (result.Success) { return; }

        var failures = result.Diagnostics.Where(diagnostic =>
            diagnostic.IsWarningAsError ||
            diagnostic.Severity == DiagnosticSeverity.Error
        );
        var errors = new StringBuilder("Compiler Errors:").AppendLine().AppendLine();

        foreach (var diagnostic in failures)
        {
            errors.Append($"{diagnostic.Location.GetLineSpan()} - {diagnostic.Id}: {diagnostic.GetMessage()}\r\n");
            errors.AppendLine();
        }

        throw new Exception($"{errors}; \r\n {_code}");
    }
}