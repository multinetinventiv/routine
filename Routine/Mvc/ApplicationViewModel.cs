using Routine.Api;
using System.Collections.Generic;
using System.Linq;
using Routine.Core;
using System;

namespace Routine.Mvc
{
	public class ApplicationViewModel : ViewModelBase
	{
		private readonly Rapplication rapp;

		public ApplicationViewModel(Rapplication rapp, IMvcConfiguration mvcConfig, IFactory factory)
			: base(mvcConfig, factory)
		{
			this.rapp = rapp;
		}

		public ObjectViewModel Index
		{
			get
			{
				foreach(var type in rapp.ObjectModels)
				{
					try
					{
						string id = MvcConfig.IndexIdExtractor.Extract(type);

						return CreateObject().With(rapp.Get(id, type.Id));
					}
					catch(CannotExtractException) { continue; }
				}

				throw new IndexObjectNotFoundException();
			}
		}

		public MenuViewModel Menu
		{
			get
			{
				var menuObjs = new List<Robject>();
				foreach(var type in rapp.ObjectModels)
				{
					menuObjs.AddRange(
						MvcConfig.MenuIdsExtractor
						.Extract(type)
						.Select(id => rapp.Get(id, type.Id))
					);
				}
				return CreateMenu().With(menuObjs);
			}
		}

		public ObjectViewModel Get(string id, string modelId) 
		{
			return CreateObject().With(rapp.Get(id, modelId));
		}

		public ObjectViewModel Get(string id, string actualModelId, string viewModelId) 
		{
			return CreateObject().With(rapp.Get(id, actualModelId, viewModelId));
		}
	}

	public class IndexObjectNotFoundException : Exception {}
}

