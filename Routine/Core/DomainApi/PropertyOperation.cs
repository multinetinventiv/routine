using System.Collections.Generic;
using System.Linq;
using Routine.Core.Reflection;

namespace Routine.Core.DomainApi
{
	public class PropertyOperation : IOperation
	{
		private readonly PropertyInfo propertyInfo;
		private readonly string namePrefix;

		public PropertyOperation(PropertyInfo propertyInfo) : this(propertyInfo, Constants.PROPERTY_OPERATION_DEFAULT_PREFIX) { }
		public PropertyOperation(PropertyInfo propertyInfo, string namePrefix)
		{
			this.propertyInfo = propertyInfo;
			this.namePrefix = namePrefix;
		}

		public List<IParameter> Parameters 
		{ 
			get 
			{
				return propertyInfo.IsIndexer ?
					propertyInfo.GetIndexParameters().Select(p => new ParameterParameter(this, p) as IParameter).ToList() :
					new List<IParameter>();
			} 
		}

		public TypeInfo Type { get { return propertyInfo.ReflectedType; } }
		public TypeInfo ReturnType { get { return propertyInfo.PropertyType; } }
		public string Name { get { return propertyInfo.Name.Prepend(namePrefix); } }
		public object[] GetCustomAttributes() { return propertyInfo.GetCustomAttributes(); }

		public object PerformOn(object target, params object[] parameters)
		{
			return propertyInfo.GetValue(target, parameters);
		}
	}

	public static class PropertyInfo_PropertyOperationExtensions
	{
		public static IOperation ToOperation(this PropertyInfo source) { return source.ToOperation(Constants.PROPERTY_OPERATION_DEFAULT_PREFIX); }
		public static IOperation ToOperation(this PropertyInfo source, string namePrefix)
		{
			if (source == null) { return null; }

			return new PropertyOperation(source, namePrefix);
		}
	}
}
