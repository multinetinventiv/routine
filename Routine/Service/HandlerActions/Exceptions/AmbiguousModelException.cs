using System;
using System.Collections.Generic;
using Routine.Core;

namespace Routine.Service.HandlerActions.Exceptions
{
	public class AmbiguousModelException : Exception
	{
		public List<ObjectModel> AvailableModels { get; }

		public AmbiguousModelException(List<ObjectModel> availableModels)
		{
			AvailableModels = availableModels;
		}
	}
}