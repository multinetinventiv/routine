using System.Collections.Generic;
using Routine.Core;
using Routine.Core.Configuration;

namespace Routine.Engine
{
	public class DomainObject
	{
		private readonly object target;
		private readonly DomainType actualDomainType;
		private readonly DomainType viewDomainType;

		public DomainObject(object target, DomainType actualDomainType, DomainType viewDomainType)
		{
			this.target = target;
			this.actualDomainType = actualDomainType;
			this.viewDomainType = viewDomainType;
		}

		public string GetId()
		{
			if (target == null || viewDomainType.IdExtractor == null)
			{
				if (actualDomainType.IdExtractor == null)
				{
					throw new ConfigurationException("Id", target);
				}

				return actualDomainType.IdExtractor.GetId(target);
			}

			return viewDomainType.IdExtractor.GetId(target);
		}

		public string GetValue()
		{
			if (viewDomainType.IsValueModel)
			{
				return GetId();
			}

			if (target == null || viewDomainType.ValueExtractor == null)
			{
				if (actualDomainType.ValueExtractor == null)
				{
					throw new ConfigurationException("Value", target);
				}

				return actualDomainType.ValueExtractor.GetValue(target);
			}

			return viewDomainType.ValueExtractor.GetValue(target);
		}

		public ObjectReferenceData GetReferenceData()
		{
			var result = new ObjectReferenceData();

			result.IsNull = target == null;
			result.ActualModelId = actualDomainType.Id;
			result.ViewModelId = viewDomainType.Id;
			result.Id = GetId();

			return result;
		}

		public ObjectData GetObjectData(bool eager)
		{
			var result = new ObjectData
			{
				Reference = GetReferenceData(),
				Value = GetValue()
			};

			if (target == null) { return result; }
			if (!eager) { return result; }

			foreach (var member in viewDomainType.Members)
			{
				result.Members.Add(member.Id, member.CreateData(target));
			}

			return result;
		}

		public ValueData GetMemberData(string memberModelId)
		{
			DomainMember member;
			if (!viewDomainType.Member.TryGetValue(memberModelId, out member))
			{
				throw new MemberDoesNotExistException(viewDomainType.Id, memberModelId);
			}

			return member.CreateData(target, true);
		}

		public ValueData Perform(string operationModelId, Dictionary<string, ParameterValueData> parameterValues)
		{
			DomainOperation operation;
			if (!viewDomainType.Operation.TryGetValue(operationModelId, out operation))
			{
				throw new OperationDoesNotExistException(viewDomainType.Id, operationModelId);
			}

			return operation.Perform(target, parameterValues);
		}
	}
}

