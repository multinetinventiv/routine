using System;
using System.Data;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Type;

namespace Routine.Test.Domain.NHibernate.UserType
{
	internal abstract class BaseSimpleImmutableCompositeUserType<TType, TValueType> : BaseImmutableCompositeUserType<TType>
	{
		public BaseSimpleImmutableCompositeUserType(IType propertyType)
			: base(new[] { "Value" }, new[] { propertyType }) { }
		
		public override object GetPropertyValue(object component, int property) { return component != null ? ToValueType((TType)component):default(TValueType); }

		public override object NullSafeGet(IDataReader dr, string[] names, ISessionImplementor session, object owner)
		{
			var value = (TValueType)PropertyTypes[0].NullSafeGet(dr, names[0], session, owner);

			if (object.Equals(value, default(TValueType)))
			{
				return default(TType);
			}

			return ToType(value);
		}

		public abstract TType ToType(TValueType value);
		public abstract TValueType ToValueType(TType component);
	}
}
