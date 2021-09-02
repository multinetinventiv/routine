using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Moq;
using Moq.Language.Flow;
using NUnit.Framework;
using Routine.Client;
using Routine.Client.Context;
using Routine.Core;
using Routine.Test.Core;

namespace Routine.Test.Client
{
    public abstract class ClientTestBase : CoreTestBase
    {
        protected Mock<IObjectService> objectServiceMock;

        protected IClientContext ctx;
        protected Rapplication testingRapplication;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            objectServiceMock = new Mock<IObjectService>();

            var clientContext = new DefaultClientContext(objectServiceMock.Object, new Rapplication(objectServiceMock.Object));

            ctx = clientContext;
            testingRapplication = clientContext.Application;

            objectServiceMock.Setup(o => o.ApplicationModel)
                .Returns(GetApplicationModel);
            objectServiceMock.Setup(o => o.Get(It.IsAny<ReferenceData>()))
                .Returns((ReferenceData ord) => ObjectData(ord));
            objectServiceMock.Setup(o => o.Do(It.IsAny<ReferenceData>(), It.IsAny<string>(), It.IsAny<Dictionary<string, ParameterValueData>>()))
                .Returns(Void());

            ModelsAre(Model());
        }

        private ObjectData ObjectData(ReferenceData ord) => objectDictionary[ord];

        #region Stubbers

        protected ObjectStubber When(ReferenceData referenceData) => new(objectServiceMock, referenceData);

        protected class ObjectStubber
        {
            private readonly Mock<IObjectService> objectServiceMock;
            private readonly ReferenceData referenceData;

            public ObjectStubber(Mock<IObjectService> objectServiceMock, ReferenceData referenceData)
            {
                this.objectServiceMock = objectServiceMock;
                this.referenceData = referenceData;
            }

            public ISetup<IObjectService, VariableData> Performs(string operationName) => Performs(operationName, p => true);

            public ISetup<IObjectService, VariableData> Performs(string operationName,
                Expression<Func<Dictionary<string, ParameterValueData>, bool>> parameterMatcher
            ) => objectServiceMock
                .Setup(o => o.Do(
                    referenceData,
                    operationName,
                    It.Is(parameterMatcher)
                ));
        }

        #endregion

        protected Rtype Rtyp(string id) => testingRapplication[id];

        protected Robject RobjNull() => testingRapplication.NullObject();
        protected Robject Robj(string id) => Robj(id, DefaultObjectModelId);
        protected Robject Robj(string id, string modelId) => Robj(id, modelId, modelId);
        protected Robject Robj(string id, string actualModelId, string viewModelId) =>
            testingRapplication.Get(id, actualModelId, viewModelId);

        protected Robject Robj(string modelId, params Rvariable[] initializationParameters) =>
            testingRapplication.Init(modelId, initializationParameters);

        protected Rvariable Rvar(string name, Robject value) => new(name, value);
        protected Rvariable Rvarlist(string name, IEnumerable<Robject> values) => new(name, values);
    }
}

