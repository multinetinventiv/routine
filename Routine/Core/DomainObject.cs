using System.Collections.Generic;
using System.Linq;

namespace Routine.Core
{
	public class DomainObject
	{
		private readonly ICoreContext ctx;

		public DomainObject(ICoreContext coreContext)
		{
			this.ctx = coreContext;
		}

		private ObjectReferenceData reference;
		private object target;
		private DomainType domainType;

		internal DomainObject For(ObjectReferenceData reference)
		{
			this.reference = reference;

			target = ctx.Locate(reference);
			domainType = ctx.GetDomainType(reference.ViewModelId);

			return this;
		}

		internal DomainObject For(object target, string viewModelId)
		{
			this.target = target;

			domainType = ctx.GetDomainType(viewModelId);
			reference = ctx.CreateReferenceData(target, viewModelId);

			return this;
		}

		public ObjectReferenceData GetReference()
		{
			return reference;
		}

		public string GetValue()
		{
			return ctx.GetValue(target, domainType, reference.Id);
		}

		public ObjectData GetSingleValue()
		{
			return new ObjectData {
				Reference = GetReference(),
				Value = GetValue()
			};
		}

		public ObjectData GetObject()
		{
			var result = GetSingleValue();

			if (target == null)
			{
				return result;
			}

			foreach (var member in domainType.Members)
			{
				result.Members.Add(member.Id, member.CreateData(target));
			}

			return result;
		}
	
		public ValueData GetMember(string memberModelId)
		{
			DomainMember member;
			if(!domainType.Member.TryGetValue(memberModelId, out member))
			{
				throw new MemberDoesNotExistException(domainType.Id, memberModelId);
			}

			return member.CreateData(target, true);
		}

		public ValueData Perform(string operationModelId, Dictionary<string, ParameterValueData> parameterValues)
		{
			DomainOperation operation;
			if(!domainType.Operation.TryGetValue(operationModelId, out operation))
			{
				throw new OperationDoesNotExistException(domainType.Id, operationModelId);
			}

			return operation.Perform(target, parameterValues);
		}
	}
}

