using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Engine.Reflection;
using Routine.Engine.Virtual;

namespace Routine.Engine.Configuration
{
	public partial class OperationBuilder
	{
		private readonly IType parentType;

		public OperationBuilder(IType parentType)
		{
			this.parentType = parentType;
		}

		public IType ParentType { get { return parentType; } }

		public IEnumerable<IOperation> Proxy<T>(T target) { return Proxy<T>().Target(target); }

		public ProxyOperationBuilder<T> Proxy<T>() { return Proxy<T>(m => true); }
		public ProxyOperationBuilder<T> Proxy<T>(string targetMethodName) { return Proxy<T>(m => m.Name == targetMethodName); }
		public ProxyOperationBuilder<T> Proxy<T>(Func<MethodInfo, bool> targetMethodPredicate)
		{
			return new ProxyOperationBuilder<T>(parentType, type.of<T>().GetAllMethods().Where(targetMethodPredicate))
				.Name.Set(c => c.By(o => o.Name))
				.NextLayer();
		}

		private VirtualOperation Virtual()
		{
			return new VirtualOperation(parentType);
		}

		public VirtualOperation Virtual(string name)
		{
			return Virtual()
				.Name.Set(name)
				.ReturnType.Set(type.ofvoid())
			;
		}

		public VirtualOperation Virtual<T>(string name)
		{
			return Virtual()
				.Name.Set(name)
				.ReturnType.Set(type.of<T>())
			;
		}

		public VirtualOperation Virtual(string name, Action body)
		{
			return Virtual(name)
				.Body.Set((target, parameters) =>
				{
					body();
					return null;
				})
			;
		}

		public VirtualOperation Virtual<TReturn>(string name, Func<TReturn> body)
		{
			return Virtual()
				.Name.Set(name)
				.ReturnType.Set(type.of<TReturn>())
				.Body.Set((target, parameters) => (object)body())
			;
		}
	}
}