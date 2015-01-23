using System;
using System.Linq;
using System.Reflection;
using Routine.Api;
using Routine.Api.Configuration;
using Routine.Api.Context;
using Routine.Client;
using Routine.Test.Client;

namespace Routine.Test.Api
{
	public abstract class ApiTestBase : ClientTestBase
	{
		protected ApiGenerator Generator() { return Generator(c => c); }
		protected ApiGenerator Generator(Func<ConventionalApiConfiguration, ConventionalApiConfiguration> config)
		{
			return EmptyGenerator(c =>
				config(c
				.InMemory.Set(true)
				.DefaultNamespace.Set(DefaultNamespace)
				.TypeIsRendered.Set(false, t => t.Id == DefaultObjectModelId)
				)
			);
		}

		protected virtual ConventionalApiConfiguration BaseConfiguration()
		{
			return BuildRoutine.ApiConfig().FromBasic();
		}

		protected ApiGenerator EmptyGenerator(Func<ConventionalApiConfiguration, ConventionalApiConfiguration> config)
		{
			var cfg = config(BaseConfiguration());

			return new ApiGenerator(new DefaultApiContext(cfg, new ApplicationCodeModel(testingRapplication, cfg)));
		}

		protected Type GetRenderedType(string typeName) { return GetRenderedType(Generator().Generate(DefaultTestTemplate), typeName); }
		protected Type GetRenderedType(Assembly clientAssembly, string typeName) { return GetRenderedType(clientAssembly, t => t.Name == typeName); }
		protected Type GetRenderedType(Assembly clientAssembly, Func<Type, bool> typePredicate)
		{
			return clientAssembly.GetTypes().SingleOrDefault(typePredicate);
		}

		protected object CreateInstance(string id, string modelId) { return CreateInstance(Generator(), id, modelId); }
		protected object CreateInstance(string id, string actualModelId, string viewModelId) { return CreateInstance(Generator(), id, actualModelId, viewModelId); }
		protected object CreateInstance(ApiGenerator generator, string id, string modelId) { return CreateInstance(generator, id, modelId, modelId); }
		protected object CreateInstance(ApiGenerator generator, string id, string actualModelId, string viewModelId)
		{
			return CreateInstance(GetRenderedType(generator.Generate(DefaultTestTemplate), viewModelId), id, actualModelId, viewModelId);
		}

		protected object CreateInstance(Type renderedType, string id, string modelId) { return CreateInstance(renderedType, id, modelId, modelId); }
		protected object CreateInstance(Type renderedType, string id, string actualModelId, string viewModelId)
		{
			var ctor = renderedType
				.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.Single(ci => ci.GetParameters().Length == 1 && ci.GetParameters().Any(pi => pi.ParameterType == typeof(Robject)));

			return ctor.Invoke(new object[] { Robj(id, actualModelId, viewModelId) });
		}

		protected abstract IApiTemplate DefaultTestTemplate { get; }
		protected abstract string DefaultNamespace { get; }
	}
}

