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

		public TypeCastConverter ByCastingFrom(IType type)
		{
			return new TypeCastConverter(type);
		}
	}
}