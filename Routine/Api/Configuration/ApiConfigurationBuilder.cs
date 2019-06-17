using System;

namespace Routine.Api.Configuration
{
	public class ApiConfigurationBuilder
	{
		public ConventionBasedApiConfiguration FromBasic()
		{
			return new ConventionBasedApiConfiguration()
				.AssemblyVersion.Set(new Version(1, 0, 0, 0))
				.AssemblyGuid.Set(c => c.By(acm => Guid.NewGuid()))

				.TypeIsRendered.Set(false, rt => rt.IsVoid)
				.ReferencedType.Set(typeof(void), rt => rt.IsVoid)

				.TypeIsRendered.Set(true)

				.ReferencedType.SetDefault()
				
				.InitializerIsRendered.Set(true)
				.DataIsRendered.Set(true)
				.OperationIsRendered.Set(true)

				.RenderedTypeNamespace.Set(c => c.By(mm => string.Format("{0}.{1}", mm.Model.Application.DefaultNamespace, mm.Model.Type.Module))
												 .When(mm => !string.IsNullOrEmpty(mm.Model.Type.Module) && !mm.Model.Application.DefaultNamespace.Contains(mm.Model.Type.Module)))
				.RenderedTypeNamespace.Set(c => c.By(mm => mm.Model.Application.DefaultNamespace))

				.RenderedTypeName.Set(c => c.By(mm => mm.Model.Type.Name))
				.RenderedDataName.Set(c => c.By(mm => mm.Model.Name))
				.RenderedOperationName.Set(c => c.By(mm => mm.Model.Name))
				.RenderedParameterName.Set(c => c.By(mm => mm.Model.Name))

				.Override(c => c
					.ReferencedTypeTemplate.Set(new SimpleTypeConversionTemplate("{robject}.Id", "{rtype}.Get({object})"), typeof(string))
				)

				.NextLayer()
			;
		}
	}
}
