using System;
using Routine.Core.Reflection;

namespace Routine.Core.DomainApi
{
	public class MethodMember : IMember
	{
		private readonly MethodInfo method;

		public MethodMember(MethodInfo method)
		{
			if (!method.HasNoParameters()) { throw new ArgumentException("Given method cannot have a parameter"); }
			if (method.ReturnsVoid()) { throw new ArgumentException("Given method must have a return type"); }

			this.method = method; 
		}

		public TypeInfo Type { get { return method.ReflectedType; } }
		public string Name { get { return method.Name; } }
		public TypeInfo ReturnType{ get { return method.ReturnType; } }

		public object FetchFrom(object target)
		{
			return method.Invoke(target); 
		}
		
		public object[] GetCustomAttributes()
		{
			return method.GetCustomAttributes();
		}
	}

	public static class MethodInfo_MethodMemberExtensions
	{
		public static IMember ToMember(this MethodInfo source)
		{
			if (source == null) { return null; }

			return new MethodMember(source);
		}
	}
}
