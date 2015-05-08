using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Routine.Api.Configuration;
using Routine.Client;
using Routine.Core.Configuration;
using Routine.Engine;

namespace Routine.Api.Template
{
	public partial class ClientApiTemplate : IApiTemplate
	{
		public string ApiName { get; private set; }

		public ClientApiTemplate(string apiName)
		{
			ApiName = apiName;
		}

		private ApplicationCodeModel applicationCodeModel;

		public string Render(ApplicationCodeModel applicationCodeModel)
		{
			this.applicationCodeModel = applicationCodeModel;

			return TransformText();
		}

		public ApplicationCodeModel Application { get { return applicationCodeModel; } }

		public List<TypeCodeModel> OperationalModels { get { return Application.Models.Where(m => m.HasMode(Mode.Interface)).ToList(); } }
		public List<TypeCodeModel> InitializableOperationalModels { get { return OperationalModels.Where(m => m.HasMode(Mode.FactoryInterface)).ToList(); } }
		public List<TypeCodeModel> SingletonModels { get { return OperationalModels.Where(m => m.Type.StaticInstances.Count == 1).ToList(); } }
		public List<TypeCodeModel> GetInstanceModels { get { return OperationalModels.Where(m => !m.Type.IsViewType).ToList(); } }

		public List<TypeCodeModel> InitializeOnlyStructModels { get { return Application.Models.Where(m => m.HasMode(Mode.InitializeOnlyStruct)).ToList(); } }

		public List<TypeCodeModel> EnumModels { get { return Application.Models.Where(m => m.HasMode(Mode.Enum)).ToList(); } }

		public class Mode
		{
			internal static readonly int Referenced = -1;

			public static readonly int Interface = 0;
			public static readonly int Concrete = 1;
			public static readonly int Factory = 2;
			public static readonly int FactoryInterface = 3;
			public static readonly int InitializeOnlyStruct = 4;
			public static readonly int InitializeOnlyStructProperty = 5;
			public static readonly int Enum = 6;
			public static readonly int EnumConverter = 7;
		}
	}

	public static class ClientApiTemplateExtensions
	{
		public static ConventionalApiConfiguration ClientApi(this ApiConfigurationBuilder source)
		{
			return source.ClientApi(
				tcm => tcm.Members.Any() || tcm.Operations.Any()
			);
		}

		public static ConventionalApiConfiguration ClientApi(this ApiConfigurationBuilder source, Func<TypeCodeModel, bool> interfaceModePredicate)
		{
			return source.ClientApi(
				interfaceModePredicate,
				tcm => !interfaceModePredicate(tcm)
			);
		}

		public static ConventionalApiConfiguration ClientApi(this ApiConfigurationBuilder source,
			Func<TypeCodeModel, bool> interfaceModePredicate,
			Func<TypeCodeModel, bool> initializeOnlyStructModePredicate
			)
		{
			return source.ClientApi(
				interfaceModePredicate,
				initializeOnlyStructModePredicate,
				tcm => !interfaceModePredicate(tcm)
			);
		}
		public static ConventionalApiConfiguration ClientApi(this ApiConfigurationBuilder source,
			Func<TypeCodeModel, bool> interfaceModePredicate,
			Func<TypeCodeModel, bool> initializeOnlyStructModePredicate,
			Func<TypeCodeModel, bool> enumPredicate
		)
		{
			var initializeOnlyStructModePredicate_inner = initializeOnlyStructModePredicate.And(tcm => tcm.Initializable);
			var enumPredicate_inner = enumPredicate.And(tcm => tcm.Type.StaticInstances.Any());

			return source.FromBasic()
				.Modes.Add(ClientApiTemplate.Mode.Interface, interfaceModePredicate)
				.Modes.Add(ClientApiTemplate.Mode.Concrete, interfaceModePredicate)

				.Modes.Add(ClientApiTemplate.Mode.FactoryInterface, tcm => interfaceModePredicate(tcm) && tcm.Initializable)
				.Modes.Add(ClientApiTemplate.Mode.Factory, tcm => interfaceModePredicate(tcm) && tcm.Initializable)

				.Modes.Add(ClientApiTemplate.Mode.Enum, enumPredicate_inner)
				.Modes.Add(ClientApiTemplate.Mode.EnumConverter, enumPredicate_inner)

				.Modes.Add(ClientApiTemplate.Mode.InitializeOnlyStruct, initializeOnlyStructModePredicate_inner)

				.RenderedTypeTemplate.Set(new ClientApiInterfaceConversionTemplate(), mm => mm.Mode == ClientApiTemplate.Mode.Interface)

				.RenderedTypeName.Set(c => c.By(mm => mm.Model.Type.Name + "Impl").When(mm => mm.Mode == ClientApiTemplate.Mode.Concrete && mm.Model.Type.IsViewType))
				.RenderedTypeName.Set(c => c.By(mm => mm.Model.Type.Name + "FactoryImpl").When(mm => mm.Mode == ClientApiTemplate.Mode.Factory && mm.Model.Type.IsViewType))

				.RenderedTypeName.Set(c => c.By(mm => "I" + mm.Model.Type.Name).When(mm => mm.Mode == ClientApiTemplate.Mode.Interface))
				.RenderedTypeName.Set(c => c.By(mm => mm.Model.Type.Name).When(mm => mm.Mode == ClientApiTemplate.Mode.Concrete))

				.RenderedTypeName.Set(c => c.By(mm => "I" + mm.Model.Type.Name + "Factory").When(mm => mm.Mode == ClientApiTemplate.Mode.FactoryInterface))
				.RenderedTypeName.Set(c => c.By(mm => mm.Model.Type.Name + "Factory").When(mm => mm.Mode == ClientApiTemplate.Mode.Factory))

				.RenderedTypeTemplate.Set(new ClientApiEnumConversionTemplate(), mm => mm.Mode == ClientApiTemplate.Mode.Enum)
				.RenderedTypeName.Set(c => c.By(mm => mm.Model.Type.Name).When(mm => mm.Mode == ClientApiTemplate.Mode.Enum))
				.RenderedTypeName.Set(c => c.By(mm => mm.Model.Type.Name + "Converter").When(mm => mm.Mode == ClientApiTemplate.Mode.EnumConverter))

				.RenderedTypeTemplate.Set(new ClientApiStructConversionTemplate(), mm => mm.Mode == ClientApiTemplate.Mode.InitializeOnlyStruct)
				.RenderedTypeName.Set(c => c.By(mm => mm.Model.Type.Name).When(mm => mm.Mode == ClientApiTemplate.Mode.InitializeOnlyStruct))
				.RenderedParameterName.Set(c => c.By(mm => mm.Model.Id.ToUpperInitial()).When(mm => mm.Mode == ClientApiTemplate.Mode.InitializeOnlyStructProperty))

				.NextLayer()
			;
		}

