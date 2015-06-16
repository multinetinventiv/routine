using System;

namespace Routine.Engine.Converter
{
	public class NullableConverter : ConverterBase<NullableConverter>
	{
		private readonly TypeInfo type;

		public NullableConverter(TypeInfo type)
		{
			if (!type.IsValueType) { throw new ArgumentException("'type' should be value type", "type"); }

			this.type = type;
		}

		protected override object Convert(object @object, IType targetType)
		{
			var targetTypeInfo = targetType as TypeInfo;

			if (targetTypeInfo == null) { throw new CannotConvertException(@object, targetType); }
			if (!targetTypeInfo.IsValueType) { throw new CannotConvertException(@object, targetType); }

			try
			{
				return Activator.CreateInstance(targetTypeInfo.GetActualType(), @object);
			}
			catch (Exception)
			{
				throw new CannotConvertException(@object, targetType);
			}
		}
	}
}