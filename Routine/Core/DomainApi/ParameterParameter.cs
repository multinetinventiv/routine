using System;
using Routine.Core.Reflection;

namespace Routine.Core.DomainApi
{
	public class ParameterParameter : IParameter
	{
		private readonly IParametric owner;
		private readonly ParameterInfo parameter;

		public ParameterParameter(IParametric owner, ParameterInfo parameter) 
		{
			this.owner = owner;
			this.parameter = parameter;
		}

		public TypeInfo Type { get { return owner.Type; } }
		public IParametric Owner { get { return owner; } }
		public string Name { get{return parameter.Name;}}
		public TypeInfo ParameterType {get{return parameter.ParameterType;}}
		public int Index{get{return parameter.Position;}}

		public object[] GetCustomAttributes()
		{
			return parameter.GetCustomAttributes();
		}
	}
}
