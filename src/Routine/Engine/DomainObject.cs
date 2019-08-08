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

		public string GetDisplay()
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

		public ReferenceData GetReferenceData()
		{
			if (actualTarget == null)
			{
				return null;
			}

			var result = new ReferenceData();

			result.ModelId = actualDomainType.Id;
			result.ViewModelId = viewDomainType.Id;
			result.Id = GetId();

			return result;
		}

		public ObjectData GetObjectData(bool eager) { return GetObjectData(Constants.FIRST_DEPTH, eager); }
		internal ObjectData GetObjectData(int currentDepth, bool eager)
		{
			if (actualTarget == null)
			{
				return null;
			}

			var result = new ObjectData
			{
				Id = GetId(),
				ModelId = actualDomainType.Id,
				Display = GetDisplay()
			};

			if (actualTarget == null) { return result; }
			if (!eager && actualDomainType.Locatable) { return result; }

			if (currentDepth > actualDomainType.MaxFetchDepth)
			{
				throw new MaxFetchDepthExceededException(actualDomainType.MaxFetchDepth, actualTarget);
			}

			foreach (var data in viewDomainType.Datas)
			{
				result.Data.Add(data.Name, data.CreateData(viewTarget, currentDepth + 1));
			}

			return result;
		}

		public VariableData GetData(string dataName)
		{
			DomainData data;
			if (!viewDomainType.Data.TryGetValue(dataName, out data))
			{
				throw new DataDoesNotExistException(viewDomainType.Id, dataName);
			}

			return data.CreateData(viewTarget, true);
		}

		public VariableData Perform(string operationName, Dictionary<string, ParameterValueData> parameterValues)
		{
			DomainOperation operation;
			if (!viewDomainType.Operation.TryGetValue(operationName, out operation))
			{
				throw new OperationDoesNotExistException(viewDomainType.Id, operationName);
			}

			return operation.Perform(viewTarget, parameterValues);
		}
	}
}

