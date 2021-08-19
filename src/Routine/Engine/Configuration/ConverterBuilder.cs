using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Engine.Converter;

namespace Routine.Engine.Configuration
{
	public class ConverterBuilder
	{
		public DelegateBasedConverter ToConstant(object staticResult)
		{
			if (staticResult == null)
			{
				return By((IType)null, (_, _) => null);
			}

			return By(() => staticResult.GetTypeInfo(), (_, _) => staticResult);
		}

		public DelegateBasedConverter By(IType targetType, Func<object, IType, object> converterDelegate) { return By(() => targetType, converterDelegate); }
		public DelegateBasedConverter By(IEnumerable<IType> targetTypes, Func<object, IType, object> converterDelegate) { return By(() => targetTypes.ToList(), converterDelegate); }

		public DelegateBasedConverter By(Func<IType> targetTypeDelegate, Func<object, IType, object> converterDelegate)
		{
			if (targetTypeDelegate == null) { throw new ArgumentNullException(nameof(targetTypeDelegate)); }

			return By(() => new List<IType> { targetTypeDelegate() }, converterDelegate);
		}

		public DelegateBasedConverter By(Func<IEnumerable<IType>> targetTypesDelegate, Func<object, IType, object> converterDelegate)
		{
			return new DelegateBasedConverter(targetTypesDelegate, converterDelegate);
		}

		public TypeCastConverter ByCasting() { return ByCasting(_ => true); }
		public TypeCastConverter ByCasting(Func<IType, bool> viewTypePredicate)
		{
			if (viewTypePredicate == null) { throw new ArgumentNullException(); }

			return new TypeCastConverter(viewTypePredicate);
		}

		public NullableConverter ToNullable()
		{
			return new NullableConverter();
		}
	}
}