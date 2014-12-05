using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Engine.Reflection;

namespace Routine.Engine.Configuration
{
	public class OperationBuilder
	{
		private readonly IType parentType;

		public OperationBuilder(IType parentType)
		{
			this.parentType = parentType;
		}

		public IEnumerable<IOperation> Proxy<T>(T target) { return Proxy<T>().Target(target); }

		public ProxyOperationBuilder<T> Proxy<T>() { return Proxy<T>(m => true); }
		public ProxyOperationBuilder<T> Proxy<T>(string methodName) { return Proxy<T>(m => m.Name == methodName); }
		public ProxyOperationBuilder<T> Proxy<T>(Func<MethodInfo, bool> methodPredicate)
		{
			return new ProxyOperationBuilder<T>(parentType, type.of<T>().GetAllMethods().Where(methodPredicate));
		}
	}
}