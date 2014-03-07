using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using Routine.Core.Service;

namespace Routine.Api.Generator
{
	public class ClientApiGenerator
	{
		private class TypeRegistration
		{
			public Type Type{ get; private set;}
			public string ModelId{get;private set;}
			public string ValueToStringCodeTemplate{get;private set;}
			public string StringToValueCodeTemplate{get;private set;}
			public bool ClientType{get; private set;}

			public TypeRegistration(Type type, string modelId, string valueToStringCodeTemplate, string stringToValueCodeTemplate)
			{
				Type = type;
				ModelId = modelId;
				ValueToStringCodeTemplate = valueToStringCodeTemplate;
				StringToValueCodeTemplate = stringToValueCodeTemplate;
			}
			public TypeRegistration(Type type, string modelId, bool clientType)
			{
				Type = type;
				ModelId = modelId;
				ClientType = clientType;
				
				if(!clientType)
				{
					ValueToStringCodeTemplate = "{value}";
					StringToValueCodeTemplate = "{valueString}";
				}
			}
		}

		private class SingletonConfiguration
		{
			private readonly Func<ObjectModel, bool> predicate;

			public List<string> IdList{ get; private set;}

			public SingletonConfiguration(Func<ObjectModel, bool> predicate, IEnumerable<string> idList)
			{
				this.predicate = predicate;
				this.IdList = new List<string>(idList);
			}

			public List<string> GetIdList(ObjectModel model)
			{
				if(!predicate(model))
				{
					return new List<string>();
				}

				return IdList;
			}
		}
			
		private readonly IObjectService service;
		private readonly Dictionary<string, TypeRegistration> typeRegistrations;
		private readonly List<Assembly> references;
		private ApplicationModel applicationModel;
		private readonly List<SingletonConfiguration> singletonConfigurations;

		public ClientApiGenerator(IObjectService service)
		{
			this.service = service;
			this.typeRegistrations = new Dictionary<string, TypeRegistration>();
			this.references = new List<Assembly>();
			this.Modules = new ModuleFilter();
			this.singletonConfigurations = new List<SingletonConfiguration>();
		}

		public ModuleFilter Modules{ get; private set;}
		public string DefaultNamespace{get;set;}
		public bool InMemory {get;set;}
		public string ApiName {get; set;}

		public void AddSystemReference() { AddReference(typeof(int).Assembly); }
		public void AddReference(Assembly assembly)
		{
			if(references.Contains(assembly))
			{
				return;
			}

			references.Add(assembly);
		}

		public void Using<T>(string modelId)
		{
			AddReference(typeof(T).Assembly);

			typeRegistrations.Add(modelId, new TypeRegistration(typeof(T), modelId, false));
		}

		public void Using(Func<TypeInfo, bool> predicate, Func<TypeInfo, string> modelIdExtractor)
		{
			Using(predicate, modelIdExtractor, false);
		}

		public void Using(Func<TypeInfo, bool> predicate, Func<TypeInfo, string> modelIdExtractor, bool clientType)
		{
			Using(predicate, modelIdExtractor, (type, modelId) => new TypeRegistration(type, modelId, clientType));
		}

		public void Using(Func<TypeInfo, bool> predicate, Func<TypeInfo, string> modelIdExtractor, string valueToStringCodeTemplate, string stringToValueCodeTemplate)
		{
			Using(predicate, modelIdExtractor, (type, modelId) => new TypeRegistration(type, modelId, valueToStringCodeTemplate, stringToValueCodeTemplate));
		}

		private void Using(Func<TypeInfo, bool> predicate, Func<TypeInfo, string> modelIdExtractor, Func<Type, string, TypeRegistration> typeRegistrar)
		{
			//TODO loop types at build time
			foreach(var reference in references)
			{
				foreach(var type in reference.GetTypes())
				{
					if(predicate(TypeInfo.Get(type)))
					{
						var modelId = modelIdExtractor(TypeInfo.Get(type));
						typeRegistrations.Add(modelId, typeRegistrar(type, modelId));
					}
				}
			}
		}

