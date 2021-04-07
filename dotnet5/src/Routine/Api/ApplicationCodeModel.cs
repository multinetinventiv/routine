using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Client;
using Routine.Core.Configuration;

namespace Routine.Api
{
	public class ApplicationCodeModel
	{
		private readonly List<TypeCodeModel> models;
		private readonly Dictionary<Rtype, TypeCodeModel> modelCache;

		public Rapplication Application { get; private set; }
		internal IApiConfiguration Configuration { get; private set; }

		public ApplicationCodeModel(Rapplication rapp, IApiConfiguration config)
		{
			Application = rapp;
			Configuration = config;

			models = rapp.Types
				.Where(t => !t.IsVoid && config.IsRendered(t))
				.Select(m => new TypeCodeModel(this, m))
				.ToList();

			modelCache = models.ToDictionary(m => m.Type);

			foreach (var model in models)
			{
				model.Load();
			}
		}

		public string DefaultNamespace { get { return Configuration.GetDefaultNamespace(); } }

		public List<TypeCodeModel> Models { get { return models; } }

		public TypeCodeModel GetModel(Rtype type) { return GetModel(type, false); }
		public TypeCodeModel GetModel(Rtype type, bool isList)
		{
			if (!modelCache.ContainsKey(type))
			{
				var clientType = Configuration.GetReferencedType(type);
				if (clientType == null)
				{
					throw new InvalidOperationException(string.Format("ReferencedType cannot be null for {0}", type));
				}

				var result = new TypeCodeModel(this, type, clientType);

				lock (models)
				{
					if (!modelCache.ContainsKey(type))
					{
						modelCache.Add(type, result);
					}
				}

				result.Load();
			}

			if (isList)
			{
				return modelCache[type].GetListType();
			}

			return modelCache[type];
		}

		public TypeCodeModel GetVoidModel()
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
				return Configuration.GetReferencedType(type) != null;
			}
			catch (ConfigurationException)
			{
				return false;
			}
		}

		#region Equality & Hashcode

		protected bool Equals(ApplicationCodeModel other)
		{
			return Equals(Application, other.Application);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((ApplicationCodeModel)obj);
		}

		public override int GetHashCode()
		{
			return (Application != null ? Application.GetHashCode() : 0);
		}

		#endregion
	}
}
