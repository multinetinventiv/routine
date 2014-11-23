using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace Routine.Api
{
	public class ApiGenerator
	{
		private readonly ApplicationCodeModel applicationCodeModel;
		private readonly IApiGenerationConfiguration applicationConfiguration;
		private readonly List<Assembly> references;

		public ApiGenerator(IApiGenerationContext context)
		{
			applicationCodeModel = context.Application;
			applicationConfiguration = context.Configuration;

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
			if (string.IsNullOrEmpty(applicationConfiguration.GetDefaultNamespace()))
			{
				throw new InvalidOperationException("DefaultNamespace property cannot be null or empty!");
			}

			var provider = new CSharpCodeProvider();
			CompilerParameters compilerparams = CreateCompilerParameters();

			string sourceCode = template.Render(applicationCodeModel);

			var results = provider.CompileAssemblyFromSource(compilerparams, sourceCode);
			if (results.Errors.HasErrors)
			{
				var errors = new StringBuilder("Compiler Errors :\n");
				foreach (CompilerError error in results.Errors)
				{
					errors.AppendFormat("Line {0},{1}\t: {2}\n", error.Line, error.Column, error.ErrorText);
				}

				throw new Exception(string.Format("{0}\n\n Generated source code: \n\n{1}", errors, sourceCode));
			}
			Console.WriteLine(sourceCode);

			return results.CompiledAssembly;
		}

		private CompilerParameters CreateCompilerParameters()
		{
			var result = new CompilerParameters();
			result.GenerateExecutable = false;

			if (applicationConfiguration.GetInMemory())
			{
				result.GenerateInMemory = true;
			}
			else
			{
				result.OutputAssembly = applicationConfiguration.GetDefaultNamespace() + ".dll";
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
}
