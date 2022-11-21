using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace Routine.Core.Reflection;

public class CodeCompiler
{
    private readonly StringBuilder _code = new();
    private readonly Dictionary<string, MetadataReference> _references = new();

    public bool HasCode => _code.Length > 0;

    public void AddCode(string code)
    {
        _code.AppendLine(code);
    }

    public void AddReference(Assembly assembly)
    {
        if (_references.ContainsKey(assembly.Location)) { return; }

        _references.Add(assembly.Location, MetadataReference.CreateFromFile(assembly.Location));

        foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
        {
            AddReference(Assembly.Load(referencedAssembly));
        }
    }

    public void AddReferenceFrom<T>() => AddReferenceFrom(typeof(T));
    public void AddReferenceFrom(Type type) => RecursiveAddReferenceFrom(type, new HashSet<Type>());
    private void RecursiveAddReferenceFrom(Type type, HashSet<Type> visits)
    {
        if (type == null) { return; }
        if (visits.Contains(type)) { return; }

        visits.Add(type);

        AddReference(type.Assembly);

        if (type.IsGenericType)
        {
            foreach (var genericArg in type.GetGenericArguments())
            {
                RecursiveAddReferenceFrom(genericArg, visits);
            }
        }

        RecursiveAddReferenceFrom(type.BaseType, visits);

        foreach (var interfaceType in type.GetInterfaces())
        {
            RecursiveAddReferenceFrom(interfaceType, visits);
        }
    }

    public Assembly Compile()
    {
        var compilation = CSharpCompilation.Create(
            Path.GetRandomFileName(),
            syntaxTrees: new[] { CSharpSyntaxTree.ParseText($"{_code}") },
            references: _references.Values,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        using var ms = new MemoryStream();

        var results = compilation.Emit(ms);

        ValidateCompilerResults(results);

        ms.Seek(0, SeekOrigin.Begin);

        return Assembly.Load(ms.ToArray());
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
