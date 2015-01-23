using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
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

			var sourceCode = new StringBuilder();

			sourceCode.AppendLine("using System.Linq;");

			sourceCode.AppendLine("[assembly: " + typeof(AssemblyVersionAttribute).FullName + "(\"" + Context.Configuration.GetVersion(Context.Application) + "\")]");
			foreach (var assemblyName in Context.Configuration.GetFriendlyAssemblyNames())
			{
				sourceCode.AppendLine("[assembly: " + typeof(InternalsVisibleToAttribute).FullName + "(\"" + assemblyName + "\")]");
			}

			sourceCode.AppendLine(template.Render(Context.Application));

			var results = provider.CompileAssemblyFromSource(compilerparams, sourceCode.ToString());
			if (results.Errors.HasErrors)
			{
				var errors = new StringBuilder("Compiler Errors :\n");
				foreach (CompilerError error in results.Errors)
				{
					errors.AppendFormat("Line {0},{1}\t: {2}\n", error.Line, error.Column, error.ErrorText);
				}

				throw new ApiGenerationException(string.Format("{0}\n\n Generated source code: \n\n{1}", errors, sourceCode));
			}

			Console.WriteLine(sourceCode);

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