		public void AddSingleton(Func<ObjectModel, bool> predicate, params string[] ids)
		{
			singletonConfigurations.Add(new SingletonConfiguration(predicate, ids));
		}

		public Assembly Build()
		{
			if(string.IsNullOrEmpty(DefaultNamespace))
			{
				throw new InvalidOperationException("DefaultNamespace property cannot be null or empty!");
			}

			applicationModel = service.GetApplicationModel();

			var provider = new CSharpCodeProvider();
			CompilerParameters compilerparams = CreateCompilerParameters();

			string sourceCode = GenerateAssemblySourceCode();

			#if DEBUG
			Console.WriteLine(sourceCode);
			#endif

			var results = provider.CompileAssemblyFromSource(compilerparams, sourceCode);
			if(results.Errors.HasErrors)
			{
				var errors = new StringBuilder("Compiler Errors :\n");
				foreach(CompilerError error in results.Errors)
				{
					errors.AppendFormat("Line {0},{1}\t: {2}\n", error.Line, error.Column, error.ErrorText);
				}
				Console.WriteLine("Generated Source Code: \n\n" + sourceCode);
				throw new Exception(errors.ToString());
			}

			return results.CompiledAssembly;
		}

		private CompilerParameters CreateCompilerParameters()
		{
			var result = new CompilerParameters();
			result.GenerateExecutable = false;
			if(InMemory)
			{
				result.GenerateInMemory = InMemory;
			}
			else
			{
				result.OutputAssembly = DefaultNamespace + ".dll";
			}
			foreach(var reference in references)
			{
				result.ReferencedAssemblies.Add(reference.GetName().Name + ".dll");
			}
            if (!result.ReferencedAssemblies.Contains("Routine.dll")) { result.ReferencedAssemblies.Add("Routine.dll"); }
            if (!result.ReferencedAssemblies.Contains("System.Core.dll")) { result.ReferencedAssemblies.Add("System.Core.dll"); }

			return result;
		}

		private string GenerateAssemblySourceCode()
		{
			var result = new StringBuilder();

			result.AppendLine("using System.Linq;\n");

			result.AppendLine(GenerateApiClassCode());

			foreach(var model in applicationModel.Models)
			{
				if(model.IsValueModel) { continue; }
				if(!Modules.Check(model.Module)){continue;}

				result.AppendLine(GenerateClientClassCode(model));
			}

			return result.ToString();
		}

		private string GenerateApiClassCode()
		{
			if(string.IsNullOrEmpty(ApiName))
			{
				return "";
			}

			var result = new StringBuilder();

			result.AppendFormat(	"namespace {0}\n", DefaultNamespace);
			result.Append(			"{\n");
			result.AppendFormat(	"\tpublic class {0}\n", ApiName);
			result.Append(			"\t{\n");
			result.AppendFormat(	"\t\tpublic {0} Rapplication", typeof(Rapplication).FullName);
			result.Append(			"{get;private set;}\n");
			result.AppendFormat(	"\t\tpublic {0}({1} rapplication)\n", ApiName, typeof(Rapplication).FullName);
			result.Append(			"\t\t{\n");
			result.Append(			"\t\t\tRapplication = rapplication;\n");
			result.Append(			"\t\t}\n");
			foreach(var model in applicationModel.Models)
			{
				foreach(var singletonId in GetSingletonIdList(model))
				{
					result.AppendFormat("\t\tpublic {0} Get{1}{2}()\n", GenerateTypeName(model.Id), model.Name, singletonId.ToUpperInitial());
					result.Append(		"\t\t{\n");
					result.AppendFormat("\t\t\treturn new {0}(Rapplication.Get(\"{1}\", \"{2}\"));\n", GenerateTypeName(model.Id), singletonId, model.Id);
					result.Append(		"\t\t}\n");
				}
			}
			result.Append(			"\t}\n");
			result.Append(			"}");

			return result.ToString();
		}

		private List<string> GetSingletonIdList(ObjectModel model)
		{
			var result = new List<string>();

			foreach(var singletonConfiguration in singletonConfigurations)
			{
				result.AddRange(singletonConfiguration.GetIdList(model));
			}

			return result;
		}

