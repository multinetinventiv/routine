using System;
using Routine.Engine.Configuration.ConventionBased;

namespace Routine.Engine.Configuration
{
	public class CodingStyleBuilder
	{
		public ConventionBasedCodingStyle FromBasic()
		{
			return new ConventionBasedCodingStyle()
				.AddCommonSystemTypes()

				.MaxFetchDepth.Set(Constants.DEFAULT_MAX_FETCH_DEPTH)

				.Type.Set(c => c.By(o => o.GetTypeInfo()))

				.Module.Set(c => c
					.By(t => ((TypeInfo)t).GetActualType().GetGenericArguments()[0].Namespace)
					.When(t => t is TypeInfo ti && ti.IsValueType && ti.IsGenericType && ti.GetActualType().GetGenericTypeDefinition() == typeof(Nullable<>)))
				.TypeName.Set(c => c
					.By(t => ((TypeInfo)t).GetActualType().GetGenericArguments()[0].Name + "?")
					.When(t => t is TypeInfo ti && ti.IsValueType && ti.IsGenericType && ti.GetActualType().GetGenericTypeDefinition() == typeof(Nullable<>)))

				.Module.Set(c => c.By(t => t.Namespace))
				.TypeName.Set(c => c.By(t => t.ToCSharpString(false)))
				.DataName.Set(c => c.By(m => m.Name))
				.OperationName.Set(c => c.By(o => o.Name))
				.ParameterName.Set(c => c.By(p => p.Name))

				.TypeIsValue.Set(false)

				.Converters.Add(c => c.Convert(b => b.ToNullable()).When(t => t.IsValueType))
				.Converters.Add(c => c.Convert(b => b.ByCasting()))

				.TypeIsView.Set(true, t => t.IsAbstract || t.IsInterface)
				.TypeIsView.Set(false)

				.DataFetchedEagerly.Set(false)

				.IdExtractor.SetDefault()
				.ValueExtractor.SetDefault()
				.Locator.SetDefault()

				.NextLayer()

				.Use(p => p.ParseableValueTypePattern())

				.Override(cfg => cfg
					.AddTypes((IType)null)
					.TypeIsValue.Set(true, t => t == null)
					.TypeIsView.Set(true, t => t == null)
					.Locator.Set(c => c.Locator(l => l.Constant(null)).WhenDefault())
					.IdExtractor.Set(c => c.Id(e => e.Constant(null)).WhenDefault())
					.Converters.AddNoneWhen(t => t == null)
					.ValueExtractor.Set(c => c.Value(e => e.Constant(string.Empty)).WhenDefault())
					.TypeMarks.AddNoneWhen(t => t == null)
					.Initializers.AddNoneWhen(t => t == null)
					.Operations.AddNoneWhen(t => t == null)
					.Datas.AddNoneWhen(t => t == null)
					.StaticInstances.AddNoneWhen(t => t == null)
					.Module.Set(null, t => t == null)
					.TypeName.Set(Constants.NULL_MODEL_NAME, t => t == null)

					.Module.Set(null, t => t.IsVoid)
					.TypeName.Set(c => c.By(_ => Constants.VOID_MODEL_NAME).When(t => t.IsVoid))
					.TypeIsValue.Set(true, t => t.IsVoid)
				)
					
				.Override(cfg => cfg
					.TypeIsValue.Set(true, t => t.CanBe<string>())
				
					.IdExtractor.Set(c => c.Id(e => e.By(o => o as string)).When(type.of<string>()))
					.Locator.Set(c => c.Locator(l => l.SingleBy(o => o)).When(type.of<string>()))

					.Datas.AddNoneWhen(type.of<string>())
					.Operations.AddNoneWhen(type.of<string>())
				)

				.NextLayer()
			;
		}
	}
}
