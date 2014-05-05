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
		private readonly IApiGenerationContext context;
		private readonly List<Assembly> references;

		public ApiGenerator(IApiGenerationContext context)
		{
			this.context = context;
			this.references = new List<Assembly>();

			AddSystemReference();
		}

		private void AddSystemReference() { AddReference<int>(); }
		public ApiGenerator AddReference<T>() { return AddReference(typeof(T).Assembly); }
		public ApiGenerator AddReference(Assembly assembly)
		{
			if(references.Contains(assembly))
			{
				return this;
			}

			references.Add(assembly);

			return this;
		}

		public Assembly Generate(IApiTemplate template)
		{
			if(string.IsNullOrEmpty(context.ApiGenerationConfiguration.DefaultNamespace))
			{
				throw new InvalidOperationException("DefaultNamespace property cannot be null or empty!");
			}

			var provider = new CSharpCodeProvider();
			CompilerParameters compilerparams = CreateCompilerParameters();

			string sourceCode = template.Render(context);
			var results = provider.CompileAssemblyFromSource(compilerparams, sourceCode);
			if (results.Errors.HasErrors)
			{
				var errors = new StringBuilder("Compiler Errors :\n");
				foreach (CompilerError error in results.Errors)
				{
					errors.AppendFormat("Line {0},{1}\t: {2}\n", error.Line, error.Column, error.ErrorText);
				}
				Console.WriteLine("Generated Source Code: \n\n" + sourceCode);
				throw new Exception(errors.ToString());
			}
#if DEBUG
			else
			{
				Console.WriteLine(sourceCode);
			}
#endif
			return results.CompiledAssembly;
		}

		private CompilerParameters CreateCompilerParameters()
		{
			var result = new CompilerParameters();
			result.GenerateExecutable = false;

			if (context.ApiGenerationConfiguration.InMemory)
			{
				result.GenerateInMemory = true;
			}
			else
			{
				result.OutputAssembly = context.ApiGenerationConfiguration.DefaultNamespace + ".dll";
			}

			foreach(var reference in references)
			{
				result.ReferencedAssemblies.Add(reference.GetName().Name + ".dll");
			}

            if (!result.ReferencedAssemblies.Contains("Routine.dll")) { result.ReferencedAssemblies.Add("Routine.dll"); }
            if (!result.ReferencedAssemblies.Contains("System.Core.dll")) { result.ReferencedAssemblies.Add("System.Core.dll"); }

			return result;
		}
	}
}

//		public interface ITodoItemService
//		{
//			ServiceResultData<TodoItemData> Update(ClientData client, Guid uid, string name);
//		}
//
//		public class TodoItemData
//		{
//			public string Id{get;set;}
//			public Guid Uid{get;set;}
//			public string Name{get;set;}
//		}
//
//		public class TodoItemService : ITodoItemService
//		{
//			private Rapplication rapplication;
//			public TodoItemService(Rapplication rapplication){this.rapplication = rapplication;}
//
//			public ServiceResultData<TodoItemData> Update(ClientData client, string id, string name, List<Guid> orderIds, TodoItemData other, List<TodoItemData> others)
//			{
//				var target = rapp.Get(id, "Multinet.Todo.TodoItem");
//
//				var Update_result = target.Perform("Update"
//					, rapplication.NewVar("name", name, o => o, ":System.String"))
//					, rapplication.NewVarList("orderIds", orderIds, o => o.ToString(), ":System.Guid"))
//					, rapplication.NewVar("other", rapplication.Get(other.Id, "Multinet.Todo.TodoItem"))
//					, rapplication.NewVarList("others", others.Select(o => rapplication.Get(o.Id, "Multinet.Todo.TodoItem")))
//				);
//
//				var Update_resultData = new TodoItemData {
//					Id = Update_result.Object.Id,
//					Uid = Update_result.Object["Uid"].GetValue().As(robj => Guid.Parse(robj.Value)),
//					Name = Update_result.Object["Name"].GetValue().As(robj => robj.Value)
//				};
//
//				return new ServiceResultData<TodoItemData>(Update_resultData);
//			}
//		}
