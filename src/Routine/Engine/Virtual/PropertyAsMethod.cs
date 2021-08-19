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

		public string Name => property.Name.Prepend(namePrefix);
        public object[] GetCustomAttributes() { return property.GetCustomAttributes(); }

		public IType ParentType => property.ParentType;
        public IType ReturnType => property.ReturnType;
        public object[] GetReturnTypeCustomAttributes() { return property.GetReturnTypeCustomAttributes(); }

		public List<IParameter> Parameters => new List<IParameter>();
        public bool IsPublic => property.IsPublic;
        public IType GetDeclaringType(bool firstDeclaringType) { return property.GetDeclaringType(firstDeclaringType); }
		public object PerformOn(object target, params object[] parameters) { return property.FetchFrom(target); }
	}
}
