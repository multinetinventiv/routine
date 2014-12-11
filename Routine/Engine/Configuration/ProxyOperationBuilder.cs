using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Engine.Virtual;

namespace Routine.Engine.Configuration
{
	public class ProxyOperationBuilder<T>
	{
		private readonly IType parentType;
		private readonly IEnumerable<IOperation> operations;

		public ProxyOperationBuilder(IType parentType, IEnumerable<IOperation> operations)
		{
			this.parentType = parentType;
			this.operations = operations;
		}

		public IType ParentType { get { return parentType; } }
		public IEnumerable<IOperation> Operations { get { return operations; } }

		public IEnumerable<IOperation> TargetBySelf() { return TargetBy(o => (T)o); }
		public IEnumerable<IOperation> Target(T target) { return TargetBy(() => target); }
		public IEnumerable<IOperation> TargetBy(Func<T> targetDelegate) { return TargetBy(o => targetDelegate()); }
		public IEnumerable<IOperation> TargetBy(Func<object, T> targetDelegate)
		{
			return operations.Select(o => new ProxyOperation(parentType, o, (obj, parameters) => targetDelegate(obj)));
		}

		public IEnumerable<IOperation> TargetByParameter() { return TargetByParameter(typeof(T).Name.ToLowerInitial()); }
		public IEnumerable<IOperation> TargetByParameter(string parameterName) { return TargetByParameter<T>(parameterName); }
		public IEnumerable<IOperation> TargetByParameter<TConcrete>() where TConcrete : T { return TargetByParameter<TConcrete>(typeof(TConcrete).Name.ToLowerInitial()); }
		public IEnumerable<IOperation> TargetByParameter<TConcrete>(string parameterName) where TConcrete : T
		{
			return operations.Select(o =>
				new ProxyOperation(parentType, o,
					(obj, parameters) => parameters[0],
					BuildRoutine.Parameter(o).Virtual()
						.ParameterType.Set(type.of<TConcrete>())
						.Name.Set(parameterName)
				)
			);
		}
	}
}