		private string GenerateClientClassCode(ObjectModel model)
		{
			var result = 
				"namespace " + GenerateNamespace(model) + "\n" +
				"{\n" +
				"\tpublic class " + model.Name + "\n" + 
				"\t{\n" +
				"\t\tpublic " + typeof(Robject).FullName + " Robject{get; private set;}\n" +
				"\t\tpublic " + model.Name + "(" + typeof(Robject).FullName + " robject){Robject = robject;}\n" +
				"\n" +
				GenerateClientPropertyCode(model) + "\n" + 
				GenerateClientMethodCode(model) + "\n" + 
				GenerateInvalidate() + "\n" +
				GenerateToString() + "\n" +
				GenerateEqualityMembers(model) + "\n" +
				"\t}\n" +
				"}";

			return result;
		}

		private string GenerateNamespace(ObjectModel model)
		{
			string result = DefaultNamespace;
			if(!string.IsNullOrEmpty(model.Module) && !result.EndsWith(model.Module))
			{
				result += "." + model.Module;
			}
			return result;
		}

		private string GenerateClientPropertyCode(ObjectModel model)
		{
			var result = "";
			foreach(var member in model.Members)
			{
				if(!ModelCanBeUsed(member.ViewModelId)){continue;}

				result +=
					"\t\tpublic " + GenerateTypeName(member.ViewModelId, member.IsList) + " " + member.Id + "\n" +
					"\t\t{\n" +
					"\t\t\tget{return Robject[\"" + member.Id + "\"].GetValue().As"+(member.IsList?"List":"")+"(robj => " + GenerateConversion(member.ViewModelId) + ");}\n" + 
					"\t\t}\n";

			}
			return result.BeforeLast("\n");
		}

		private string GenerateConversion(string modelId)
		{
			if(!typeRegistrations.ContainsKey(modelId))
			{
				return "new " + GenerateTypeName(modelId) + "(robj)";
			}

			var registration = typeRegistrations[modelId];

			if(registration.ClientType) 
			{
				return "new " + GenerateTypeName(modelId) + "(robj)";
			}

			return registration.StringToValueCodeTemplate
					.Replace("{valueString}", "robj.Value")
					.Replace("{valueRobject}", "robj")
					.Replace("{type}", registration.Type.FullName);
		}

		private string GenerateClientMethodCode(ObjectModel model)
		{
			var result = "";
			foreach(var operation in model.Operations)
			{
				if(!ModelCanBeUsed(operation.Result.ViewModelId)){continue;}
				if(!operation.Parameters.All(p => ModelCanBeUsed(p.ViewModelId))){continue;}

				result +=
					"\t\tpublic " + GenerateReturnType(operation) + " " + operation.Id + "(" + GenerateParameterList(operation) + ")\n" +
					"\t\t{\n" +
					"\t\t\tvar " + GenerateResultVariableName(operation) + " =\n" +
					"\t\t\t\tRobject.Perform(\"" + operation.Id + "\"\n" +
					GenerateParameterPassingCode(operation) + "\n" +
					"\t\t\t\t);\n" +
					"\t\t\t" + GenerateReturnStatement(operation) + "\n" + 
					"\t\t}\n";

			}
			return result.BeforeLast("\n");
		}

		private string GenerateReturnType(OperationModel operation)
		{
			if(operation.Result.IsVoid){return "void";}

			return GenerateTypeName(operation.Result.ViewModelId, operation.Result.IsList);
		}
		
		private string GenerateParameterList(OperationModel model)
		{
			return string.Join(",", model.Parameters.Select(p => GenerateTypeName(p.ViewModelId, p.IsList) + " " + p.Id));
		}
		
		private string GenerateResultVariableName(OperationModel operation)
		{
			return operation.Id + "_result";
		}

