using System;
using System.Collections;
using System.Linq;

namespace Routine.Engine.Reflection
{
	public abstract class PreloadedTypeInfo : TypeInfo
	{
		private string name;
		private string fullName;
		private string @namespace;
		private TypeInfo baseType;
		private TypeInfo[] genericArguments;
		private TypeInfo[] interfaces;
		private TypeInfo[] assignableTypes;
		private object[] customAttributes;

		public override string Name { get { return name; } }
		public override string FullName { get { return fullName; } }
		public override string Namespace { get { return @namespace; } }
		public override TypeInfo BaseType { get { return baseType; } }

		protected PreloadedTypeInfo(Type type)
			: base(type) { }

		protected override void Load()
		{
			name = type.Name;
			fullName = type.FullName;
			@namespace = type.Namespace;
			baseType = Get(type.BaseType);

			genericArguments = type.GetGenericArguments().Select(t => Get(t)).ToArray();
			interfaces = type.GetInterfaces().Select(t => Get(t)).ToArray();
			assignableTypes = base.GetAssignableTypes();

			customAttributes = type.GetCustomAttributes(true);
		}

		protected override TypeInfo[] GetAssignableTypes()
		{
			return assignableTypes;
		}

		protected override TypeInfo[] GetGenericArguments() { return genericArguments; }
		protected override TypeInfo[] GetInterfaces() { return interfaces; }
		public override bool CanBe(TypeInfo other) { return assignableTypes.Any(t => t == other); }
		protected override TypeInfo GetElementType() { return null; }
		public override ConstructorInfo[] GetAllConstructors() { return new ConstructorInfo[0]; }
		public override PropertyInfo[] GetAllProperties() { return new PropertyInfo[0]; }
		public override PropertyInfo[] GetAllStaticProperties() { return new PropertyInfo[0]; }
		public override MethodInfo[] GetAllMethods() { return new MethodInfo[0]; }
		public override MethodInfo[] GetAllStaticMethods() { return new MethodInfo[0]; }
		public override object[] GetCustomAttributes() { return customAttributes; }

		protected override MethodInfo GetParseMethod() { return null; }

		public override object CreateInstance() { return Activator.CreateInstance(type); }
		public override IList CreateListInstance(int length) { return (IList)Activator.CreateInstance(type, length); }
	}
}

