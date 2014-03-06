using System.Collections.Generic;
using System.Linq;
using Routine.Core.Service;
using Routine.Core.Service.Impl;

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

		public ObjectReferenceData GetReference()
		{
			return reference;
		}

		public string GetValue()
		{
			return ctx.GetValue(target, domainType, reference.Id);
		}

		public SingleValueData GetSingleValue()
		{
			return new SingleValueData {
				Reference = GetReference(),
				Value = GetValue()
			};
		}

		public ObjectData GetObject()
		{
			return new ObjectData {
				Reference = reference,
				Value = GetValue(),
				Members = domainType.LightMembers.Select(m => m.CreateData(target)).ToList(),
				Operations = domainType.LightOperations.Select(o => o.CreateData(target)).ToList()
			};
		}
	
		public MemberData GetMember(string memberModelId)
		{
			DomainMember member;
			if(!domainType.Member.TryGetValue(memberModelId, out member))
			{
				throw new MemberDoesNotExistException(domainType.Id, memberModelId);
			}

			return member.CreateData(target);
		}

		public OperationData GetOperation(string operationModelId)
		{
			DomainOperation operation;
			if(!domainType.Operation.TryGetValue(operationModelId, out operation))
			{
				throw new OperationDoesNotExistException(domainType.Id, operationModelId);
			}

			return operation.CreateData(target);
		}

		public ResultData Perform(string operationModelId, List<ParameterValueData> parameterValues)
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

