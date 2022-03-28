using Moq;
using Routine.Client;
using Routine.Core;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;

namespace Routine.Test.Client.Stubs
{
    public interface IPerformer
    {
        Rvariable Perform(Robject target, string operationName, params Rvariable[] parameters);

        public void SetUp(Mock<IObjectService> mock, ReferenceData id, string operation, VariableData result) => SetUp(mock, id, operation, parameters => true, result);
        void SetUp(Mock<IObjectService> mock, ReferenceData id, string operation, Expression<Func<Dictionary<string, ParameterValueData>, bool>> match, VariableData result);
    }
}
