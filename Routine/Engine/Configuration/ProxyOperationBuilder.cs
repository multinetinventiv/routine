using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core.Configuration;
using Routine.Engine.Virtual;

namespace Routine.Engine.Configuration
{
	public class ProxyOperationBuilder<T> : LayeredBase<ProxyOperationBuilder<T>>
	{
		private readonly IType parentType;
		private readonly IEnumerable<IOperation> operations;

		public ConventionalConfiguration<ProxyOperationBuilder<T>, IOperation, string> Name { get; private set; }

		public ProxyOperationBuilder(IType parentType, IEnumerable<IOperation> operations)
		{
			this.parentType = parentType;
			this.operations = operations;

			Name = new ConventionalConfiguration<ProxyOperationBuilder<T>, IOperation, string>(this, "Name");
		}

		public IType ParentType { get { return parentType; } }
		public IEnumerable<IOperation> Operations { get { return operations; } }

		public IEnumerable<IOperation> TargetBySelf() { return TargetBy(o => (T)o); }
		public IEnumerable<IOperation> Target(T target) { return TargetBy(() => target); }
		public IEnumerable<IOperation> TargetBy(Func<T> targetDelegate) { return TargetBy(o => targetDelegate()); }
		public IEnumerable<IOperation> TargetBy(Func<object, T> targetDelegate)
		{
			return operations.Select(o => Build(parentType, o, (obj, parameters) => targetDelegate(obj)));
		}

		public IEnumerable<IOperation> TargetByParameter() { return TargetByParameter(typeof(T).Name.ToLowerInitial()); }
		public IEnumerable<IOperation> TargetByParameter(string parameterName) { return TargetByParameter<T>(parameterName); }
		public IEnumerable<IOperation> TargetByParameter<TConcrete>() where TConcrete : T { return TargetByParameter<TConcrete>(typeof(TConcrete).Name.ToLowerInitial()); }
		public IEnumerable<IOperation> TargetByParameter<TConcrete>(string parameterName) where TConcrete : T
		{
			return operations.Select(o =>
				Build(parentType, o,
					(obj, parameters) => parameters[0],
					BuildRoutine.Parameter(o).Virtual()
						.ParameterType.Set(type.of<TConcrete>())
						.Name.Set(parameterName)
				)
			);
		}

		private ProxyOperation Build(IType parentType, IOperation real, Func<object, object[], object> targetDelegate,
			params IParameter[] parameters)
		{
			return new ProxyOperation(parentType, real, targetDelegate, parameters).Name.Set(Name.Get(real));
		}
	}
}