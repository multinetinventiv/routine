using System.Collections.Generic;
using System.Linq;
using Routine.Core.Reflection;

namespace Routine.Core.DomainApi
{
	public class MethodOperation : IOperation
	{
		private readonly MethodInfo method;

		public MethodOperation(MethodInfo method) 
		{
			this.method = method; 
		}

		public TypeInfo Type { get { return method.ReflectedType; } }
		public string Name{ get { return method.Name;}}
		public TypeInfo ReturnType { get { return method.ReturnType;}}

		public List<IParameter> Parameters { get { return method.GetParameters().Select(p => new MethodParameter(this, p) as IParameter).ToList();}}

		public object PerformOn(object target, params object[] parameters) 
		{
			return method.Invoke(target, parameters);
		}

		public object[] GetCustomAttributes()
		{
			return method.GetCustomAttributes();
		}
	}

	public static class MethodInfo_MethodOperationExtensions
	{
		public static IOperation ToOperation(this MethodInfo source)
		{
			if (source == null) { return null; }

			return new MethodOperation(source);
		}
	}
}
