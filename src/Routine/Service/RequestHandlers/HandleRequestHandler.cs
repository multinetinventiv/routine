using Microsoft.AspNetCore.Http;
using Routine.Core.Rest;
using Routine.Core;
using Routine.Service.RequestHandlers.Exceptions;
using Routine.Service.RequestHandlers.Helper;

namespace Routine.Service.RequestHandlers;

public class HandleRequestHandler : RequestHandlerBase
{
    private readonly Func<Resolution, IRequestHandler> actionFactory;

    public HandleRequestHandler(
        IServiceContext serviceContext,
        IJsonSerializer jsonSerializer,
        IHttpContextAccessor httpContextAccessor,
        Func<Resolution, IRequestHandler> actionFactory
    )
        : base(serviceContext, jsonSerializer, httpContextAccessor)
    {
        this.actionFactory = actionFactory;
    }

    public override async Task WriteResponse()
    {
        var pathItems = $"{HttpContext.Request.Path}".After($"{UrlBase}", StringComparison.OrdinalIgnoreCase).Trim('/').Split('/');

        await Handle(
            modelId: $"{(pathItems.Length > 0 ? pathItems[0] : null)}",
            idOrViewModelIdOrOperation: $"{(pathItems.Length > 1 ? pathItems[1] : null)}",
            viewModelIdOrOperation: $"{(pathItems.Length > 2 ? pathItems[2] : null)}",
            operation: $"{(pathItems.Length > 3 ? pathItems[3] : null)}"
        );
    }

    internal async Task Handle(string modelId, string idOrViewModelIdOrOperation, string viewModelIdOrOperation, string operation)
    {
        if (modelId == null) { throw new InvalidOperationException("Handle action does not handle request when modelId is null. Please check your route configuration, handle action should only be called when modelId is available."); }

        ObjectModel model;

        try
        {
            model = FindModel(modelId);
        }
        catch (AmbiguousModelException ex)
        {
            HttpContext.Response.SetStatus(StatusCodes.Status404NotFound,
                $"More than one model found with given modelId ({modelId}). " +
                $"Try sending full names. Available models are {string.Join(",", ex.AvailableModels.Select(om => om.Id))}."
            );

            return;
        }
        catch (ModelNotFoundException)
        {
            HttpContext.Response.SetStatus(StatusCodes.Status404NotFound,
                $"Could not resolve modelId or find an existing model from this modelId ({modelId}). " +
                "Make sure given modelId has a corresponding model and url is in one of the following format; " +
                "- serviceurlbase/modelId - serviceurlbase/modelId/id - serviceurlbase/modelId/operation " +
                "- serviceurlbase/modelId/viewModelId - serviceurlbase/modelId/id/operation " +
                "- serviceurlbase/modelId/id/viewModelId - serviceurlbase/modelId/id/viewModelId/operation"
            );

            return;
        }

        await actionFactory(Resolve(
            model: model,
            idOrViewModelIdOrOperation: idOrViewModelIdOrOperation,
            viewModelIdOrOperation: viewModelIdOrOperation,
            operation: operation
        )).WriteResponse();
    }

    private ObjectModel FindModel(string modelId)
    {
        var appModel = ServiceContext.ObjectService.ApplicationModel;

        if (appModel.Model.TryGetValue(modelId, out var result))
        {
            return result;
        }

        ModelIndex.TryGetValue(modelId.AfterLast("."), out var availableModels);

        if (availableModels == null)
        {
            throw new ModelNotFoundException(modelId);
        }

        if (availableModels.Count > 1)
        {
            throw new AmbiguousModelException(availableModels);
        }

        if (availableModels.Count <= 0)
        {
            throw new ModelNotFoundException(modelId);
        }

        return availableModels[0];
    }

    private Resolution Resolve(ObjectModel model, string idOrViewModelIdOrOperation, string viewModelIdOrOperation, string operation)
    {
        var appModel = ServiceContext.ObjectService.ApplicationModel;

        string id = null;
        var viewModelId = ViewModelId(model, viewModelIdOrOperation) ?? ViewModelId(model, idOrViewModelIdOrOperation) ?? model.Id; //view model is found by looking from right to left

        var viewModel = appModel.Model[viewModelId];

        // if operation is not null then idOrViewModelIdOrOperation must be id, since all of four parameters are given
        if (!string.IsNullOrWhiteSpace(operation))
        {
            id = idOrViewModelIdOrOperation;
        }
        // if operation is null then it may be given in other parameters
        else if (!string.IsNullOrWhiteSpace(viewModelIdOrOperation))
        {
            // if viewModel has an operation named exactly the same as viewModelIdOrOperation then it indicates an operation
            if (viewModel.Operation.ContainsKey(viewModelIdOrOperation))
            {
                operation = viewModelIdOrOperation;

                // if idOrViewModelIdOrOperation is not like viewModelId then it indicates id
                if (!viewModel.Id.EndsWith(idOrViewModelIdOrOperation))
                {
                    id = idOrViewModelIdOrOperation;
                }
            }
            // if viewModelIdOrOperation is not an operation then it must be viewModelId so idOrViewModelIdOrOperation must be id
            else
            {
                id = idOrViewModelIdOrOperation;
            }
        }
        // here we only have one parameter
        else if (!string.IsNullOrWhiteSpace(idOrViewModelIdOrOperation))
        {
            // if viewModel has an operation named exactly the as idOrViewModelIdOrOperation then it indicates an operation
            if (viewModel.Operation.ContainsKey(idOrViewModelIdOrOperation))
            {
                operation = idOrViewModelIdOrOperation;
            }
            // if idOrViewModelIdOrOperation is not like viewModelId then it indicates id
            else if (!viewModel.Id.EndsWith(idOrViewModelIdOrOperation))
            {
                id = idOrViewModelIdOrOperation;
            }

            //otherwise idOrViewModelIdOrOperation must have been set as viewmodelid, so here we don't have id or operation name
        }

        return new Resolution(appModel, model.Id, id, viewModelId, operation);
    }

    private static string ViewModelId(ObjectModel model, string viewModelIdOrOperation) =>
        string.IsNullOrWhiteSpace(viewModelIdOrOperation)
            ? null
            : model.Id == viewModelIdOrOperation
                ? model.Id
                : model.ViewModelIds.FirstOrDefault(vmid => vmid == viewModelIdOrOperation) ??
                  model.ViewModelIds.FirstOrDefault(vmid => vmid.EndsWith(viewModelIdOrOperation));
}
