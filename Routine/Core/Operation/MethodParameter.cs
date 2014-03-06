using System;
using Routine.Core.Reflection;

namespace Routine.Core.Operation
{
	public class MethodParameter : IParameter
	{
		private readonly ParameterInfo parameter;
		public MethodParameter(ParameterInfo parameter) { this.parameter = parameter;}

		public string Name { get{return parameter.Name;}}
		public TypeInfo Type {get{return parameter.ParameterType;}}
		public int Index{get{return parameter.Position;}}
	}
}