		public static ConventionalApiConfiguration ReferenceOtherClientApiPattern(this PatternBuilder<ConventionalApiConfiguration> source, Assembly otherApiAssembly, IApiContext otherApiContext)
		{
			return source
				.FromEmpty()

				.TypeIsRendered.Set(false, rt => otherApiContext.Configuration.IsRendered(rt))
				.ReferencedType.Set(c => c
					.By(rt => otherApiAssembly.GetTypes().Single(t => t.FullName == otherApiContext.Application.GetModel(rt).GetFullName()).ToTypeInfo())
					.When(rt => otherApiContext.Configuration.IsRendered(rt)))

				.ReferencedTypeTemplate.Set(c => c
					.By(t => new ClientApiReferencedEnumConversionTemplate(otherApiContext.Application.Models.Single(tcm => tcm.GetFullName(ClientApiTemplate.Mode.Enum) == t.FullName)))
					.When(t => t.IsInAssembly(otherApiAssembly) && t.IsEnum)
				)
				.ReferencedTypeTemplate.Set(new ClientApiStructConversionTemplate(), t => t.IsInAssembly(otherApiAssembly) && t.IsValueType && !t.IsEnum)
				.ReferencedTypeTemplate.Set(c => c
					.By(t => new ClientApiReferencedInterfaceConversionTemplate(otherApiContext.Application.Models.Single(tcm => tcm.GetFullName(ClientApiTemplate.Mode.Interface) == t.FullName)))
					.When(t => t.IsInAssembly(otherApiAssembly) && t.IsInterface)
				)
			;
		}

		private static bool IsInAssembly(this IType source, Assembly assembly)
		{
			return source is TypeInfo && ((TypeInfo)source).GetActualType().Assembly == assembly;
		}

		private class ClientApiStructConversionTemplate : TypeConversionTemplateBase
		{
			public ClientApiStructConversionTemplate()
				: base(null, "{object}.ToRobject({rtype})") { }

			public override string RenderRobjectToObject(TypeCodeModel model, string robjectExpression, string rtypeExpression)
			{
				return RenderRobjectToObject(
					"robject", robjectExpression,
					"rtype", rtypeExpression
				);
			}

			public override string RenderObjectToRobject(TypeCodeModel model, string objectExpression, string rtypeExpression)
			{
				return RenderObjectToRobject(
					"object", objectExpression,
					"rtype", rtypeExpression
				);
			}
		}

		private class ClientApiInterfaceConversionTemplate : TypeConversionTemplateBase
		{
			public ClientApiInterfaceConversionTemplate()
				: base("(({type}) new {concrete-type}({robject}))", "(({concrete-type}){object}).Robject") { }

			public override string RenderRobjectToObject(TypeCodeModel model, string robjectExpression, string rtypeExpression)
			{
				return RenderRobjectToObject(
					"robject", robjectExpression,
					"type", model.GetFullName(ClientApiTemplate.Mode.Interface, true),
					"concrete-type", model.GetFullName(ClientApiTemplate.Mode.Concrete, true)
				);
			}

