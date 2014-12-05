using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Routine.Core;
using Routine.Core.Cache;
using Routine.Engine;
using Routine.Engine.Configuration.Conventional;
using Routine.Engine.Context;
using Routine.Test.Core;

namespace Routine.Test.Engine
{
	public abstract class ObjectServiceTestBase : CoreTestBase
	{
		protected Dictionary<string, object> objectRepository;

		protected ICoreContext ctx;
		protected ConventionalCodingStyle codingStyle;

		protected ObjectService testing;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			objectRepository = new Dictionary<string, object>();

			codingStyle = BuildRoutine.CodingStyle().FromBasic()
				.AddTypes(GetType().Assembly, t => t.IsPublic && t.Namespace.StartsWith(RootNamespace))
				.Use(p => p.ShortModelIdPattern("System", "s"))
				.Use(p => p.ShortModelIdPattern("Routine.Test.Common", "c"))
				.Use(p => p.ParseableValueTypePattern())

				.Initializers.Add(c => c.Initializers().When(t => t.IsValueType && t.IsDomainType))
				.Members.Add(c => c.PublicMembers(m => !m.IsInherited()).When(t => t.IsDomainType))
				.Operations.Add(c => c.PublicOperations(m => !m.IsInherited()).When(t => t.IsDomainType))

				.IdExtractor.Set(c => c.IdByMember(p => p.Returns<string>("Id")).When(t => t.IsDomainType))
				.ObjectLocator.Set(c => c.Locator(l => l.By(id => objectRepository[id])).When(t => t.IsDomainType))

				.NextLayer()
				;

			var cache = new DictionaryCache();
			ctx = new DefaultCoreContext(codingStyle, cache);

			testing = new ObjectService(ctx, cache);
		}

		protected void AddToRepository(object obj)
		{
			testing.GetApplicationModel();

			var idExtractor = ctx.CodingStyle.GetIdExtractor(obj.GetTypeInfo());
			var id = idExtractor.GetId(obj);
			objectRepository.Add(id, obj);
		}

		protected ObjectReferenceData IdNull(){return Id(null, null, null, true);}
		protected virtual ObjectReferenceData Id(string id) { return Id(id, DefaultModelId);}
		protected ObjectReferenceData Id(string id, string modelId) {return Id(id, modelId, modelId);}
		protected ObjectReferenceData Id(string id, string actualModelId, string viewModelId) {return Id(id, actualModelId, viewModelId, false);}
		protected ObjectReferenceData Id(string id, string actualModelId, string viewModelId, bool isNull)
		{
			return new ObjectReferenceData{ Id = id, ActualModelId = actualModelId, ViewModelId = viewModelId, IsNull = isNull };
		}

		protected Dictionary<string, ParameterValueData> Params(params KeyValuePair<string, ParameterValueData>[] parameters)
		{
			var result = new Dictionary<string, ParameterValueData>();

			foreach (var parameter in parameters)
			{
				result.Add(parameter.Key, parameter.Value);
			}

			return result;
		}

		protected ParameterData Init(string objectModelId, Dictionary<string, ParameterValueData> initializationParameters)
		{
			return new ParameterData {
				ObjectModelId = objectModelId,
				InitializationParameters = initializationParameters
			};
		}

		protected KeyValuePair<string, ParameterValueData> Param(string modelId, params ParameterData[] values) { return Param(modelId, values.Length > 1, values); }
		protected KeyValuePair<string, ParameterValueData> Param(string modelId, bool isList, params ParameterData[] values) 
		{
			return new KeyValuePair<string, ParameterValueData>(modelId, 
				new ParameterValueData {
					IsList = isList,
					Values = values.ToList()
				}
			);
		}

		protected KeyValuePair<string, ParameterValueData> Param(string modelId, params ObjectReferenceData[] references) { return Param(modelId, references.Length > 1, references); }
		protected KeyValuePair<string, ParameterValueData> Param(string modelId, bool isList, params ObjectReferenceData[] references)
		{
			return new KeyValuePair<string, ParameterValueData>(modelId, new ParameterValueData { IsList = isList, Values = references.Select(r => PD(r)).ToList() });
		}

		protected ParameterData PD(ObjectReferenceData reference)
		{
			return new ParameterData { ObjectModelId = reference.ActualModelId, IsNull = reference.IsNull, ReferenceId = reference.Id };
		}

		protected abstract string DefaultModelId { get; }
		protected abstract string RootNamespace { get; }
	}
}

