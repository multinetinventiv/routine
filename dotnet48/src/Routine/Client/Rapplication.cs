using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core;
using Routine.Engine.Context;

namespace Routine.Client
{
	public class Rapplication
	{
		public IObjectService Service { get; private set; }

		public Rapplication(IObjectService service)
		{
			Service = service;
		}

		private readonly object typesLock = new object();
		private Dictionary<string, Rtype> types;
		private void FetchModelIfNecessary()
		{
			if (types != null) { return; }

			lock (typesLock)
			{
				if (types != null) { return; }

				types = Service.ApplicationModel.Models.Select(m => new Rtype(this, m)).ToDictionary(t => t.Id);

				foreach (var type in Types)
				{
					type.Load();
				}
			}
		}

		public Rtype this[string objectModelId]
		{
			get
			{
				FetchModelIfNecessary();

				Rtype result;
				if (!types.TryGetValue(objectModelId, out result))
				{
					throw new TypeNotFoundException(objectModelId);
				}

				return result;
			}
		}

		public List<Rtype> Types
		{
			get
			{
				FetchModelIfNecessary();

				return types.Values.ToList();
			}
		}

		public Rvariable NewVar<T>(string name, T value, string modelId)
		{
			return NewVar(name, value, o => o.ToString(), modelId);
		}

		public Rvariable NewVar<T>(string name, T value, Func<T, string> idExtractor, string modelId)
		{
			return NewVar(name, value, o => Get(o, idExtractor, modelId));
		}

		public Rvariable NewVar<T>(string name, T value, Func<T, Robject> robjectExtractor)
		{
			return NewVar(name, Get(value, robjectExtractor));
		}

		public Rvariable NewVar(string name, Robject robj)
		{
			return new Rvariable(name, robj);
		}

		public Rvariable NewVarList<T>(string name, IEnumerable<T> list, string modelId)
		{
			return NewVarList(name, list, o => o.ToString(), modelId);
		}

		public Rvariable NewVarList<T>(string name, IEnumerable<T> list, Func<T, string> idExtractor, string modelId)
		{
			return NewVarList(name, list, o => Get(o, idExtractor, modelId));
		}

		public Rvariable NewVarList<T>(string name, IEnumerable<T> list, Func<T, Robject> robjectExtractor)
		{
			return NewVarList(name, list.Select(o => Get(o, robjectExtractor)));
		}

		public Rvariable NewVarList(string name, params Robject[] list) { return NewVarList(name, list.AsEnumerable()); }
		public Rvariable NewVarList(string name, IEnumerable<Robject> list)
		{
			return new Rvariable(name, list);
		}

		public Rvariable NullVariable() { return new Rvariable(); }
		public Robject NullObject() { return new Robject(); }

		private Robject Get<T>(T value, Func<T, Robject> robjectExtractor)
		{
			object boxedValue = value;
			var robj = new Robject();
			if (boxedValue != null)
			{
				robj = robjectExtractor(value);
			}

			return robj;
		}

		private Robject Get<T>(T value, Func<T, string> idExtractor, string modelId)
		{
			object boxedValue = value;
			string id = null;
			if (boxedValue != null)
			{
				id = idExtractor(value);
			}

			return Get(id, modelId);
		}

		public Robject Get(string id, string modelId) { return Get(id, modelId, modelId); }
		public Robject Get(string id, string actualModelId, string viewModelId)
		{
			if (string.IsNullOrEmpty(actualModelId)) { throw new ArgumentNullException("actualModelId"); }
			if (string.IsNullOrEmpty(viewModelId)) { throw new ArgumentNullException("viewModelId", "If view model is not needed, pass the same as actualModelId or invoke Get(id, modelId)"); }

			return this[actualModelId].Get(id, this[viewModelId]);
		}

		public Robject Init(string modelId, params Rvariable[] initializationParameters) { return Init(modelId, initializationParameters.AsEnumerable()); }
		public Robject Init(string modelId, IEnumerable<Rvariable> initializationParameters)
		{
			return this[modelId].Init(initializationParameters);
		}

		#region Equality & Hashcode

		protected bool Equals(Rapplication other)
		{
			return Equals(Service.ApplicationModel, other.Service.ApplicationModel);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((Rapplication)obj);
		}

		public override int GetHashCode()
		{
			return (Service.ApplicationModel != null ? Service.ApplicationModel.GetHashCode() : 0);
		}

		#endregion
	}
}
