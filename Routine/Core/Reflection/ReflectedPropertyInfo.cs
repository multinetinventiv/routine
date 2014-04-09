using System;
using System.Linq;

namespace Routine.Core.Reflection
{
	public class ReflectedPropertyInfo : PropertyInfo
	{
		internal ReflectedPropertyInfo(System.Reflection.PropertyInfo propertyInfo)
			: base(propertyInfo){}

		protected override PropertyInfo Load(){return this;}

		public override MethodInfo GetGetMethod(){return MethodInfo.Reflected(propertyInfo.GetGetMethod());}
		public override MethodInfo GetSetMethod(){return MethodInfo.Reflected(propertyInfo.GetSetMethod());}

		public override ParameterInfo[] GetIndexParameters()
		{
			return propertyInfo.GetIndexParameters().Select(p => ParameterInfo.Reflected(p)).ToArray();
		}

		public override TypeInfo GetFirstDeclaringType()
		{
			if(IsIndexer)
			{
				return DeclaringType;
			}

			var getMethod = GetGetMethod();
			if(getMethod != null)
			{
				return getMethod.GetFirstDeclaringType();
			}

			var setMethod = GetSetMethod();
			if(setMethod != null)
			{
				return setMethod.GetFirstDeclaringType();
			}

			return DeclaringType;
		}

		public override object GetValue(object target, params object[] index)
		{
			return propertyInfo.GetValue(target, index);
		}

		public override object GetStaticValue(params object[] index)
		{
			return propertyInfo.GetValue(null, index);
		}

		public override void SetValue(object target, object value, params object[] index)
		{
			propertyInfo.SetValue(target, value, index);
		}

		public override void SetStaticValue(object value, params object[] index)
		{
			propertyInfo.SetValue(null, value, index);
		}

		public override string Name{get{return propertyInfo.Name;}}
		public override TypeInfo DeclaringType{get{return TypeInfo.Get(propertyInfo.DeclaringType);}}
		public override TypeInfo ReflectedType{get{return TypeInfo.Get(propertyInfo.ReflectedType);}}
		public override TypeInfo PropertyType{get{return TypeInfo.Get(propertyInfo.PropertyType);}}

		public override object[] GetCustomAttributes(){return Attribute.GetCustomAttributes(propertyInfo, true);}
	}
}
