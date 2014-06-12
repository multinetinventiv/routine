using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Routine.Core.Reflection;

namespace Routine.Core.DomainApi
{
	public class ConstructorInitializer : IInitializer
	{
		private readonly ConstructorInfo constructor;

		public ConstructorInitializer(ConstructorInfo constructor)
		{
			this.constructor = constructor;
		}

		public TypeInfo Type { get { return constructor.DeclaringType; } }
		public string Name { get { return Constants.INITIALIZER_NAME; } }
		public TypeInfo InitializedType { get { return constructor.DeclaringType; } }

		public List<IParameter> Parameters { get { return constructor.GetParameters().Select(p => new ParameterParameter(this, p) as IParameter).ToList(); } }

		public object Initialize(params object[] parameters)
		{
			return constructor.Invoke(parameters);
		}

		public object[] GetCustomAttributes()
		{
			return constructor.GetCustomAttributes();
		}
	}

	public static class ConstructorInfo_ConstructorInitializerExtensions
	{
		public static IInitializer ToInitializer(this ConstructorInfo source)
		{
			if (source == null) { return null; }

			return new ConstructorInitializer(source);
		}
	}
}
