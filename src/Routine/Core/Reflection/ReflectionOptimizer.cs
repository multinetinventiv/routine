using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.CSharp;

namespace Routine.Core.Reflection
{
    internal class ReflectionOptimizer
    {
        public static bool Enabled { get; private set; } = true;
        public static void Disable() => Enabled = false;
        public static void Enable() => Enabled = true;

        private static readonly object OPTIMIZE_LIST_LOCK = new();
        private static readonly object INVOKERS_LOCK = new();

        private static readonly Dictionary<System.Reflection.MethodBase, bool> optimizeList = new();
        private static readonly Dictionary<System.Reflection.MethodBase, IMethodInvoker> invokers = new();

        public static void AddToOptimizeList(System.Reflection.MethodBase method)
        {
            if (method == null) { throw new ArgumentNullException(nameof(method)); }
            if (invokers.ContainsKey(method)) { return; }

            lock (OPTIMIZE_LIST_LOCK)
            {
                optimizeList[method] = true;
            }
        }

        public static IMethodInvoker CreateInvoker(System.Reflection.MethodBase method)
        {
            if (method == null) { throw new ArgumentNullException(nameof(method)); }

            OptimizeTheListFor(method);

            return invokers[method];
        }

        //TODO refactor, use a class instance for each optimization - hint: use method object
        private static void OptimizeTheListFor(System.Reflection.MethodBase method)
        {
            AddToOptimizeList(method);

            lock (OPTIMIZE_LIST_LOCK)
            {
                var localOptimizeList = new List<System.Reflection.MethodBase>(optimizeList.Keys);

                lock (INVOKERS_LOCK)
                {
                    var references = new Dictionary<string, MetadataReference>
                    {
                        {
                            typeof(IMethodInvoker).Assembly.Location,
                            MetadataReference.CreateFromFile(typeof(IMethodInvoker).Assembly.Location)
                        }
                    };

                    var willOptimize = new List<System.Reflection.MethodBase>();
                    var sources = new List<string>();
                    foreach (var current in localOptimizeList)
                    {
                        try
                        {
                            if (current.ContainsGenericParameters) { throw new InvalidOperationException(MissingGenericParametersMessage(current)); }

                            if (!current.IsPublic)
                            {
                                SafeAdd(current, new ReflectionMethodInvoker(current));
                            }
                            else if (!current.ReflectedType.IsPublic && !current.ReflectedType.IsNestedPublic)
                            {
                                SafeAdd(current, new ReflectionMethodInvoker(current));
                            }
                            else if (current.GetParameters().Any(pi => pi.IsIn || pi.IsOut || pi.ParameterType.IsPointer || pi.ParameterType.IsByRef))
                            {
                                SafeAdd(current, new ReflectionMethodInvoker(current));
                            }
                            else if (current.ReflectedType.IsValueType && current.IsSpecialName)
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
                                string typeName = InvokerTypeName(current);

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
                    optimizeList.Remove(current);
                }
            }
        }

        private static void SafeAdd(MethodBase current, IMethodInvoker invoker)
        {
            if (invokers.ContainsKey(current)) { return; }

            invokers.Add(current, invoker);
        }

        private static void ValidateCompilerResults(EmitResult result, string code)
        {
            if (!result.Success)
            {
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
        }

        private static void AddReferences(System.Reflection.MethodBase current, Dictionary<string, MetadataReference> references)
        {
            AddTypeReference(current.ReflectedType, references);
            AddTypeReference(current.DeclaringType, references);
            if (current is System.Reflection.MethodInfo)
            {
                AddTypeReference(((System.Reflection.MethodInfo)current).ReturnType, references);
            }

            foreach (var parameter in current.GetParameters())
            {
                AddTypeReference(parameter.ParameterType, references);
            }
        }

        private static string InvokerTypeName(System.Reflection.MethodBase current)
        {
            return current.ReflectedType.Namespace + "." + TypeName(current.ReflectedType) + "_" + MethodName(current) + "_Invoker_" + current.GetHashCode();
        }

        private const string methodInvokerTemplate =
            "namespace $Namespace$ {\n" +
            "\tpublic class $ReflectedTypeName$_$MethodName$_Invoker_$HashCode$ : $BaseInterface$ {\n" +
            "\t\tpublic object Invoke(object target, params object[] args) {\n" +
            "$Invocation$" +
            "\t\t}\n" +
            "\t}\n" +
            "}\n";

        private const string notSupportedInvocationTemplate =
            "\t\t\tthrow new System.NotSupportedException(\"Cannot optimize methods that use ref struct types such as Span<T>, Memory<T> etc.\");";

        private const string voidInvocationTemplate =
            "\t\t\t$Target$.$MethodName$($Parameters$);\n" +
            "\t\t\treturn null;\n";

        private const string nonVoidInvocationTemplate =
            "\t\t\treturn $Target$.$MethodName$($Parameters$);\n";

        private const string propertyGetInvocationTemplate =
            "\t\t\treturn $Target$.$MethodName$;\n";

        private const string indexerPropertyGetInvocationTemplate =
            "\t\t\treturn $Target$[$Parameters$];\n";

        private const string propertySetInvocationTemplate =
            "\t\t\t$Target$.$MethodName$ = $LastParameter$;\n" +
            "\t\t\treturn null;\n";

        private const string indexerPropertySetInvocationTemplate =
            "\t\t\t$Target$[$ParametersExceptLast$] = $LastParameter$;\n" +
            "\t\t\treturn null;\n";

        private const string newInvocationTemplate =
            "\t\t\treturn new $ReflectedType$($Parameters$);\n";

        private const string parameterTemplate =
            "(($ParameterType$)(args[$ParameterIndex$]??default($ParameterType$)))";

        private static string Parameter(System.Reflection.ParameterInfo parameterInfo)
        {
            if (parameterInfo == null) { return ""; }

            return parameterTemplate
                    .Replace("$ParameterType$", parameterInfo.ParameterType.ToCSharpString())
                    .Replace("$ParameterIndex$", parameterInfo.Position.ToString());
        }

        private static string Parameters(System.Reflection.ParameterInfo[] parameters)
        {
            return string.Join(",", parameters.Select(p => Parameter(p)));
        }

        private static string Invocation(System.Reflection.MethodBase method)
        {
            string result;

            if (method.IsConstructor)
            {
                if (method.GetParameters().Any(p => p.ParameterType.IsByRefLike)) { result = notSupportedInvocationTemplate; }
                else { result = newInvocationTemplate; }
            }
            else if (method.IsSpecialName)
            {
                var methodInfo = method as System.Reflection.MethodInfo;
                if (methodInfo.ReturnType.IsByRefLike || methodInfo.GetParameters().Any(p => p.ParameterType.IsByRefLike)) { result = notSupportedInvocationTemplate; }
                else if (methodInfo.Name.StartsWith("get_"))
                {
                    if (methodInfo.GetParameters().Any()) { result = indexerPropertyGetInvocationTemplate; }
                    else { result = propertyGetInvocationTemplate; }
                }
                else
                {
                    if (methodInfo.GetParameters().Length > 1) { result = indexerPropertySetInvocationTemplate; }
                    else { result = propertySetInvocationTemplate; }
                }
            }
            else
            {
                var methodInfo = method as System.Reflection.MethodInfo;
                if (methodInfo.ReturnType.IsByRefLike || methodInfo.GetParameters().Any(p => p.ParameterType.IsByRefLike)) { result = notSupportedInvocationTemplate; }
                else if (methodInfo.ReturnType == typeof(void)) { result = voidInvocationTemplate; }
                else { result = nonVoidInvocationTemplate; }
            }

            if (method.IsStatic) { result = result.Replace("$Target$", "$ReflectedType$"); }
            else { result = result.Replace("$Target$", "(($ReflectedType$)target)"); }

            return result;
        }

        private static string TypeName(Type type)
        {
            return type.ToCSharpString().AfterLast(".").Replace("<", "_").Replace(">", "_");
        }

        private static string MethodName(System.Reflection.MethodBase method)
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

        private static string Method(System.Reflection.MethodBase method)
        {
            var parameters = method.GetParameters();
            var lastParameter = parameters.LastOrDefault();
            var parametersExceptLast = parameters.Where((_, i) => i < parameters.Length - 1).ToArray();

            return methodInvokerTemplate
                    .Replace("$Invocation$", Invocation(method))
                    .Replace("$MethodName$", MethodName(method))
                    .Replace("$BaseInterface$", typeof(IMethodInvoker).ToCSharpString())
                    .Replace("$ReflectedType$", method.ReflectedType.ToCSharpString())
                    .Replace("$ReflectedTypeName$", TypeName(method.ReflectedType))
                    .Replace("$Parameters$", Parameters(parameters))
                    .Replace("$LastParameter$", Parameter(lastParameter))
                    .Replace("$ParametersExceptLast$", Parameters(parametersExceptLast))
                    .Replace("$HashCode$", method.GetHashCode().ToString())
                    .Replace("$Namespace$", method.ReflectedType.Namespace);
        }

        private static string MissingGenericParametersMessage(System.Reflection.MethodBase method)
        {
            return
                $"Missing generic parameters: {method}, {method.ReflectedType}. Cannot create invoker for a method with generic parameters. Method should already be given with its type parameters. (E.g. Cannot create invoker for IndexOf<T>, can create invoker for IndexOf<string>)";
        }

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

        private static void SafeAddReference(System.Reflection.Assembly assembly, Dictionary<string, MetadataReference> references)
        {
            if (references.ContainsKey(assembly.Location)) { return; }

            references.Add(assembly.Location, MetadataReference.CreateFromFile(assembly.Location));

            foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
            {
                SafeAddReference(System.Reflection.Assembly.Load(referencedAssembly), references);
            }
        }
    }
}
