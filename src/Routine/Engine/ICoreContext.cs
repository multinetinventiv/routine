using Routine.Core;
using System.Collections.Generic;
using System.Collections;

namespace Routine.Engine
{
    public interface IDomainObjectFactory
    {
        DomainObject CreateDomainObject(DomainType type, string id);
    }

    public interface ICoreContext
    {
        ICodingStyle CodingStyle { get; }
        DomainType GetDomainType(string typeId);
        DomainType GetDomainType(IType type);
        List<DomainType> GetDomainTypes();

        object GetObject(ReferenceData aReference);

        DomainObject CreateDomainObject(object @object, DomainType viewDomainType);
        DomainObject CreateDomainObject(ReferenceData reference);

        public DomainObject CreateDomainObject(string id, string modelId) => CreateDomainObject(id, modelId, modelId);
        public DomainObject CreateDomainObject(string id, string modelId, string viewModelId) =>
            CreateDomainObject(new ReferenceData
            {
                Id = id,
                ModelId = modelId,
                ViewModelId = viewModelId
            });

        public DomainObject CreateDomainObject(object anObject) => CreateDomainObject(anObject, null);

        internal VariableData CreateValueData(object anObject, bool isList, DomainType viewDomainType, bool eager) => CreateValueData(anObject, isList, viewDomainType, Constants.FIRST_DEPTH, eager);
        internal VariableData CreateValueData(object anObject, bool isList, DomainType viewDomainType, int currentDepth, bool eager)
        {
            var result = new VariableData { IsList = isList };

            if (anObject == null) { return result; }

            if (isList)
            {
                if (anObject is not ICollection list) { return result; }

                foreach (var item in list)
                {
                    result.Values.Add(CreateDomainObject(item, viewDomainType).GetObjectData(currentDepth, eager));
                }
            }
            else
            {
                result.Values.Add(CreateDomainObject(anObject, viewDomainType).GetObjectData(currentDepth, eager));
            }

            return result;
        }

        internal string BuildTypeId(string module, string name) => string.IsNullOrEmpty(module) ? name : $"{module}.{name}";
    }
}
