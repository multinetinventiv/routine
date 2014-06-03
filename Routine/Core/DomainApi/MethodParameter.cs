using System;
using Routine.Core.Reflection;

namespace Routine.Core.DomainApi
{
	public class MethodParameter : IParameter
	{
		private readonly IOperation operation;
		private readonly ParameterInfo parameter;

		public MethodParameter(IOperation operation, ParameterInfo parameter) 
		{
			this.operation = operation;
			this.parameter = parameter;
		}

		public IOperation Operation { get { return operation; } }
		public string Name { get{return parameter.Name;}}
		public TypeInfo ParameterType {get{return parameter.ParameterType;}}
		public int Index{get{return parameter.Position;}}

		public object[] GetCustomAttributes()
		{
			return parameter.GetCustomAttributes();
		}
	}
}
