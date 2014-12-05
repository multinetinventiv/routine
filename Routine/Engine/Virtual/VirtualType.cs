using System;
using System.Collections;
using System.Collections.Generic;
using Routine.Core.Configuration;

namespace Routine.Engine.Virtual
{
	public class VirtualType : IType
	{
		public SingleConfiguration<VirtualType, string> Name { get; private set; }
		public SingleConfiguration<VirtualType, string> Namespace { get; private set; }
		public SingleConfiguration<VirtualType, bool> IsInterface { get; private set; }
		public SingleConfiguration<VirtualType, string> DefaultInstanceId { get; private set; }
		public SingleConfiguration<VirtualType, Func<VirtualObject, string>> ToStringMethod { get; private set; }
		public ListConfiguration<VirtualType, IOperation> Operations { get; private set; }

		public VirtualType()
		{
			Name = new SingleConfiguration<VirtualType, string>(this, "Name", true);
			Namespace = new SingleConfiguration<VirtualType, string>(this, "Namespace", true);
			IsInterface = new SingleConfiguration<VirtualType, bool>(this, "IsInterface");
			DefaultInstanceId = new SingleConfiguration<VirtualType, string>(this, "DefaultInstanceId", true);
			ToStringMethod = new SingleConfiguration<VirtualType, Func<VirtualObject, string>>(this, "ToStringMethod");
			Operations = new ListConfiguration<VirtualType, IOperation>(this, "Operations");
		}

		#region ITypeComponent implementation

		string ITypeComponent.Name { get { return Name.Get(); } }
		IType ITypeComponent.ParentType { get { return null; } }
		object[] ITypeComponent.GetCustomAttributes() { return new object[0]; }

		#endregion

		#region IType implementation

		bool IType.IsPublic { get { return true; } }
		bool IType.IsAbstract { get { return false; } }
		bool IType.IsInterface { get { return IsInterface.Get(); } }
		bool IType.IsValueType { get { return false; } }
		bool IType.IsGenericType { get { return false; } }
		bool IType.IsPrimitive { get { return false; } }
		bool IType.IsVoid { get { return false; } }
		bool IType.IsEnum { get { return false; } }
		bool IType.IsArray { get { return false; } }
		bool IType.IsDomainType { get { return true; } }
		string IType.FullName { get { return string.Format("{0}.{1}", Namespace.Get(), Name.Get()); } }
		string IType.Namespace { get { return Namespace.Get(); } }
		IType IType.BaseType { get { return type.of<object>(); } }
		List<IType> IType.ConvertibleTypes { get { return new List<IType>(); } }
		List<IInitializer> IType.Initializers { get { return new List<IInitializer>(); } }
		List<IMember> IType.Members { get { return new List<IMember>(); } }
		List<IOperation> IType.Operations { get { return Operations.Get(); } }
		List<IType> IType.GetGenericArguments() { return new List<IType>(); }
		IType IType.GetElementType() { return null; }
		IOperation IType.GetParseOperation() { return null; }
		List<string> IType.GetEnumNames() { return new List<string>(); }
		List<object> IType.GetEnumValues() { return new List<object>(); }
		IType IType.GetEnumUnderlyingType() { return null; }
		bool IType.CanBe(IType otherType) { return Equals(this, otherType) || Equals(type.of<object>(), otherType); }
		object IType.Convert(object target, IType otherType) { return target; }
		object IType.CreateInstance() { return new VirtualObject(DefaultInstanceId.Get(), this); }
		IList IType.CreateListInstance(int length) { throw new NotSupportedException("Virtual types does not support list type"); }

		#endregion
	}
}
