using NHibernate;
using Routine;

namespace Routine.Test.Domain.NHibernate.UserType
{
	internal class ParseableValueUserType<T> : BaseSimpleImmutableCompositeUserType<T, string>
	{
		public ParseableValueUserType() : base(NHibernateUtil.String) { }

		public override T ToType(string value) { return (T)type.of<T>().Parse(value); }
		public override string ToValueType(T component) { return component.ToString(); }
	}
}
