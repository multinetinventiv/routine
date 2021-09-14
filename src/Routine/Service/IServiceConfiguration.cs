using System;
using System.Collections.Generic;
using Routine.Core;

namespace Routine.Service
{
    public interface IServiceConfiguration
    {
        string GetRootPath();

        bool GetAllowGet(ObjectModel objectModel, OperationModel operationModel);

        List<string> GetRequestHeaders();
        List<IHeaderProcessor> GetRequestHeaderProcessors();

        List<string> GetResponseHeaders();
        string GetResponseHeaderValue(string responseHeader);

        ExceptionResult GetExceptionResult(Exception exception);

        public string GetPath() => GetPath(string.Empty);
        public string GetPath(string path)
        {
            var rootPath = GetRootPath() ?? string.Empty;

            if (rootPath.StartsWith("/"))
            {
                rootPath = rootPath.After("/");
            }

            if (!string.IsNullOrEmpty(rootPath) && !rootPath.EndsWith("/"))
            {
                rootPath += "/";
            }

            if (path.StartsWith("/"))
            {
                path = path.After("/");
            }

            return rootPath + path;
        }
    }
}

