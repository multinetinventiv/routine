using System;
using System.Collections.Generic;
using System.Linq;

namespace Routine.Engine.Converter
{
	public class DelegateBasedConverter : ConverterBase<DelegateBasedConverter>
	{
		private readonly Func<IEnumerable<IType>> targetTypesDelegate;
		private readonly Func<object, IType, object> converterDelegate;

		public DelegateBasedConverter(Func<IEnumerable<IType>> targetTypesDelegate, Func<object, IType, object> converterDelegate)
		{
			if (targetTypesDelegate == null) { throw new ArgumentNullException("targetTypesDelegate"); }
			if (converterDelegate == null) { throw new ArgumentNullException("converterDelegate"); }

			this.targetTypesDelegate = targetTypesDelegate;
			this.converterDelegate = converterDelegate;
		}

		protected override List<IType> GetTargetTypes(IType type)
		{
			return targetTypesDelegate().ToList();
		}

		protected override object Convert(object @object, IType from, IType to)
		{
			return converterDelegate(@object, to);
		}
	}
}