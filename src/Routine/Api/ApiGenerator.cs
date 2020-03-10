using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.CSharp;

namespace Routine.Api
{
    public class ApiGenerator
    {
        private readonly List<Assembly> references;

        public IApiContext Context { get; private set; }

        public ApiGenerator(IApiContext context)
        {
            Context = context;

            references = new List<Assembly>();

            AddSystemReference();
        }


        private void AddSystemReference() { AddReference<int>(); }
        public ApiGenerator AddReference<T>() { return AddReference(typeof(T).Assembly); }
        public ApiGenerator AddReference(Assembly assembly)
        {
            if (references.Contains(assembly))
            {
                return this;
            }

            references.Add(assembly);

            return this;
        }

        public Assembly Generate(IApiTemplate template)
        {
            var defaultNamespace = Context.Configuration.GetDefaultNamespace();

            var provider = new CSharpCodeProvider();
            var compilerparams = CreateCompilerParameters(defaultNamespace);

            var renderedCode = template.Render(Context.Application);

            if (!renderedCode.Contains("using System.Linq;"))
            {
                renderedCode = renderedCode.Prepend("using System.Linq;");
            }

            var assemblyCode = new StringBuilder();
            assemblyCode.AppendLine("[assembly: " + typeof(AssemblyVersionAttribute).FullName + "(\"" + Context.Configuration.GetAssemblyVersion(Context.Application) + "\")]");
            assemblyCode.AppendLine("[assembly: " + typeof(GuidAttribute).FullName + "(\"" + Context.Configuration.GetAssemblyGuid(Context.Application) + "\")]");
            foreach (var assemblyName in Context.Configuration.GetFriendlyAssemblyNames())
            {
                assemblyCode.AppendLine("[assembly: " + typeof(InternalsVisibleToAttribute).FullName + "(\"" + assemblyName + "\")]");
            }
            assemblyCode.AppendLine();

            var results = provider.CompileAssemblyFromSource(compilerparams, assemblyCode.ToString(), renderedCode);
            if (results.Errors.HasErrors)
            {
                var errors = new StringBuilder("Compiler Errors :\n");
                foreach (CompilerError error in results.Errors)
                {
                    errors.AppendFormat("Line {0},{1}\t: {2}\n", error.Line, error.Column, error.ErrorText);
                }

                throw new ApiGenerationException(
                    $"{errors}\n\n Generated source code: \n\n{assemblyCode}\n\n{renderedCode}"
                );
            }
#if DEBUG
			Console.WriteLine(assemblyCode);
			Console.WriteLine(renderedCode);
#else
            Console.WriteLine("Generation Successful! (INFO: Routine does not print successful client api code any more)");
#endif

            return results.CompiledAssembly;
        }

        private CompilerParameters CreateCompilerParameters(string defaultNamespace)
        {
            var result = new CompilerParameters();
            result.GenerateExecutable = false;

            if (Context.Configuration.GetInMemory())
            {
                result.GenerateInMemory = true;
            }
            else
            {
                var outputFileName = Context.Configuration.GetOutputFileName();
                if (string.IsNullOrEmpty(outputFileName))
                {
                    outputFileName = defaultNamespace;
                }

                result.OutputAssembly = outputFileName + ".dll";
            }

            foreach (var reference in references)
            {
                result.ReferencedAssemblies.Add(reference.Location);
            }

            if (!result.ReferencedAssemblies.Contains(GetType().Assembly.Location)) { result.ReferencedAssemblies.Add(GetType().Assembly.Location); }
            if (!result.ReferencedAssemblies.Contains("System.Core.dll")) { result.ReferencedAssemblies.Add("System.Core.dll"); }

            return result;
        }
    }

    public class ApiGenerationException : Exception
    {
        public ApiGenerationException(string message)
            : base(message) { }
    }
}
