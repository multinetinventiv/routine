using System;
using Routine.Core.Reflection;

namespace Routine.Core.DomainApi
{
	public class PropertyMember : IMember
	{
		private readonly PropertyInfo property;

		public PropertyMember(PropertyInfo property) 
		{
			if (property.IsIndexer) { throw new ArgumentException("Given property cannot be an indexer"); }

			this.property = property;
		}

		public TypeInfo Type { get { return property.ReflectedType; } }
		public string Name { get { return property.Name; } }
		public TypeInfo ReturnType { get { return property.PropertyType; } }

		public object FetchFrom(object target) 
		{
			return property.GetValue(target); 
		}

		public object[] GetCustomAttributes()
		{
			return property.GetCustomAttributes();
		}
	}

	public static class PropertyInfo_PropertyMemberExtensions
	{
		public static IMember ToMember(this PropertyInfo source)
		{
			if (source == null) { return null; }

			return new PropertyMember(source);
		}
	}
}
