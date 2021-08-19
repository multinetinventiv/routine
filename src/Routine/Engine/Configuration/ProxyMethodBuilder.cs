using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core.Configuration;
using Routine.Engine.Virtual;

namespace Routine.Engine.Configuration
{
	public class ProxyMethodBuilder<T> : LayeredBase<ProxyMethodBuilder<T>>
	{
		private readonly IType parentType;
		private readonly IEnumerable<IMethod> methods;

		public ConventionBasedConfiguration<ProxyMethodBuilder<T>, IMethod, string> Name { get; }

		public ProxyMethodBuilder(IType parentType, IEnumerable<IMethod> methods)
		{
			this.parentType = parentType;
			this.methods = methods;

			Name = new ConventionBasedConfiguration<ProxyMethodBuilder<T>, IMethod, string>(this, "Name");
		}

		public IType ParentType => parentType;
        public IEnumerable<IMethod> Methods => methods;

        public IEnumerable<IMethod> TargetBySelf() { return TargetBy(o => (T)o); }
		public IEnumerable<IMethod> Target(T target) { return TargetBy(() => target); }
		public IEnumerable<IMethod> TargetBy(Func<T> targetDelegate) { return TargetBy(_ => targetDelegate()); }
		public IEnumerable<IMethod> TargetBy(Func<object, T> targetDelegate)
		{
			return methods.Select(o => Build(parentType, o, (obj, _) => targetDelegate(obj)));
		}

		public IEnumerable<IMethod> TargetByParameter() { return TargetByParameter(typeof(T).Name.ToLowerInitial()); }
		public IEnumerable<IMethod> TargetByParameter(string parameterName) { return TargetByParameter<T>(parameterName); }
		public IEnumerable<IMethod> TargetByParameter<TConcrete>() where TConcrete : T { return TargetByParameter<TConcrete>(typeof(TConcrete).Name.ToLowerInitial()); }
		public IEnumerable<IMethod> TargetByParameter<TConcrete>(string parameterName) where TConcrete : T
		{
			return methods.Select(o =>
				Build(parentType, o,
					(_, parameters) => parameters[0],
					BuildRoutine.Parameter(o).Virtual()
						.ParameterType.Set(type.of<TConcrete>())
						.Name.Set(parameterName)
				)
			);
		}

		private ProxyMethod Build(IType parentType, IMethod real, Func<object, object[], object> targetDelegate,
			params IParameter[] parameters)
		{
			return new ProxyMethod(parentType, real, targetDelegate, parameters).Name.Set(Name.Get(real));
		}
	}
}