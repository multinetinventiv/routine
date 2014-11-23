using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Client;
using Routine.Core.Configuration;

namespace Routine.Api
{
	public class ApplicationCodeModel
	{
		private readonly IApiGenerationConfiguration config;
		private readonly List<ObjectCodeModel> models;
		private readonly Dictionary<Rtype, ObjectCodeModel> modelCache;

		public Rapplication Application { get; private set; }

		public ApplicationCodeModel(Rapplication rapp, IApiGenerationConfiguration config)
		{
			Application = rapp;

			this.config = config;

			models = rapp.Types
				.Where(t => !t.IsValueType && config.IsRendered(t))
				.Select(m => new ObjectCodeModel(this, m, config.GetDefaultNamespace()))
				.ToList();

			modelCache = models.ToDictionary(m => m.Type);

			foreach (var model in models)
			{
				model.Load();
			}
		}

		public string ApiName { get { return config.GetApiName(); } }
		public bool ApiExists { get { return !string.IsNullOrEmpty(ApiName); } }
		public string DefaultNamespace { get { return config.GetDefaultNamespace(); } }

		public List<string> FriendlyAssemblyNames { get { return config.GetFriendlyAssemblyNames(); } }
		public List<ObjectCodeModel> Models { get { return models; } }

		public ObjectCodeModel GetModel(Rtype type) { return GetModel(type, false); }
		public ObjectCodeModel GetModel(Rtype type, bool isList)
		{
			if (!modelCache.ContainsKey(type))
			{
				CreateModel(type);
			}

			if (isList)
			{
				return modelCache[type].GetListType();
			}

			return modelCache[type];
		}

		private void CreateModel(Rtype type)
		{
			ObjectCodeModel result;

			if (type.IsVoid)
			{
				result = new ObjectCodeModel(this, type);
			}
			else
			{
				var clientType = config.GetReferencedType(type);

				if (clientType == null)
				{
					throw new InvalidOperationException(string.Format("No model or referenced type found with given model id: {0}", type));
				}

				if (config.GetReferencedTypeIsValueType(clientType))
				{
					clientType = config.GetTargetValueType(clientType) ?? clientType;

					result = new ObjectCodeModel(this, type, clientType, config.GetStringToValueCodeTemplate(clientType),
						config.GetValueToStringCodeTemplate(clientType));
				}
				else
				{
					result = new ObjectCodeModel(this, type, clientType);
				}
			}

			result.Load();

			lock (models)
			{
				if (!modelCache.ContainsKey(type))
				{
					modelCache.Add(type, result);
				}
			}
		}

		public ObjectCodeModel GetVoidModel()
		{
			return GetModel(Rtype.Void, false);
		}

		public bool ValidateType(Rtype type)
		{
			if (modelCache.ContainsKey(type))
			{
				return true;
			}

			try
			{
				return config.GetReferencedType(type) != null;
			}
			catch (ConfigurationException)
			{
				if (!type.IsValueType)
				{
					Console.WriteLine("Type '{0}' was not included so ignored", type);

					return false;
				}

				if (config.GetIgnoreReferencedTypeNotFound())
				{
					Console.WriteLine("Referenced type for '{0}' was not found and ignored", type);

					return false;
				}

				throw;
			}
		}

		internal bool IsRendered(Rtype type)
		{
			return config.IsRendered(type);
		}

		internal bool IsRendered(Rinitializer initializer)
		{
			return config.IsRendered(initializer);
		}

		internal bool IsRendered(Rmember member)
		{
			return config.IsRendered(member);
		}

		internal bool IsRendered(Roperation operation)
		{
			return config.IsRendered(operation);
		}

		public List<string> GetStaticInstanceIds(ObjectCodeModel objectCodeModel)
		{
			return config.GetStaticInstanceIds(objectCodeModel);
		}
	}
}
