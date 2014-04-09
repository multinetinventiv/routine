using System;
using Routine.Core.Reflection;

namespace Routine.Core.Member
{
	public class MethodMember : IMember
	{
		private readonly MethodInfo method;

		public MethodMember(MethodInfo method) 
		{
			this.method = method; 
		}

		public TypeInfo Type { get { return method.ReflectedType; } }
		public string Name { get { return method.Name; } }
		public TypeInfo ReturnType{ get { return method.ReturnType; } }

		public bool CanFetchFrom(object target) { return method.HasNoParameters() && !method.ReturnsVoid(); }
		public object FetchFrom(object target)
		{
			return method.Invoke(target); 
		}
		
		public object[] GetCustomAttributes()
		{
			return method.GetCustomAttributes();
		}
	}
}
