using System;

namespace Routine.Engine.Converter
{
	public class TypeCastConverter : ConverterBase<TypeCastConverter>
	{
		private readonly IType type;

		public TypeCastConverter(IType type)
		{
			if (type == null) { throw new ArgumentNullException("type"); }

			this.type = type;
		}

		protected override object Convert(object @object, IType targetType)
		{
			if (!type.CanBe(targetType))
			{
				throw new CannotConvertException(@object, targetType);
			}
			
			return type.Cast(@object, targetType);
		}
	}
}