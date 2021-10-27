using Moq;
using Routine.Client;
using Routine.Core;
using Routine.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Routine.Test.Client.Stubs
{
    public class Async : IPerformer
    {
        public Rvariable Perform(Robject target, string operationName, params Rvariable[] parameters) =>
            target.PerformAsync(operationName, parameters).WaitAndGetResult();

        public void SetUp(Mock<IObjectService> mock,
            ReferenceData id,
            string operation,
            Expression<Func<Dictionary<string, ParameterValueData>, bool>> match,
            VariableData result
        ) => mock.Setup(os => os.DoAsync(id, operation, It.Is(match))).ReturnsAsync(result);
    }
}