		private string GenerateParameterPassingCode(OperationModel operation)
		{
			var result = new StringBuilder();
			foreach(var parameter in operation.Parameters)
			{
				result.AppendFormat(
					"\t\t\t\t\t, Robject.Application.NewVar" + (parameter.IsList?"List":"") +
					"(\"{0}\"", parameter.Id);
				if(typeRegistrations.ContainsKey(parameter.ViewModelId))
				{
					var registration = typeRegistrations[parameter.ViewModelId];
					if(!registration.ClientType)
					{
						var toStringCode = registration.ValueToStringCodeTemplate.Replace("{value}", "o");
						result.AppendFormat(", {0}, o => {1}, \"{2}\"", parameter.Id, toStringCode, parameter.ViewModelId);
					}
					else if(parameter.IsList)
					{
						result.AppendFormat(", {0}.Select(o => o.Robject)", parameter.Id);
					}
					else
					{
						result.AppendFormat(", {0}.Robject", parameter.Id);
					}
				}
				else if(parameter.IsList)
				{
					result.AppendFormat(", {0}.Select(o => o.Robject)", parameter.Id);
				}
				else
				{
					result.AppendFormat(", {0}.Robject", parameter.Id);
				}

				result.AppendFormat(")\n");
			}

			return result.ToString().BeforeLast("\n");
		}

		private string GenerateReturnStatement(OperationModel operation)
		{
			if(operation.Result.IsVoid){return "return;";}

			return 
				"return " + GenerateResultVariableName(operation) +
				".As"+(operation.Result.IsList?"List":"")+"(robj => " + GenerateConversion(operation.Result.ViewModelId) + ");";
		}

		private string GenerateTypeName(string modelId, bool isList)
		{
			if(!isList) { return GenerateTypeName(modelId); }

			var listType = typeof(List<>);

			return listType.Namespace + "." + listType.Name.Before("`") + 
				"<" + GenerateTypeName(modelId) + ">";
		}

		private string GenerateTypeName(string modelId)
		{
			if(typeRegistrations.ContainsKey(modelId))
			{
				return typeRegistrations[modelId].Type.ToCSharpString();
			}

			var model = applicationModel.Models.SingleOrDefault(m => m.Id == modelId);

			if(model == null)
			{
				throw new InvalidOperationException(modelId + " was not found in application model or referenced types!");
			}

			return GenerateNamespace(model) + "." + model.Name;
		}

		private string GenerateInvalidate()
		{
			return
				"\t\tpublic void Invalidate()\n" +
				"\t\t{\n" +
				"\t\t\tRobject.Invalidate();\n" +
				"\t\t}\n"
				;
		}

		private string GenerateToString()
		{
			return
				"\t\tpublic override string ToString()\n" +
				"\t\t{\n" +
				"\t\t\treturn Robject.Value;\n" +
				"\t\t}\n" +
				"\n" +
				"\t\tpublic string ToString(bool includeDebugInfo)\n" +
				"\t\t{\n" +
				"\t\t\tif(!includeDebugInfo) {return ToString();}\n" +
				"\n" +
				"\t\t\treturn string.Format(\"[Id: {0}, Value: {1}]\", Robject.Id, ToString());\n" +
				"\t\t}\n"
				;
		}

		private string GenerateEqualityMembers(ObjectModel model)
		{
			return 
				"\t\tpublic override bool Equals(object obj)\n" +
				"\t\t{\n" +
				"\t\t\tif(obj == null)\n" +
				"\t\t\t\treturn false;\n" +
				"\t\t\tif(ReferenceEquals(this, obj))\n" +
				"\t\t\t\treturn true;\n" +
				"\t\t\tif(obj.GetType() != typeof("+GenerateTypeName(model.Id)+"))\n" +
				"\t\t\t\treturn false;\n" +
				"\n" +
				"\t\t\tvar other = ("+GenerateTypeName(model.Id)+")obj;\n" +
				"\t\t\treturn Robject.Equals(other.Robject);\n" +
				"\t\t}\n" +
				"\n" +
				"\t\tpublic override int GetHashCode()\n" +
				"\t\t{\n" +
				"\t\t\treturn Robject.GetHashCode();\n" +
				"\t\t}";
		}

		private bool ModelCanBeUsed(string modelId)
		{
			var model = applicationModel.Models.SingleOrDefault(m => m.Id == modelId);

			if(model == null)
			{
				return true;
			}

			if(typeRegistrations.ContainsKey(model.Id))
			{
				return true;
			}

			return Modules.Check(model.Module);
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
