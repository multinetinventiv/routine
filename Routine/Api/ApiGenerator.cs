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
				result.ReferencedAssemblies.Add(reference.Location);
			}

			if (!result.ReferencedAssemblies.Contains(GetType().Assembly.Location)) { result.ReferencedAssemblies.Add(GetType().Assembly.Location); }
            if (!result.ReferencedAssemblies.Contains("System.Core.dll")) { result.ReferencedAssemblies.Add("System.Core.dll"); }

			return result;
		}
	}
}

//		public interface ITodoItemService
//		{
//			ServiceResultData<TodoItemData> Update(string appToken, string languageCode, Guid uid, string name, List<string> orderIds, int otherId, List<int> otherIds);
//			ServiceResultData<List<TodoItemData>> GetAll(string appToken, string languageCode);
//		}
//
//		public class TodoItemData
//		{
//			public TodoItemData() { }
//			public TodoItemData(Robject robj)
//			{
//				Id = System.Int32.Parse(robj.Id);
//				Uid = robj["Uid"].GetValue().As(robj => System.Guid.Parse(robj.Value));
//				Name = robj["Name"].GetValue().As(robj => robj.Value);
//			}
//
//			public string Id{get;set;}
//			public Guid Uid{get;set;}
//			public string Name{get;set;}
//		}
//
//		public class TodoItemService : ITodoItemService
//		{
//			private Rapplication rapp;
//			public TodoItemService(Rapplication rapp){this.rapp = rapp;}
//
//			public ServiceResultData<TodoItemData> Update(string appToken, string languageCode, int todoItemId, string name, List<string> orderIds, int otherId, List<int> otherIds)
//			{
//				var target = rapp.Get(todoItemId, "m-todo--todo-item");
//
//				var Update_result = target.Perform("Update"
//					, rapp.NewVar("name", name, o => o, "s-string"))
//					, rapp.NewVarList("orderIds", orderIds, o => o, ":s-guid"))
//					, rapp.NewVar("other", rapp.Get(otherId.ToString(), "m-todo--todo-item"))
//					, rapp.NewVarList("others", otherIds.Select(o => rapplication.Get(o.ToString(), "m-todo--todo-item")))
//				);
//
//				var Update_resultData = Update_result.As(robj => new TodoItemData(robj));
//
//				return new ServiceResultData<TodoItemData>(Update_resultData);
//			}
//
//			public ServiceResultData<List<TodoItemData>> GetAll(string appToken, string languageCode)
//			{
//			}
//		}
