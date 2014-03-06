using System;
using System.Linq;

namespace Routine.Core.Reflection
{
	internal class ReflectedTypeInfo : TypeInfo
	{
		internal ReflectedTypeInfo(Type type) 
			: base(type) {}

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

		protected override TypeInfo[] GetGenericArguments()
		{
			return type.GetGenericArguments().Select(t => Get(t)).ToArray();
		}

		protected override TypeInfo GetElementType()
		{
			return TypeInfo.Get(type.GetElementType());
		}

		protected override TypeInfo[] GetInterfaces()
		{
			return type.GetInterfaces().Select(t => Get(t)).ToArray();
		}

		public override bool CanBe(TypeInfo other)
		{
			return other.GetActualType().IsAssignableFrom(type);
		}

		protected override MethodInfo GetParseMethod(){return null;}

		protected override void Load(){}

		public override string Name { get { return type.Name; } }
		public override string FullName { get { return type.FullName; } }
		public override string Namespace { get { return type.Namespace; } }
		public override TypeInfo BaseType { get { return TypeInfo.Get(type.BaseType); } }

		public override object CreateInstance()
		{
			return Activator.CreateInstance(type);
		}
	}
}
