using System;
using Routine.Core.Reflection;

namespace Routine.Core.Operation
{
	public class MethodParameter : IParameter
	{
		private readonly MethodOperation method;
		private readonly ParameterInfo parameter;
		public MethodParameter(MethodOperation method, ParameterInfo parameter) 
		{
			this.method = method;
			this.parameter = parameter;
		}

		public IOperation Operation { get { return method; } }
		public string Name { get{return parameter.Name;}}
		public TypeInfo ParameterType {get{return parameter.ParameterType;}}
		public int Index{get{return parameter.Position;}}

		public object[] GetCustomAttributes()
		{
			return parameter.GetCustomAttributes();
		}
	}
}
