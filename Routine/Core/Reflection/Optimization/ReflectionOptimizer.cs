using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Text;
using Microsoft.CSharp;
using Routine;

namespace Routine.Core.Reflection.Optimization
{
	public static class SystemReflectionFacadeExtensions
	{
		public static IMethodInvoker CreateInvoker(this System.Reflection.MethodBase source)
		{
			return new ReflectionOptimizer().CreateInvoker(source);
		}
	}
	public class ReflectionOptimizer
	{
		private const string methodInvokerTemplate = 
			"namespace $Namespace$ {\n" +
			"\tpublic class $ReflectedTypeName$_$MethodName$Invoker : $BaseInterface$ {\n" +
			"\t\tpublic object Invoke(object target, params object[] args) {\n" +
			"$Invocation$" +
			"\t\t}\n" +
			"\t}\n" +
			"}\n";
			
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
			"($ParameterType$)args[$ParameterIndex$]";

		private static string Parameter(System.Reflection.ParameterInfo parameterInfo)
		{
			if(parameterInfo == null){return "";}

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

			if(method.IsConstructor)
			{
				result = newInvocationTemplate;
			}
			else if(method.IsSpecialName)
			{
				if(method.Name.StartsWith("get_"))
				{
					if(method.GetParameters().Any()) { result = indexerPropertyGetInvocationTemplate; }
					else { result = propertyGetInvocationTemplate; }
				}
				else
				{
					if(method.GetParameters().Length > 1) { result = indexerPropertySetInvocationTemplate; }
					else { result = propertySetInvocationTemplate; }
				}
			}
			else
			{
				var methodInfo = method as System.Reflection.MethodInfo;
				if(methodInfo.ReturnType == typeof(void)) { result = voidInvocationTemplate; }
				else { result = nonVoidInvocationTemplate; }
			}

			if(method.IsStatic) { result = result.Replace("$Target$", "$ReflectedType$"); }
			else { result = result.Replace("$Target$", "(($ReflectedType$)target)"); }

			return result;
		}

		private static string TypeName(Type type)
		{
			return type.ToCSharpString().AfterLast(".").Replace("<", "_").Replace(">", "_");
		}

		private static string MethodName(System.Reflection.MethodBase method)
		{
			if(method.IsConstructor)
			{
				return "Constructor";
			}

			if(method.IsSpecialName)
			{
				return method.Name.After("_");
			}

			return method.Name;
		}

		private static string Method(System.Reflection.MethodBase method)
		{
			var parameters = method.GetParameters();
			var lastParameter = parameters.LastOrDefault();
			var parametersExceptLast = parameters.Where((p, i) => i < parameters.Length - 1).ToArray();

			return methodInvokerTemplate
					.Replace("$Invocation$", Invocation(method))
					.Replace("$MethodName$", MethodName(method))
					.Replace("$BaseInterface$", typeof(IMethodInvoker).ToCSharpString())
					.Replace("$ReflectedType$", method.ReflectedType.ToCSharpString())
					.Replace("$ReflectedTypeName$", TypeName(method.ReflectedType))
					.Replace("$Parameters$", Parameters(parameters))
					.Replace("$LastParameter$", Parameter(lastParameter))
					.Replace("$ParametersExceptLast$", Parameters(parametersExceptLast))
					.Replace("$Namespace$", method.ReflectedType.Namespace);
		}

		private static void AddReferences(System.Reflection.MethodBase method, CompilerParameters compilerparams)
		{
			compilerparams.ReferencedAssemblies.Add(typeof(IMethodInvoker).Assembly.GetName().Name + ".dll");
			compilerparams.ReferencedAssemblies.Add(method.ReflectedType.Assembly.GetName().Name + ".dll");
			if(method.ReflectedType.IsGenericType)
			{
				foreach(var genericArg in method.ReflectedType.GetGenericArguments())
				{
					compilerparams.ReferencedAssemblies.Add(genericArg.Assembly.GetName().Name + ".dll");
				}
			}
		}

		public IMethodInvoker CreateInvoker(System.Reflection.MethodBase method)
		{
			if(method == null){throw new ArgumentNullException("method");}
			if(!method.IsPublic){return new ReflectionMethodInvoker(method);}
			if(!method.ReflectedType.IsPublic && !method.ReflectedType.IsNestedPublic){return new ReflectionMethodInvoker(method);}

			var provider = new CSharpCodeProvider();
			var compilerparams = new CompilerParameters();
			compilerparams.GenerateExecutable = false;
			compilerparams.GenerateInMemory = true;
			AddReferences(method, compilerparams);

			string typeName = method.ReflectedType.Namespace + "." + TypeName(method.ReflectedType) + "_" +  MethodName(method) + "Invoker";

			string code = Method(method);

			var results = provider.CompileAssemblyFromSource(compilerparams, code);
			if(results.Errors.HasErrors)
			{
				var errors = new StringBuilder("Compiler Errors :\r\n");
				foreach(CompilerError error in results.Errors)
				{
					errors.AppendFormat("Line {0},{1}\t: {2}\n", error.Line, error.Column, error.ErrorText);
				}
				Console.WriteLine("Generated Source Code: \n\n" + code);
				throw new Exception(errors.ToString());
			}

			var type = results.CompiledAssembly.GetType(typeName);
			var result = (IMethodInvoker)Activator.CreateInstance(type);

			return result;
		}
	}
}
