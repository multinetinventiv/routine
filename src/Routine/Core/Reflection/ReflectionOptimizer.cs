using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.CSharp;
using System.Threading.Tasks;

namespace Routine.Core.Reflection
{
    internal class ReflectionOptimizer
    {
        public static bool Enabled { get; private set; } = true;
        public static void Disable() => Enabled = false;
        public static void Enable() => Enabled = true;

        private static readonly object OPTIMIZE_LIST_LOCK = new();
        private static readonly object INVOKERS_LOCK = new();

        private static readonly Dictionary<MethodBase, bool> OPTIMIZE_LIST = new();
        private static readonly Dictionary<MethodBase, IMethodInvoker> INVOKERS = new();

        public static void AddToOptimizeList(MethodBase method)
        {
            if (method == null) { throw new ArgumentNullException(nameof(method)); }
            if (INVOKERS.ContainsKey(method)) { return; }

            lock (OPTIMIZE_LIST_LOCK)
            {
                OPTIMIZE_LIST[method] = true;
            }
        }

        public static IMethodInvoker CreateInvoker(MethodBase method)
        {
            if (method == null) { throw new ArgumentNullException(nameof(method)); }

            OptimizeTheListFor(method);

            return INVOKERS[method];
        }

        // TODO refactor, use a class instance for each optimization - hint: use method object
        private static void OptimizeTheListFor(MethodBase method)
        {
            AddToOptimizeList(method);

            lock (OPTIMIZE_LIST_LOCK)
            {
                var localOptimizeList = new List<MethodBase>(OPTIMIZE_LIST.Keys);

                lock (INVOKERS_LOCK)
                {
                    var references = new Dictionary<string, MetadataReference>();

                    AddTypeReference(typeof(IMethodInvoker), references);
                    AddTypeReference(typeof(Task), references);

                    var willOptimize = new List<MethodBase>();
                    var sources = new List<string>();
                    foreach (var current in localOptimizeList)
                    {
                        try
                        {
                            if (current.ContainsGenericParameters) { throw new InvalidOperationException(MissingGenericParametersMessage(current)); }
                            if (current.ReflectedType == null) { throw new NotSupportedException(); }

                            if ((!current.IsPublic) ||
                                (!current.ReflectedType.IsPublic && !current.ReflectedType.IsNestedPublic) ||
                                (current.GetParameters().Any(pi => pi.IsIn || pi.IsOut || pi.ParameterType.IsPointer || pi.ParameterType.IsByRef)) ||
                                (current.ReflectedType.IsValueType && current.IsSpecialName) ||
                                (current.Name == "<Clone>$") ||
                                (current.Name.StartsWith("set_") && SetIsInitOnly(current)))
                            {
                                SafeAdd(current, new ReflectionMethodInvoker(current));
                            }
                            else
                            {
                                AddReferences(current, references);
                                sources.Add(Method(current));
                                willOptimize.Add(current);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (current == method)
                            {
                                throw new InvalidOperationException(
                                    $"Cannot optimize {current.ReflectedType} {current}", ex);
                            }
                        }
                    }

                    if (willOptimize.Any())
                    {
                        var code = string.Join(Environment.NewLine, sources);

                        var compilation = CSharpCompilation.Create(
                            Path.GetRandomFileName(),
                            syntaxTrees: new[] { CSharpSyntaxTree.ParseText(code) },
                            references: references.Values,
                            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                        );

                        Assembly assembly;
                        using (var ms = new MemoryStream())
                        {
                            var results = compilation.Emit(ms);

                            ValidateCompilerResults(results, code);

                            ms.Seek(0, SeekOrigin.Begin);
                            assembly = Assembly.Load(ms.ToArray());
                        }

                        foreach (var current in willOptimize)
                        {
                            try
                            {
                                var typeName = InvokerTypeName(current);
                                var type = assembly.GetType(typeName);

                                SafeAdd(current, (IMethodInvoker)Activator.CreateInstance(type));
                            }
                            catch (Exception ex)
                            {
                                if (current == method)
                                {
                                    throw new InvalidOperationException(
                                        $"Cannot optimize {current.ReflectedType} {current}", ex);
                                }
                            }
                        }
                    }
                }

                foreach (var current in localOptimizeList)
                {
                    OPTIMIZE_LIST.Remove(current);
                }
            }
        }

        private static bool SetIsInitOnly(MethodBase setMethod) =>
            setMethod is MethodInfo setMethodInfo &&
            setMethodInfo.ReturnParameter != null && 
            setMethodInfo.ReturnParameter.GetRequiredCustomModifiers().Contains(typeof(System.Runtime.CompilerServices.IsExternalInit));

        private static void SafeAdd(MethodBase current, IMethodInvoker invoker)
        {
            if (INVOKERS.ContainsKey(current)) { return; }

            INVOKERS.Add(current, invoker);
        }

        private static void ValidateCompilerResults(EmitResult result, string code)
        {
            if (result.Success) { return; }

            var failures = result.Diagnostics.Where(diagnostic =>
                diagnostic.IsWarningAsError ||
                diagnostic.Severity == DiagnosticSeverity.Error
            );
            var errors = new StringBuilder("Compiler Errors:").AppendLine().AppendLine();

            foreach (var diagnostic in failures)
            {
                errors.AppendFormat("{0} - {1}: {2}", diagnostic.Location.GetLineSpan(), diagnostic.Id, diagnostic.GetMessage());
                errors.AppendLine();
            }

            throw new Exception($"{errors}; \r\n {code}");
        }

        private static void AddReferences(MethodBase current, Dictionary<string, MetadataReference> references)
        {
            AddTypeReference(current.ReflectedType, references);
            AddTypeReference(current.DeclaringType, references);
            if (current is MethodInfo methodInfo)
            {
                AddTypeReference(methodInfo.ReturnType, references);
            }

            foreach (var parameter in current.GetParameters())
            {
                AddTypeReference(parameter.ParameterType, references);
            }
        }

        private static string InvokerTypeName(MethodBase current) => current.ReflectedType?.Namespace + "." + TypeName(current.ReflectedType) + "_" + MethodName(current) + "_Invoker_" + current.GetHashCode();

        private const string METHOD_INVOKER_TEMPLATE =
            "namespace $Namespace$ {\n" +
            "\tpublic class $ReflectedTypeName$_$MethodName$_Invoker_$HashCode$ : $BaseInterface$ {\n" +
            "\t\tpublic object Invoke(object target, params object[] args) {\n" +
            "$Invocation$" +
            "\t\t}\n" +
            "\t\tpublic async System.Threading.Tasks.Task<object> InvokeAsync(object target, params object[] args) {\n" +
            "\t\t\tthrow new System.NotImplementedException();" +
            "\t\t}\n" +
            "\t}\n" +
            "}\n";

        private const string NOT_SUPPORTED_INVOCATION_TEMPLATE =
            "\t\t\tthrow new System.NotSupportedException(\"Cannot optimize methods that use ref struct types such as Span<T>, Memory<T> etc.\");";

        private const string VOID_INVOCATION_TEMPLATE =
            "\t\t\t$Target$.$MethodName$($Parameters$);\n" +
            "\t\t\treturn null;\n";

        private const string NON_VOID_INVOCATION_TEMPLATE =
            "\t\t\treturn $Target$.$MethodName$($Parameters$);\n";

        private const string PROPERTY_GET_INVOCATION_TEMPLATE =
            "\t\t\treturn $Target$.$MethodName$;\n";

        private const string INDEXER_PROPERTY_GET_INVOCATION_TEMPLATE =
            "\t\t\treturn $Target$[$Parameters$];\n";

        private const string PROPERTY_SET_INVOCATION_TEMPLATE =
            "\t\t\t$Target$.$MethodName$ = $LastParameter$;\n" +
            "\t\t\treturn null;\n";

        private const string INDEXER_PROPERTY_SET_INVOCATION_TEMPLATE =
            "\t\t\t$Target$[$ParametersExceptLast$] = $LastParameter$;\n" +
            "\t\t\treturn null;\n";

        private const string NEW_INVOCATION_TEMPLATE =
            "\t\t\treturn new $ReflectedType$($Parameters$);\n";

        private const string PARAMETER_TEMPLATE =
            "(($ParameterType$)(args[$ParameterIndex$]??default($ParameterType$)))";

        private static string Parameter(ParameterInfo parameterInfo)
        {
            if (parameterInfo == null) { return ""; }

            return PARAMETER_TEMPLATE
                    .Replace("$ParameterType$", parameterInfo.ParameterType.ToCSharpString())
                    .Replace("$ParameterIndex$", parameterInfo.Position.ToString());
        }

        private static string Parameters(ParameterInfo[] parameters) => string.Join(",", parameters.Select(Parameter));

        private static string Invocation(MethodBase method)
        {
            string result;

            if (method.IsConstructor)
            {
                result = method.GetParameters().Any(p => p.ParameterType.IsByRefLike) ? NOT_SUPPORTED_INVOCATION_TEMPLATE : NEW_INVOCATION_TEMPLATE;
            }
            else if (method.IsSpecialName)
            {
                var methodInfo = (MethodInfo)method;
                if (methodInfo.ReturnType.IsByRefLike || methodInfo.GetParameters().Any(p => p.ParameterType.IsByRefLike)) { result = NOT_SUPPORTED_INVOCATION_TEMPLATE; }
                else if (methodInfo.Name.StartsWith("get_"))
                {
                    result = methodInfo.GetParameters().Any() ? INDEXER_PROPERTY_GET_INVOCATION_TEMPLATE : PROPERTY_GET_INVOCATION_TEMPLATE;
                }
                else
                {
                    result = methodInfo.GetParameters().Length > 1 ? INDEXER_PROPERTY_SET_INVOCATION_TEMPLATE : PROPERTY_SET_INVOCATION_TEMPLATE;
                }
            }
            else
            {
                var methodInfo = (MethodInfo)method;
                if (methodInfo.ReturnType.IsByRefLike || methodInfo.GetParameters().Any(p => p.ParameterType.IsByRefLike)) { result = NOT_SUPPORTED_INVOCATION_TEMPLATE; }
                else if (methodInfo.ReturnType == typeof(void)) { result = VOID_INVOCATION_TEMPLATE; }
                else { result = NON_VOID_INVOCATION_TEMPLATE; }
            }

            result = result.Replace("$Target$", method.IsStatic ? "$ReflectedType$" : "(($ReflectedType$)target)");

            return result;
        }

        private static string TypeName(Type type) => type.ToCSharpString().AfterLast(".").Replace("<", "_").Replace(">", "_");

        private static string MethodName(MethodBase method)
        {
            if (method.IsConstructor)
            {
                return "Constructor";
            }

            if (method.IsSpecialName)
            {
                return method.Name.After("_");
            }

            return method.Name;
        }

        private static string Method(MethodBase method)
        {
            var parameters = method.GetParameters();
            var lastParameter = parameters.LastOrDefault();
            var parametersExceptLast = parameters.Where((_, i) => i < parameters.Length - 1).ToArray();

            return METHOD_INVOKER_TEMPLATE
                    .Replace("$Invocation$", Invocation(method))
                    .Replace("$MethodName$", MethodName(method))
                    .Replace("$BaseInterface$", typeof(IMethodInvoker).ToCSharpString())
                    .Replace("$ReflectedType$", method.ReflectedType.ToCSharpString())
                    .Replace("$ReflectedTypeName$", TypeName(method.ReflectedType))
                    .Replace("$Parameters$", Parameters(parameters))
                    .Replace("$LastParameter$", Parameter(lastParameter))
                    .Replace("$ParametersExceptLast$", Parameters(parametersExceptLast))
                    .Replace("$HashCode$", method.GetHashCode().ToString())
                    .Replace("$Namespace$", method.ReflectedType?.Namespace);
        }

        private static string MissingGenericParametersMessage(MethodBase method) =>
            $"Missing generic parameters: {method}, {method.ReflectedType}. " +
            "Cannot create invoker for a method with generic parameters. " +
            "Method should already be given with its type parameters. " +
            "(E.g. Cannot create invoker for IndexOf<T>, can create invoker for IndexOf<string>)";

        private static void AddTypeReference(Type type, Dictionary<string, MetadataReference> references) { AddTypeReference(type, references, new Dictionary<Type, bool>()); }
        private static void AddTypeReference(Type type, Dictionary<string, MetadataReference> references, Dictionary<Type, bool> visits)
        {
            if (type == null) { return; }
            if (visits.ContainsKey(type)) { return; }

            visits.Add(type, true);

            SafeAddReference(type.Assembly, references);

            if (type.IsGenericType)
            {
                foreach (var genericArg in type.GetGenericArguments())
                {
                    AddTypeReference(genericArg, references, visits);
                }
            }

            AddTypeReference(type.BaseType, references, visits);

            foreach (var interfaceType in type.GetInterfaces())
            {
                AddTypeReference(interfaceType, references, visits);
            }
        }

        private static void SafeAddReference(Assembly assembly, Dictionary<string, MetadataReference> references)
        {
            if (references.ContainsKey(assembly.Location)) { return; }

            references.Add(assembly.Location, MetadataReference.CreateFromFile(assembly.Location));

            foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
            {
                SafeAddReference(Assembly.Load(referencedAssembly), references);
            }
        }
    }
}
