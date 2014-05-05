using System.Collections.Generic;
using System.Linq;
using Routine.Core;
using Routine.Core.Api;

namespace Routine.Api
{
	public class ApplicationCodeModel : CodeModelBase
	{
		private readonly Rapplication rapp;

		public ApplicationCodeModel(Rapplication rapp, IApiGenerationContext context)
			: base(context)
		{
			this.rapp = rapp;
		}

		public string ApiName { get { return ApiGenConfig.ApiName; } }
		public bool ApiExists { get { return !string.IsNullOrEmpty(ApiName); } }

		public List<string> FriendlyAssemblyNames { get { return ApiGenConfig.FriendlyAssemblyNames; } }
		public List<ObjectCodeModel> Models 
		{ 
			get 
			{ 
				return rapp.Model.Models
					.Where(m => !m.IsValueModel && ApiGenConfig.Modules.IsModuleIncluded(m.Module))
					.Select(m => CreateObject().With(m))
					.ToList(); 
			} 
		}

		protected override bool ModelCanBeUsed(string modelId)
		{
			try
			{
				ApiGenConfig.ReferencedModelIdSerializer.Deserialize(modelId);
				return true;
			}
			catch (CannotDeserializeException)
			{
				return ApiGenConfig.Modules.IsModuleIncluded(modelId);
			}
		}
	}
}
