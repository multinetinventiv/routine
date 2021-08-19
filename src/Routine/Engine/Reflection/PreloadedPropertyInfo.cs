using System;
using System.Linq;

namespace Routine.Engine.Reflection
{
	public class PreloadedPropertyInfo : PropertyInfo
	{
		private string name;
		private TypeInfo declaringType;
		private TypeInfo reflectedType;
		private TypeInfo propertyType;

		private MethodInfo getMethod;
		private MethodInfo setMethod;
		private ParameterInfo[] indexParameters;
		private TypeInfo firstDeclaringType;

		private object[] customAttributes;

		internal PreloadedPropertyInfo(System.Reflection.PropertyInfo propertyInfo)
			: base(propertyInfo) { }

		protected override PropertyInfo Load()
		{
			name = propertyInfo.Name;
			declaringType = TypeInfo.Get(propertyInfo.DeclaringType);
			reflectedType = TypeInfo.Get(propertyInfo.ReflectedType);
			propertyType = TypeInfo.Get(propertyInfo.PropertyType);

			getMethod = propertyInfo.GetGetMethod(true) == null ? null : MethodInfo.Preloaded(propertyInfo.GetGetMethod(true));
			setMethod = propertyInfo.GetSetMethod(true) == null ? null : MethodInfo.Preloaded(propertyInfo.GetSetMethod(true));
			indexParameters = propertyInfo.GetIndexParameters().Select(p => ParameterInfo.Preloaded(this, p)).ToArray();

			if (IsIndexer)
			{
				firstDeclaringType = DeclaringType;
			}
			else if (getMethod != null)
			{
				firstDeclaringType = getMethod.GetFirstDeclaringType();
			}
			else if (setMethod != null)
			{
				firstDeclaringType = setMethod.GetFirstDeclaringType();
			}
			else
			{
				firstDeclaringType = DeclaringType;
			}

			//propertyInfo.GetCustomAttributes(true) does not retrieve attributes from inherited properties.
			customAttributes = Attribute.GetCustomAttributes(propertyInfo, true).Cast<object>().ToArray();

			return this;
		}

		public override string Name => name;
        public override TypeInfo DeclaringType => declaringType;
        public override TypeInfo ReflectedType => reflectedType;
        public override TypeInfo PropertyType => propertyType;

        public override MethodInfo GetGetMethod() { return getMethod; }
		public override MethodInfo GetSetMethod() { return setMethod; }
		public override ParameterInfo[] GetIndexParameters() { return indexParameters; }

		public override TypeInfo GetFirstDeclaringType() { return firstDeclaringType; }

		public override object GetValue(object target, params object[] index)
		{
			return getMethod.Invoke(target, index);
		}

		public override object GetStaticValue(params object[] index)
		{
			return getMethod.InvokeStatic(index);
		}

		public override void SetValue(object target, object value, params object[] index)
		{
			var parameters = new object[index.Length + 1];
			parameters[0] = value;
			for (int i = 0; i < index.Length; i++)
			{
				parameters[i + 1] = index[i];
			}
			setMethod.Invoke(target, parameters);
		}

		public override void SetStaticValue(object value, params object[] index)
		{
			var parameters = new object[index.Length + 1];
			parameters[0] = value;
			for (int i = 0; i < index.Length; i++)
			{
				parameters[i + 1] = index[i];
			}
			setMethod.Invoke(null, value, index);
		}

		public override object[] GetCustomAttributes() { return customAttributes; }

		public override object[] GetReturnTypeCustomAttributes()
		{
			if (getMethod == null)
			{
				return new object[0];
			}

			return getMethod.GetReturnTypeCustomAttributes();
		}
	}
}
