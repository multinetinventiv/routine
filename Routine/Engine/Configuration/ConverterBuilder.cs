using System;
using Routine.Engine.Converter;

namespace Routine.Engine.Configuration
{
	public class ConverterBuilder
	{
		public DelegateBasedConverter Constant(object staticResult)
		{
			return By((o, t) => staticResult);
		}

		public DelegateBasedConverter By(Func<object, IType, object> converterDelegate)
		{
			return new DelegateBasedConverter(converterDelegate);
		}

		public TypeCastConverter ByCasting(IType type)
		{
			return new TypeCastConverter(type);
		}

		public NullableConverter ToNullable(TypeInfo type)
		{
			return new NullableConverter(type);
		}
	}
}