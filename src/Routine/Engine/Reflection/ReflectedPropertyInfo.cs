using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Routine.Core.Reflection;

namespace Routine.Engine.Reflection
{
	public class ReflectedPropertyInfo : PropertyInfo
	{
		internal ReflectedPropertyInfo(System.Reflection.PropertyInfo propertyInfo)
			: base(propertyInfo) { }

		protected override PropertyInfo Load() { return this; }

		public override MethodInfo GetGetMethod() { return MethodInfo.Reflected(propertyInfo.GetGetMethod(true)); }
		public override MethodInfo GetSetMethod() { return MethodInfo.Reflected(propertyInfo.GetSetMethod(true)); }

		public override ParameterInfo[] GetIndexParameters()
		{
			return propertyInfo.GetIndexParameters().Select(p => ParameterInfo.Reflected(p)).ToArray();
		}

		public override TypeInfo GetFirstDeclaringType()
		{
			if (IsIndexer)
			{
				return DeclaringType;
			}

			var getMethod = GetGetMethod();
			if (getMethod != null)
			{
				return getMethod.GetFirstDeclaringType();
			}

			var setMethod = GetSetMethod();
			if (setMethod != null)
			{
				return setMethod.GetFirstDeclaringType();
			}

			return DeclaringType;
		}

		public override object GetValue(object target, params object[] index)
		{
			return new ReflectionMethodInvoker(propertyInfo.GetGetMethod()).Invoke(target, index);
		}

		public override object GetStaticValue(params object[] index)
		{
			return new ReflectionMethodInvoker(propertyInfo.GetGetMethod()).Invoke(null, index);
		}

		public override void SetValue(object target, object value, params object[] index)
		{
			new ReflectionMethodInvoker(propertyInfo.GetSetMethod()).Invoke(target, Merge(value, index));
		}

		public override void SetStaticValue(object value, params object[] index)
		{
			new ReflectionMethodInvoker(propertyInfo.GetSetMethod()).Invoke(null, Merge(value, index));
		}

		private object[] Merge(object value, object[] index)
		{
			if (index == null) { index = new object[0]; }

			var result = new object[index.Length + 1];

			result[0] = value;

			for (int i = 0; i < index.Length; i++)
			{
				result[i + 1] = index[i];
			}

			return result;
		}

		public override string Name => propertyInfo.Name;
        public override TypeInfo DeclaringType => TypeInfo.Get(propertyInfo.DeclaringType);
        public override TypeInfo ReflectedType => TypeInfo.Get(propertyInfo.ReflectedType);
        public override TypeInfo PropertyType => TypeInfo.Get(propertyInfo.PropertyType);

        public override object[] GetCustomAttributes()
		{
			//propertyInfo.GetCustomAttributes(true) does not retrieve attributes from inherited properties.
			return Attribute.GetCustomAttributes(propertyInfo, true).Cast<object>().ToArray();
		}
		public override object[] GetReturnTypeCustomAttributes() { return GetGetMethod().GetReturnTypeCustomAttributes(); }
	}
}
