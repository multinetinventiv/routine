using System;
using System.Collections.Generic;
using Routine.Core;

namespace Routine.Engine
{
	public class DomainObject
	{
		private readonly DomainType actualDomainType;
		private readonly DomainType viewDomainType;
		private readonly object actualTarget;
		private readonly object viewTarget;

		public DomainObject(object target, DomainType actualDomainType, DomainType viewDomainType)
		{
			if (actualDomainType == null) { throw new ArgumentNullException("actualDomainType"); }
			if (viewDomainType == null) { throw new ArgumentNullException("viewDomainType"); }

			this.actualDomainType = actualDomainType;
			this.viewDomainType = viewDomainType;

			actualTarget = target;
			viewTarget = actualDomainType.Convert(target, viewDomainType);
		}

		public string GetId()
		{
			if (actualDomainType.IdExtractor == null)
			{
				return string.Empty;
			}

			return actualDomainType.IdExtractor.GetId(actualTarget);
		}

		public string GetValue()
		{
			if (viewDomainType.IsValueModel)
			{
				return GetId();
			}

			if (actualTarget == null || viewDomainType.ValueExtractor == null)
			{
				if (actualDomainType.ValueExtractor == null)
				{
					return string.Empty;
				}

				return actualDomainType.ValueExtractor.GetValue(actualTarget);
			}

			return viewDomainType.ValueExtractor.GetValue(viewTarget);
		}

		public ObjectReferenceData GetReferenceData()
		{
			var result = new ObjectReferenceData();

			result.IsNull = actualTarget == null;
			result.ActualModelId = actualDomainType.Id;
			result.ViewModelId = viewDomainType.Id;
			result.Id = GetId();

			return result;
		}

		public ObjectData GetObjectData(bool eager) { return GetObjectData(Constants.FIRST_DEPTH, eager); }
		internal ObjectData GetObjectData(int currentDepth, bool eager)
		{
			var result = new ObjectData
			{
				Reference = GetReferenceData(),
				Value = GetValue()
			};

			if (actualTarget == null) { return result; }
			if (!eager && actualDomainType.Locatable) { return result; }

			if (currentDepth > actualDomainType.MaxFetchDepth)
			{
				throw new MaxFetchDepthExceededException(actualDomainType.MaxFetchDepth, actualTarget);
			}

			foreach (var member in viewDomainType.Members)
			{
				result.Members.Add(member.Id, member.CreateData(viewTarget, currentDepth + 1));
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

			return member.CreateData(viewTarget, true);
		}

		public ValueData Perform(string operationModelId, Dictionary<string, ParameterValueData> parameterValues)
		{
			DomainOperation operation;
			if (!viewDomainType.Operation.TryGetValue(operationModelId, out operation))
			{
				throw new OperationDoesNotExistException(viewDomainType.Id, operationModelId);
			}

			return operation.Perform(viewTarget, parameterValues);
		}
	}
}

