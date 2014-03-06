using System.Collections.Generic;
using System.Linq;
using Routine.Core.Reflection;

namespace Routine.Core.Operation
{
	public class MethodOperation : IOperation
	{
		private readonly MethodInfo method;

		public MethodOperation(MethodInfo method) 
		{
			this.method = method; 
		}

		public string Name{ get { return method.Name;}}
		public TypeInfo Type { get { return method.ReturnType;}}

		public List<IParameter> Parameters { get { return method.GetParameters().Select(p => new MethodParameter(p) as IParameter).ToList();}}

		public object PerformOn(object target, params object[] parameters) 
		{
			return method.Invoke(target, parameters);
		}
	}
}
