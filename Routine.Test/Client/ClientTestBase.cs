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
				.Returns(() => GetApplicationModel());
			objectServiceMock.Setup(o => o.Get(It.IsAny<ReferenceData>()))
				.Returns((ReferenceData ord) => ObjectData(ord));
			objectServiceMock.Setup(o => o.Do(It.IsAny<ReferenceData>(), It.IsAny<string>(), It.IsAny<Dictionary<string, ParameterValueData>>()))
				.Returns(Void());

			ModelsAre(Model());
		}

		private ObjectData ObjectData(ReferenceData ord)
		{
			return objectDictionary[ord];
		}

		#region Stubbers

		protected ObjectStubber When(ReferenceData referenceData)
		{
			return new ObjectStubber(objectServiceMock, referenceData);
		}

		protected class ObjectStubber
		{
			private readonly Mock<IObjectService> objectServiceMock;
			private readonly ReferenceData referenceData;

			public ObjectStubber(Mock<IObjectService> objectServiceMock, ReferenceData referenceData)
			{
				this.objectServiceMock = objectServiceMock;
				this.referenceData = referenceData;
			}

			public ISetup<IObjectService, VariableData> Performs(string operationName) { return Performs(operationName, p => true); }
			public ISetup<IObjectService, VariableData> Performs(string operationName, Expression<Func<Dictionary<string, ParameterValueData>, bool>> parameterMatcher)
			{
				return objectServiceMock
						.Setup(o => o.Do(
							referenceData,
							operationName,
							It.Is(parameterMatcher)));
			}
		}

		#endregion

		protected Rtype Rtyp(string id)
		{
			return testingRapplication[id];
		}

		protected Robject RobjNull() { return testingRapplication.NullObject(); }
		protected Robject Robj(string id) { return Robj(id, DefaultObjectModelId); }
		protected Robject Robj(string id, string modelId) { return Robj(id, modelId, modelId); }
		protected Robject Robj(string id, string actualModelId, string viewModelId)
		{
			return testingRapplication.Get(id, actualModelId, viewModelId);
		}

		protected Robject Robj(string modelId, params Rvariable[] initializationParameters)
		{
			return testingRapplication.Init(modelId, initializationParameters);
		}

		protected Rvariable Rvar(string name, Robject value)
		{
			return new Rvariable(name, value);
		}

		protected Rvariable Rvarlist(string name, IEnumerable<Robject> values)
		{
			return new Rvariable(name, values);
		}
	}
}

