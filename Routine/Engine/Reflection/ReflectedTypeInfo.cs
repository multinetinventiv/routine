using System;
using System.Collections;
using System.Linq;

namespace Routine.Engine.Reflection
{
	internal class ReflectedTypeInfo : TypeInfo
	{
		internal ReflectedTypeInfo(Type type)
			: base(type) { }

		public override ConstructorInfo[] GetAllConstructors()
		{
			return type.GetConstructors(ALL_INSTANCE).Select(c => ConstructorInfo.Reflected(c)).ToArray();
		}

		public override PropertyInfo[] GetAllProperties()
		{
			return type.GetProperties(ALL_INSTANCE).Select(p => PropertyInfo.Reflected(p)).ToArray();
		}

		public override PropertyInfo[] GetAllStaticProperties()
		{
			return type.GetProperties(ALL_STATIC).Select(p => PropertyInfo.Reflected(p)).ToArray();
		}

		public override MethodInfo[] GetAllMethods()
		{
			return type.GetMethods(ALL_INSTANCE).Where(m => !m.IsSpecialName).Select(m => MethodInfo.Reflected(m)).ToArray();
		}

		public override MethodInfo[] GetAllStaticMethods()
		{
			return type.GetMethods(ALL_STATIC).Where(m => !m.IsSpecialName).Select(m => MethodInfo.Reflected(m)).ToArray();
		}

		public override object[] GetCustomAttributes()
		{
			return type.GetCustomAttributes(true);
		}

		protected override TypeInfo[] GetGenericArguments()
		{
			return type.GetGenericArguments().Select(t => Get(t)).ToArray();
		}

		protected override TypeInfo GetElementType()
		{
			return Get(type.GetElementType());
		}

		protected override TypeInfo[] GetInterfaces()
		{
			return type.GetInterfaces().Select(t => Get(t)).ToArray();
		}

		public override bool CanBe(TypeInfo other)
		{
			return other.GetActualType().IsAssignableFrom(type);
		}

		protected override TypeInfo[] GetConvertibleTypes() { throw new NotImplementedException(); }

		protected override MethodInfo GetParseMethod() { return null; }

		protected override void Load() { }

		public override string Name { get { return type.Name; } }
		public override string FullName { get { return type.FullName; } }
		public override string Namespace { get { return type.Namespace; } }
		public override TypeInfo BaseType { get { return Get(type.BaseType); } }

		public override object CreateInstance()
		{
			return Activator.CreateInstance(type);
		}

		public override IList CreateListInstance(int length)
		{
			return (IList)Activator.CreateInstance(type, length);
		}
	}
}
