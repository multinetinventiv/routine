using Microsoft.AspNetCore.Http;
using Routine.Core;
using Routine.Core.Rest;
using Routine.Engine.Context;
using Routine.Service.RequestHandlers.Exceptions;
using Routine.Service.RequestHandlers.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.Service.RequestHandlers
{
    public class DoRequestHandler : ObjectServiceRequestHandlerBase
    {
        private readonly Resolution resolution;

        public DoRequestHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, IHttpContextAccessor httpContextAccessor, Resolution resolution)
            : base(serviceContext, jsonSerializer, httpContextAccessor)
        {
            this.resolution = resolution;
        }

        protected override bool AllowGet => ServiceContext.ServiceConfiguration.GetAllowGet(resolution.ViewModel, resolution.OperationModel);
        protected override async Task Process()
        {
            var appModel = ApplicationModel;

            Dictionary<string, ParameterValueData> parameterValues;
            try
            {
                parameterValues = (await GetParameterDictionary())
                    .Where(kvp => resolution.OperationModel.Parameter.ContainsKey(kvp.Key))
                    .ToDictionary(kvp => kvp.Key, kvp =>
                        new DataCompressor(appModel, resolution.OperationModel.Parameter[kvp.Key].ViewModelId)
                            .DecompressParameterValueData(kvp.Value)
                    );

            }
            catch (TypeNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex);
            }

            var variableData = await ServiceContext.ObjectService.DoAsync(resolution.Reference, resolution.OperationModel.Name, parameterValues);
            var compressor = new DataCompressor(appModel, resolution.OperationModel.Result.ViewModelId);

            await WriteJsonResponse(compressor.Compress(variableData));
        }

        private async Task<IDictionary<string, object>> GetParameterDictionary()
        {
            if (IsGet)
            {
                return QueryString.Keys.ToDictionary(s => s, s => QueryString[s] as object);
            }

            HttpContext.Request.EnableBuffering();
            var req = HttpContext.Request.Body;
            req.Seek(0, SeekOrigin.Begin);
            var requestBody = await new StreamReader(req).ReadToEndAsync();

            return string.IsNullOrWhiteSpace(requestBody)
                ? new Dictionary<string, object>()
                : JsonSerializer.Deserialize<Dictionary<string, object>>(requestBody);
        }
    }
}