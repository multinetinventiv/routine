using System.Linq;
using Routine.Core.Reflection.Optimization;
using System;

namespace Routine.Core.Reflection
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

		internal PreloadedPropertyInfo(System.Reflection.PropertyInfo propertyInfo) 
			: base(propertyInfo) {}

		protected override PropertyInfo Load()
		{
			name = propertyInfo.Name;
			declaringType = TypeInfo.Get(propertyInfo.DeclaringType);
			reflectedType = TypeInfo.Get(propertyInfo.ReflectedType);
			propertyType = TypeInfo.Get(propertyInfo.PropertyType);

			getMethod = propertyInfo.GetGetMethod(true) == null ?null:MethodInfo.Preloaded(propertyInfo.GetGetMethod(true));
			setMethod = propertyInfo.GetSetMethod(true) == null ?null:MethodInfo.Preloaded(propertyInfo.GetSetMethod(true));
			indexParameters = propertyInfo.GetIndexParameters().Select(p => ParameterInfo.Preloaded(p)).ToArray();

			if(IsIndexer)
			{
				firstDeclaringType = DeclaringType;
			}
			else if(getMethod != null)
			{
				firstDeclaringType = getMethod.GetFirstDeclaringType();
			}
			else if(setMethod != null)
			{
				firstDeclaringType = setMethod.GetFirstDeclaringType();
			}
			else
			{
				firstDeclaringType = DeclaringType;
			}

			return this;
		}

		public override string Name{get{return name;}}
		public override TypeInfo DeclaringType{get{return declaringType;}}
		public override TypeInfo ReflectedType{get{return reflectedType;}}
		public override TypeInfo PropertyType{get{return propertyType;}}

		public override MethodInfo GetGetMethod(){return getMethod;}
		public override MethodInfo GetSetMethod(){return setMethod;}
		public override ParameterInfo[] GetIndexParameters() {return indexParameters;}

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
			object[] parameters = new object[index.Length + 1];
			parameters[0] = value;
			for(int i = 0; i < index.Length; i++)
			{
				parameters[i + 1] = index[i];
			}
			setMethod.Invoke(target, parameters);
		}

		public override void SetStaticValue(object value, params object[] index)
		{
			object[] parameters = new object[index.Length + 1];
			parameters[0] = value;
			for(int i = 0; i < index.Length; i++)
			{
				parameters[i + 1] = index[i];
			}
			setMethod.Invoke(null, value, index);
		}
	}
}
