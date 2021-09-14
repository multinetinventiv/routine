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
            this.targetTypesDelegate = targetTypesDelegate ?? throw new ArgumentNullException(nameof(targetTypesDelegate));
			this.converterDelegate = converterDelegate ?? throw new ArgumentNullException(nameof(converterDelegate));
		}

		protected override List<IType> GetTargetTypes(IType type) => targetTypesDelegate().ToList();
        protected override object Convert(object @object, IType from, IType to) => converterDelegate(@object, to);
    }
}