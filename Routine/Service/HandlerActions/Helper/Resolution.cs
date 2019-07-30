using Routine.Core;

namespace Routine.Service.HandlerActions.Helper
{
	public class Resolution
	{
		private readonly ApplicationModel appModel;
		private readonly string modelId;
		private readonly string id;
		private readonly string viewModelId;
		private readonly string operation;

		public Resolution(ApplicationModel appModel, string modelId, string id, string viewModelId, string operation)
		{
			this.appModel = appModel;
			this.modelId = modelId;
			this.id = id;
			this.viewModelId = viewModelId;
			this.operation = operation;
		}

		public ReferenceData Reference => new ReferenceData { Id = id, ModelId = modelId, ViewModelId = viewModelId };
		public ObjectModel Model => appModel.Model[Reference.ModelId];
		public ObjectModel ViewModel => appModel.Model[Reference.ViewModelId];
		public OperationModel OperationModel => ViewModel.Operation[operation];

		public bool HasOperation => !string.IsNullOrWhiteSpace(operation);
	}

	
}