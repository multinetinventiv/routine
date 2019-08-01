using System;

namespace Routine.Service.RequestHandlers.Exceptions
{
	public class ModelNotFoundException : Exception
	{
		public string ModelId { get; }

		public ModelNotFoundException(string modelId)
		{
			ModelId = modelId;
		}
	}
}