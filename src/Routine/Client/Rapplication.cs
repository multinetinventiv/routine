using Routine.Core;
using Routine.Engine.Context;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Routine.Client
{
    public class Rapplication
    {
        public IObjectService Service { get; }

        public Rapplication(IObjectService service)
        {
            Service = service;
        }

        private readonly object typesLock = new();
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

                if (!types.TryGetValue(objectModelId, out var result))
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

        public Rvariable NewVar<T>(string name, T value, string modelId) => NewVar(name, value, o => o.ToString(), modelId);
        public Rvariable NewVar<T>(string name, T value, Func<T, string> idExtractor, string modelId) => NewVar(name, value, o => Get(o, idExtractor, modelId));
        public Rvariable NewVar<T>(string name, T value, Func<T, Robject> robjectExtractor) => NewVar(name, Get(value, robjectExtractor));
        public Rvariable NewVar(string name, Robject robj) => new(name, robj);

        public Rvariable NewVarList<T>(string name, IEnumerable<T> list, string modelId) => NewVarList(name, list, o => o.ToString(), modelId);
        public Rvariable NewVarList<T>(string name, IEnumerable<T> list, Func<T, string> idExtractor, string modelId) => NewVarList(name, list, o => Get(o, idExtractor, modelId));
        public Rvariable NewVarList<T>(string name, IEnumerable<T> list, Func<T, Robject> robjectExtractor) => NewVarList(name, list.Select(o => Get(o, robjectExtractor)));
        public Rvariable NewVarList(string name, params Robject[] list) => NewVarList(name, list.AsEnumerable());
        public Rvariable NewVarList(string name, IEnumerable<Robject> list) => new(name, list);

        public Rvariable NullVariable() => new();
        public Robject NullObject() => new();

        private static Robject Get<T>(T value, Func<T, Robject> robjectExtractor)
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

        public Robject Get(string id, string modelId) => Get(id, modelId, modelId);
        public Robject Get(string id, string actualModelId, string viewModelId)
        {
            if (string.IsNullOrEmpty(actualModelId)) { throw new ArgumentNullException(nameof(actualModelId)); }
            if (string.IsNullOrEmpty(viewModelId)) { throw new ArgumentNullException(nameof(viewModelId), "If view model is not needed, pass the same as actualModelId or invoke Get(id, modelId)"); }

            return this[actualModelId].Get(id, this[viewModelId]);
        }

        public Robject Init(string modelId, params Rvariable[] initializationParameters) => Init(modelId, initializationParameters.AsEnumerable());
        public Robject Init(string modelId, IEnumerable<Rvariable> initializationParameters) => this[modelId].Init(initializationParameters);

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
