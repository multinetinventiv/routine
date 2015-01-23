using System;

namespace Routine.Api.Configuration
{
	public class ApiConfigurationBuilder
	{
		public ConventionalApiConfiguration FromBasic()
		{
			return new ConventionalApiConfiguration()
				.Version.Set(new Version(1, 0, 0, 0))

				.TypeIsRendered.Set(true)

				.ReferencedType.SetDefault()
				
				.InitializerIsRendered.Set(true)
				.MemberIsRendered.Set(true)
				.OperationIsRendered.Set(true)

				.RenderedTypeNamespace.Set(c => c.By(mm => string.Format("{0}.{1}", mm.Model.Application.DefaultNamespace, mm.Model.Type.Module))
												 .When(mm => !string.IsNullOrEmpty(mm.Model.Type.Module) && !mm.Model.Application.DefaultNamespace.Contains(mm.Model.Type.Module)))
				.RenderedTypeNamespace.Set(c => c.By(mm => mm.Model.Application.DefaultNamespace))

				.RenderedTypeName.Set(c => c.By(mm => mm.Model.Type.Name))
				.RenderedMemberName.Set(c => c.By(mm => mm.Model.Id))
				.RenderedOperationName.Set(c => c.By(mm => mm.Model.Id))
				.RenderedParameterName.Set(c => c.By(mm => mm.Model.Id))

				.Override(c => c
					.ReferencedTypeTemplate.Set(new SimpleTypeConversionTemplate("{robject}.Id", "{rtype}.Get({object})"), type.of<string>())
				)

				.NextLayer()
			;
		}
	}
}
