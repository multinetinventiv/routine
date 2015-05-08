using System;

namespace Routine.Engine.Converter
{
	public class DelegateBasedConverter : ConverterBase<DelegateBasedConverter>
	{
		private readonly Func<object, IType, object> converterDelegate;

		public DelegateBasedConverter(Func<object, IType, object> converterDelegate)
		{
			if (converterDelegate == null) { throw new ArgumentNullException("converterDelegate"); }

			this.converterDelegate = converterDelegate;
		}

		protected override object Convert(object @object, IType targetType)
		{
			return converterDelegate(@object, targetType);
		}
	}
}