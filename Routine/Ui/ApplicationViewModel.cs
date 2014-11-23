using System;
using Routine.Client;
using Routine.Core.Configuration;

namespace Routine.Ui
{
	public class ApplicationViewModel : ViewModelBase
	{
		private readonly Rapplication rapp;

		public ApplicationViewModel(Rapplication rapp, IMvcConfiguration configuration)
			: base(configuration)
		{
			this.rapp = rapp;
		}

		public ObjectViewModel Index
		{
			get
			{
				foreach (var model in rapp.Types)
				{
					try
					{
						var id = Configuration.GetIndexId(model);

						return new ObjectViewModel(Configuration, rapp.Get(id, model.Id));
					}
					catch (ConfigurationException) { }
				}

				throw new IndexPageNotFoundException();
			}
		}

		public ObjectViewModel Get(string id, string modelId)
		{
			return new ObjectViewModel(Configuration, rapp.Get(id, modelId));
		}

		public ObjectViewModel Get(string id, string actualModelId, string viewModelId)
		{
			return new ObjectViewModel(Configuration, rapp.Get(id, actualModelId, viewModelId));
		}
	}

	public class IndexPageNotFoundException : Exception { }
}

