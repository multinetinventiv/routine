using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Engine.Reflection;
using Routine.Engine.Virtual;

namespace Routine.Engine.Configuration
{
	public partial class MethodBuilder
	{
		private readonly IType parentType;

		public MethodBuilder(IType parentType)
		{
			this.parentType = parentType;
		}

		public IType ParentType { get { return parentType; } }

		public IEnumerable<IMethod> Proxy<T>(T target) { return Proxy<T>().Target(target); }

		public ProxyMethodBuilder<T> Proxy<T>() { return Proxy<T>(m => true); }
		public ProxyMethodBuilder<T> Proxy<T>(string targetMethodName) { return Proxy<T>(m => m.Name == targetMethodName); }
		public ProxyMethodBuilder<T> Proxy<T>(Func<MethodInfo, bool> targetMethodPredicate)
		{
			return new ProxyMethodBuilder<T>(parentType, type.of<T>().GetAllMethods().Where(targetMethodPredicate))
				.Name.Set(c => c.By(o => o.Name))
				.NextLayer();
		}

		private VirtualMethod Virtual()
		{
			return new VirtualMethod(parentType);
		}

		public VirtualMethod Virtual(string name)
		{
			return Virtual()
				.Name.Set(name)
				.ReturnType.Set(type.ofvoid())
			;
		}

		public VirtualMethod Virtual<T>(string name)
		{
			return Virtual()
				.Name.Set(name)
				.ReturnType.Set(type.of<T>())
			;
		}

		public VirtualMethod Virtual(string name, Action body)
		{
			return Virtual(name)
				.Body.Set((target, parameters) =>
				{
					body();
					return null;
				})
			;
		}

		public VirtualMethod Virtual<TReturn>(string name, Func<TReturn> body)
		{
			return Virtual()
				.Name.Set(name)
				.ReturnType.Set(type.of<TReturn>())
				.Body.Set((target, parameters) => (object)body())
			;
		}
	}
}