			public override string RenderObjectToRobject(TypeCodeModel model, string objectExpression, string rtypeExpression)
			{
				return RenderObjectToRobject(
					"object", objectExpression,
					"concrete-type", model.GetFullName(ClientApiTemplate.Mode.Concrete, true)
				);
			}
		}

		private class ClientApiReferencedInterfaceConversionTemplate : ClientApiInterfaceConversionTemplate
		{
			private readonly TypeCodeModel renderedModel;

			public ClientApiReferencedInterfaceConversionTemplate(TypeCodeModel renderedModel)
			{
				this.renderedModel = renderedModel;
			}

			public override string RenderRobjectToObject(TypeCodeModel referencedModel, string robjectExpression, string rtypeExpression)
			{
				return base.RenderRobjectToObject(renderedModel, robjectExpression, rtypeExpression);
			}

			public override string RenderObjectToRobject(TypeCodeModel referencedModel, string objectExpression, string rtypeExpression)
			{
				return base.RenderObjectToRobject(renderedModel, objectExpression, rtypeExpression);
			}
		}

		private class ClientApiEnumConversionTemplate : TypeConversionTemplateBase
		{
			public ClientApiEnumConversionTemplate()
				: base("{converter-type}.ToEnum({robject})", "{converter-type}.ToRobject({object}, {rtype})") { }


			public override string RenderRobjectToObject(TypeCodeModel model, string robjectExpression, string rtypeExpression)
			{
				return RenderRobjectToObject(
					"converter-type", model.GetFullName(ClientApiTemplate.Mode.EnumConverter, true),
					"robject", robjectExpression
				);
			}

			public override string RenderObjectToRobject(TypeCodeModel model, string objectExpression, string rtypeExpression)
			{
				return RenderObjectToRobject(
					"converter-type", model.GetFullName(ClientApiTemplate.Mode.EnumConverter, true),
					"object", objectExpression,
					"rtype", rtypeExpression
				);
			}
		}

		private class ClientApiReferencedEnumConversionTemplate : ClientApiEnumConversionTemplate
		{
			private readonly TypeCodeModel renderedModel;

			public ClientApiReferencedEnumConversionTemplate(TypeCodeModel renderedModel)
			{
				this.renderedModel = renderedModel;
			}

			public override string RenderRobjectToObject(TypeCodeModel referencedModel, string robjectExpression, string rtypeExpression)
			{
				return base.RenderRobjectToObject(renderedModel, robjectExpression, rtypeExpression);
			}

			public override string RenderObjectToRobject(TypeCodeModel referencedModel, string objectExpression, string rtypeExpression)
			{
				return base.RenderObjectToRobject(renderedModel, objectExpression, rtypeExpression);
			}
		}

		public static string GetEnumMemberName(this Robject source)
		{
			var result = source.Value
				.Replace(" ", "_")
				.SnakeCaseToCamelCase()
				.ToUpperInitial();

			if (Char.IsNumber(result[0]))
			{
				result = "_" + result;
			}

			return Regex.Replace(result, "[^0-9a-zA-Z_]", "_");
		}

		public static string GetFullName(this TypeCodeModel source)
		{
			return source.GetFullName(source.GetDefaultMode());
		}

		public static string RenderRvariableToObject(this TypeCodeModel source, string rvariableExpression, string rapplicationExpression)
		{
			return source.RenderRvariableToObject(source.GetDefaultMode(), rvariableExpression, rapplicationExpression);
		}

		public static string RenderRobjectToObject(this TypeCodeModel source, string robjectExpression, string rapplicationExpression)
		{
			return source.RenderRobjectToObject(source.GetDefaultMode(), robjectExpression, rapplicationExpression);
		}

		public static string RenderObjectToRvariable(this TypeCodeModel source, string rvariableName, string objectExpression, string rapplicationExpression)
		{
			return source.RenderObjectToRvariable(source.GetDefaultMode(), rvariableName, objectExpression, rapplicationExpression);
		}

		public static string RenderObjectToRobject(this TypeCodeModel source, string objectExpression, string rapplicationExpression)
		{
			return source.RenderObjectToRobject(source.GetDefaultMode(), objectExpression, rapplicationExpression);
		}

		private static int GetDefaultMode(this TypeCodeModel source)
		{
			if (source.IsReferenced)
			{
				return ClientApiTemplate.Mode.Referenced;
			}

			if (source.HasMode(ClientApiTemplate.Mode.Interface))
			{
				return ClientApiTemplate.Mode.Interface;
			}

			if (source.HasMode(ClientApiTemplate.Mode.Enum))
			{
				return ClientApiTemplate.Mode.Enum;
			}

			if (source.HasMode(ClientApiTemplate.Mode.InitializeOnlyStruct))
			{
				return ClientApiTemplate.Mode.InitializeOnlyStruct;
			}

			throw new InvalidOperationException(string.Format("Default mode for '{0}' cannot be determined", source));
		}
	}
}
