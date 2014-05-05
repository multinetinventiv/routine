using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core;

namespace Routine.Api
{
	public class Rapplication
	{
		private readonly IApiContext context;

		public Rapplication(IApiContext context)
		{
            this.context = context;
		}

		private readonly object modelLock = new object();
		private ApplicationModel model;
		private ObjectModelIndex modelIndex;
		private void FetchModelIfNecessary()
		{
			if(model == null)
			{
				lock(modelLock)
				{
					if(model == null)
					{
						model = context.ObjectService.GetApplicationModel();
						modelIndex = ObjectModelIndex.Build(model.Models, omid => new ObjectModel{
							Id = omid,
							IsValueModel = true,
							IsViewModel = false
						});
					}
				}
			}
		}
		public ApplicationModel Model{get{FetchModelIfNecessary(); return model;}}
		public ObjectModelIndex ObjectModel{get{FetchModelIfNecessary(); return modelIndex;}}
		public List<ObjectModel> ObjectModels { get { return Model.Models; } }

		public Rvariable NewVar<T>(string name, T value, string modelId)
		{
			return NewVar(name, value, o => o.ToString(), modelId);
		}

		public Rvariable NewVar<T>(string name, T value, Func<T, string> idExtractor, string modelId)
		{
			return NewVar(name, Get(value, idExtractor, modelId));
		}

		public Rvariable NewVar(string name, Robject robj)
		{
			return context.CreateRvariable().WithSingle(name, robj);
		}

		public Rvariable NewVarList<T>(string name, IEnumerable<T> list, string modelId)
		{
			return NewVarList(name, list, o => o.ToString(), modelId);
		}

		public Rvariable NewVarList<T>(string name, IEnumerable<T> list, Func<T, string> idExtractor, string modelId)
		{
			return NewVarList(name, list.Select(o => Get(o, idExtractor, modelId)));
		}

		public Rvariable NewVarList(string name, IEnumerable<Robject> list)
		{
			return context.CreateRvariable().WithList(name, list);
		}

		private Robject Get<T>(T value, Func<T, string> idExtractor, string modelId)
		{
			if(object.Equals(value, default(T)))
			{
                return context.CreateRobject().Null();
			}

			return Get(idExtractor(value), modelId);
		}

		public Robject Get(string id, string modelId)
		{
			return context.CreateRobject().With(id, modelId);
		}

		public Robject Get(string id, string actualModelId, string viewModelId)
		{
			return context.CreateRobject().With(id, actualModelId, viewModelId);
		}

		public List<Robject> GetAvailableObjects(string modelId)
		{
			return context.ObjectService
				.GetAvailableObjects(modelId)
				.Select(v => context.CreateRobject().With(v.Reference, v.Value))
				.ToList();
		}

		public class ObjectModelIndex
		{
			internal static ObjectModelIndex Build(IEnumerable<ObjectModel> models, Func<string, ObjectModel> defaultValueDelegate)
			{
				var result = new ObjectModelIndex(defaultValueDelegate);

				foreach(var model in models)
				{
					result.index.Add(model.Id, model);
				}

				return result;
			}

			private readonly Func<string, ObjectModel> defaultValueDelegate;
			private readonly Dictionary<string, ObjectModel> index;

			private ObjectModelIndex(Func<string, ObjectModel> defaultValueDelegate)
			{
				this.defaultValueDelegate = defaultValueDelegate;
				this.index = new Dictionary<string, ObjectModel>();
			}

			public ObjectModel this[string objectModelId]
			{
				get
				{
					ObjectModel result;
					if(!index.TryGetValue(objectModelId, out result))
					{
						result = defaultValueDelegate(objectModelId);
					}

					return result;
				}
			}
		}
	}
}
