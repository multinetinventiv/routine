using System;
using System.Collections.Generic;

namespace Routine.Engine.Virtual
{
	public class PropertyAsMethod : IMethod
	{
		private readonly IProperty property;
		private readonly string namePrefix;

		public PropertyAsMethod(IProperty property) : this(property, Constants.PROPERTY_AS_METHOD_DEFAULT_PREFIX) { }
		public PropertyAsMethod(IProperty property, string namePrefix)
		{
			if (namePrefix == null) { throw new ArgumentNullException("namePrefix"); }

			this.property = property;
			this.namePrefix = namePrefix;
		}

		public string Name { get { return property.Name.Prepend(namePrefix); } }
		public object[] GetCustomAttributes() { return property.GetCustomAttributes(); }

		public IType ParentType { get { return property.ParentType; } }
		public IType ReturnType { get { return property.ReturnType; } }
		public object[] GetReturnTypeCustomAttributes() { return property.GetReturnTypeCustomAttributes(); }

		public List<IParameter> Parameters { get { return new List<IParameter>(); } }
		public bool IsPublic { get { return property.IsPublic; } }
		public IType GetDeclaringType(bool firstDeclaringType) { return property.GetDeclaringType(firstDeclaringType); }
		public object PerformOn(object target, params object[] parameters) { return property.FetchFrom(target); }
	}
}
