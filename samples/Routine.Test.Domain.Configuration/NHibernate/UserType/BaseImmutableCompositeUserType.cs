using System;
using System.Data;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace Routine.Test.Domain.NHibernate.UserType
{
	internal abstract class BaseImmutableCompositeUserType<T> : ICompositeUserType
	{
		public Type ReturnedClass { get { return typeof(T); } }
		public string[] PropertyNames { get; private set; }
		public IType[] PropertyTypes { get; private set; }

		public BaseImmutableCompositeUserType(string[] propertyNames, IType[] propertyTypes)
		{
			PropertyNames = propertyNames;
			PropertyTypes = propertyTypes;
		}

		public bool IsMutable { get { return false; } }
		public object Assemble(object cached, ISessionImplementor session, object owner) { return cached; }
		public object DeepCopy(object value) { return value; }
		public object Disassemble(object value, ISessionImplementor session) { return value; }
		public object Replace(object original, object target, ISessionImplementor session, object owner) { return original; }

		public new bool Equals(object x, object y) { return object.Equals(x, y); }
		public int GetHashCode(object x) { return x != null ? x.GetHashCode() : 0; }

		public void SetPropertyValue(object component, int property, object value) { throw new InvalidOperationException("Cannot set property of an immutable object"); }

		public void NullSafeSet(IDbCommand cmd, object value, int index, bool[] settable, ISessionImplementor session)
		{
			for (int i = 0; i < PropertyTypes.Length; i++)
			{
				PropertyTypes[i].NullSafeSet(cmd, GetPropertyValue(value, i), index + i, settable, session);
			}
		}

		public abstract object GetPropertyValue(object component, int property);
		public abstract object NullSafeGet(IDataReader dr, string[] names, ISessionImplementor session, object owner);
	}
}
