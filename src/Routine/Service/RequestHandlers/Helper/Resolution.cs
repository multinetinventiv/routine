using Routine.Core;

namespace Routine.Service.RequestHandlers.Helper;

public class Resolution
{
    private readonly ApplicationModel _appModel;
    private readonly string _modelId;
    private readonly string _id;
    private readonly string _viewModelId;
    private readonly string _operation;

    public Resolution(ApplicationModel appModel, string modelId, string id, string viewModelId, string operation)
    {
        _appModel = appModel;
        _modelId = modelId;
        _id = id;
        _viewModelId = viewModelId;
        _operation = operation;
    }

    public ReferenceData Reference => new() { Id = _id, ModelId = _modelId, ViewModelId = _viewModelId };
    public ObjectModel Model => _appModel.Model[Reference.ModelId];
    public ObjectModel ViewModel => _appModel.Model[Reference.ViewModelId];
    public OperationModel OperationModel => ViewModel.Operation[_operation];

    public bool HasOperation => !string.IsNullOrWhiteSpace(_operation);
}
