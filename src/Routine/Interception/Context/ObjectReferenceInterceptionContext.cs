using Routine.Core;

namespace Routine.Interception.Context;

public class ObjectReferenceInterceptionContext : InterceptionContext
{
    protected readonly IObjectService _objectService;

    public ObjectReferenceInterceptionContext(string target, IObjectService objectService, ReferenceData targetReference)
        : base(target)
    {
        _objectService = objectService;

        TargetReference = targetReference;
    }

    public ReferenceData TargetReference
    {
        get => this[nameof(TargetReference)] as ReferenceData;
        set => this[nameof(TargetReference)] = value;
    }

    public ObjectModel Model => _objectService.ApplicationModel.Model[TargetReference.ModelId];
    public ObjectModel ViewModel => _objectService.ApplicationModel.Model[TargetReference.ViewModelId];

    public ObjectModel TargetModel => TargetReference.ViewModelId == null ? Model : ViewModel;
}
