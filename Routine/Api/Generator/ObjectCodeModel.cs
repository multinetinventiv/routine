using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core;

namespace Routine.Api.Generator
{
	public class ObjectCodeModel : CodeModelBase
	{
		public ObjectCodeModel(IApiGenerationContext context)
			: base(context) { }

		private bool isVoid;
		private bool isList;
		private ObjectModel model;
		private string @namespace;
		private TypeInfo type;

		internal ObjectCodeModel Void()
		{
			isVoid = true;
			model = new ObjectModel { Id = "void", Name = "void" };

			return this;
		}

		internal ObjectCodeModel With(string modelId, bool isList)
		{
			this.isList = isList;

			try
			{
				this.type = ApiGenConfig.ReferencedModelIdSerializer.Deserialize(modelId);

				if (type == null) { throw new CannotDeserializeException(modelId); }

				this.model = new ObjectModel 
				{ 
					Id = modelId, 
					Name = type.Name, 
					IsValueModel = !ApiGenConfig.ReferencedTypeIsClientTypeExtractor.Extract(type),
				};

				@namespace = type.Namespace;
				
				return this;
			}
			catch (CannotDeserializeException ex)
			{
				var ocm = Application.Models.SingleOrDefault(m => m.Id == modelId);

				if(ocm == null)
				{
					throw new InvalidOperationException(modelId + " could not be deserialized to a type and was not found in application model!", ex);
				}

				return With(ocm.model);
			}
		}

		internal ObjectCodeModel With(ObjectModel model)
		{
			this.model = model;
			
			@namespace = DefaultNamespace;
			if (!string.IsNullOrEmpty(model.Module) && !@namespace.EndsWith(model.Module))
			{
				@namespace += "." + model.Module;
			}

			return this;
		}

		public bool IsVoid { get { return isVoid; } }
		public bool IsList { get { return isList; } }
		public string Id { get { return model.Id; } }
		public string Namespace { get { return @namespace; } }
		public string Name { get { return model.Name; } }
		public string FullName 
		{ 
			get 
			{
				if (!isList)
				{
					return FullNameIgnoringList;
				}

				var listType = typeof(List<>);

				return listType.Namespace + "." + listType.Name.Before("`") + "<" + FullNameIgnoringList + ">";
			} 
		}

		public string FullNameIgnoringList
		{
			get
			{
				if (string.IsNullOrEmpty(Namespace)) { return Name; }

				return Namespace + "." + Name;
			}
		}

		public bool IsValueModel { get { return model.IsValueModel; } }

		public List<string> SingletonIds { get { return ApiGenConfig.SingletonIdSelector.Select(this); } }

		public List<MemberCodeModel> Members 
		{ 
			get 
			{
				return model.Members
					.Where(m => ModelCanBeUsed(m.ViewModelId))
					.Select(m => CreateMember().With(m))
					.ToList();
			}
		}

		public List<OperationCodeModel> Operations
		{
			get
			{
				return model.Operations
					.Where(o => ModelCanBeUsed(o.Result.ViewModelId) && o.Parameters.All(p => ModelCanBeUsed(p.ViewModelId)))
					.Select(o => CreateOperation().With(o))
					.ToList();
			}
		}

		public string GetStringToValueCode(string robjectVariableName)
		{
			if (!model.IsValueModel)
			{
				throw new InvalidOperationException("Only value models can have string to value conversion");
			}

			return ApiGenConfig.StringToValueCodeTemplateExtractor.Extract(type)
					.Replace("{valueString}", robjectVariableName + ".Value")
					.Replace("{valueRobject}", robjectVariableName)
					.Replace("{type}", type.FullName);
		}

		public string GetValueToStringCode(string objectVariableName)
		{
			if (!model.IsValueModel)
			{
				throw new InvalidOperationException("Only value models can have string to value conversion");
			}

			return ApiGenConfig.ValueToStringCodeTemplateExtractor.Extract(type)
					.Replace("{value}", objectVariableName);
		}
	}
}
