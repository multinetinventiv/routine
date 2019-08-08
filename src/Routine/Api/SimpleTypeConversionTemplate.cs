namespace Routine.Api
{
	public class SimpleTypeConversionTemplate : TypeConversionTemplateBase
	{
		public SimpleTypeConversionTemplate(string robjectToObjectTemplate, string objectToRobjectTemplate)
			: base(robjectToObjectTemplate, objectToRobjectTemplate) { }

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
}