using System;
using Routine.Core.Reflection;

namespace Routine.Core.Member
{
	public class PropertyMember : IMember
	{
		private readonly PropertyInfo property;

		public PropertyMember(PropertyInfo property) 
		{
			this.property = property;
		}

		public TypeInfo Type { get { return property.ReflectedType; } }
		public string Name { get { return property.Name; } }
		public TypeInfo ReturnType { get { return property.PropertyType; } }

		public bool CanFetchFrom(object target) { return !property.IsIndexer; }
		public object FetchFrom(object target) 
		{
			return property.GetValue(target); 
		}
	}
}
