namespace Routine.Engine.Converter
{
	public abstract class ConverterBase<TConcrete> : IConverter
		where TConcrete : ConverterBase<TConcrete>
	{
		private object ConvertInner(object @object, IType targetType)
		{
			var result = Convert(@object, targetType);

			return result;
		}

		protected abstract object Convert(object @object, IType targetType);

		#region IConverter implementation

		object IConverter.Convert(object @object, IType targetType) { return ConvertInner(@object, targetType); }

		#endregion
	}

}
