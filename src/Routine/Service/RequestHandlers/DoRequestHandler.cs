using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Routine.Core;
using Routine.Core.Rest;
using Routine.Engine.Context;
using Routine.Service.RequestHandlers.Exceptions;
using Routine.Service.RequestHandlers.Helper;

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
        protected override void Process()
        {
            var appModel = ApplicationModel;

            Dictionary<string, ParameterValueData> parameterValues;
            try
            {
                parameterValues = GetParameterDictionary()
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

            var variableData = ServiceContext.ObjectService.Do(resolution.Reference, resolution.OperationModel.Name, parameterValues);
            var compressor = new DataCompressor(appModel, resolution.OperationModel.Result.ViewModelId);

            WriteJsonResponse(compressor.Compress(variableData));
        }

        private IDictionary<string, object> GetParameterDictionary()
        {
            if (IsGet)
            {
                return QueryString.Keys.ToDictionary(s => s, s => QueryString[s] as object);
            }
            HttpContext.Request.EnableBuffering();

            var req = HttpContext.Request.Body;
            req.Seek(0, SeekOrigin.Begin);
            var requestBody = new StreamReader(req).ReadToEnd();

            return deserializeToDictionary(requestBody);
        }

        private Dictionary<string, object> deserializeToDictionary(string jo)
        {
            var values = JsonSerializer.Deserialize<Dictionary<string, object>>(jo);
            var values2 = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> d in values)
            {
                if (d.Value is JObject)
                {
                    values2.Add(d.Key, deserializeToDictionary(d.Value.ToString()));
                }
                else
                {
                    values2.Add(d.Key, d.Value);
                }
            }
            return values2;
        }
    }
}