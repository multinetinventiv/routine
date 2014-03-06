using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Routine.Core.Service;
using Routine.Core.Service.Impl;
using Routine.Core;
using Routine.Core.Configuration;
using Routine.Core.Context;
using Routine.Core.Cache;

namespace Routine.Test.Core.Service
{
	public abstract class ObjectServiceTestBase  :CoreTestBase
	{
		protected Dictionary<string, object> objectRepository;

		protected ICoreContext ctx;
		protected GenericCodingStyle codingStyle;

		protected ObjectService testing;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			objectRepository = new Dictionary<string, object>();

			codingStyle = BuildRoutine.CodingStyle().FromBasic()
				.Use(p => p.NullPattern("_null"))
				.Use(p => p.ParseableValueTypePattern(":"))
				.Member.Done(s => s.ByPublicProperties(p => p.IsOnReflected() && !p.IsIndexer).When(t => t.IsDomainType))
				.Operation.Done(s => s.ByPublicMethods(m => m.IsOnReflected()).When(t => t.IsDomainType))

				.Id.Done(e => e.ByProperty(p => p.Returns<string>("Id")))
				.Locator.Done(l => l.ByConverting(id => objectRepository[id]).WhenId(id => objectRepository.ContainsKey(id)))
				;

			var cache = new DictionaryCache();
			ctx = new CachedCoreContext(codingStyle, cache);

			testing = new ObjectService(ctx, cache);
		}

		protected void AddToRepository(object obj)
		{
			objectRepository.Add(ctx.CodingStyle.IdExtractor.Extract(obj), obj);
		}

		protected ObjectReferenceData IdNull(){return Id(null, null, null, true);}
		protected virtual ObjectReferenceData Id(string id) { return Id(id, DefaultModelId);}
		protected ObjectReferenceData Id(string id, string modelId) {return Id(id, modelId, modelId);}
		protected ObjectReferenceData Id(string id, string actualModelId, string viewModelId) {return Id(id, actualModelId, viewModelId, false);}
		protected ObjectReferenceData Id(string id, string actualModelId, string viewModelId, bool isNull)
		{
			return new ObjectReferenceData{ Id = id, ActualModelId = actualModelId, ViewModelId = viewModelId, IsNull = isNull };
		}

		protected List<ParameterValueData> Params(params ParameterValueData[] parameters)
		{
			return parameters.ToList();
		}

		protected ParameterValueData Param(string modelId, params ObjectReferenceData[] references) { return Param(modelId, references.Length == 1, references);}
		protected ParameterValueData Param(string modelId, bool isList, params ObjectReferenceData[] references)
		{
			return new ParameterValueData {
				ParameterModelId = modelId,
				Value = new ReferenceData {
					IsList = isList,
					References = references.ToList()
				}
			};
		}

		protected abstract string DefaultModelId{get;}
	}
}

