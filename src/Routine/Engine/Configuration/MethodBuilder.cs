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

		public IType ParentType => parentType;

        public IEnumerable<IMethod> Proxy<T>(T target) => Proxy<T>().Target(target);

        public ProxyMethodBuilder<T> Proxy<T>() => Proxy<T>(_ => true);
        public ProxyMethodBuilder<T> Proxy<T>(string targetMethodName) => Proxy<T>(m => m.Name == targetMethodName);
        public ProxyMethodBuilder<T> Proxy<T>(Func<MethodInfo, bool> targetMethodPredicate) =>
            new ProxyMethodBuilder<T>(parentType, type.of<T>().GetAllMethods().Where(targetMethodPredicate))
                .Name.Set(c => c.By(o => o.Name))
                .NextLayer();

        private VirtualMethod Virtual() => new(parentType);

        public VirtualMethod Virtual(string name) => 
            Virtual()
                .Name.Set(name)
                .ReturnType.Set(type.ofvoid());

        public VirtualMethod Virtual<T>(string name) =>
            Virtual()
                .Name.Set(name)
                .ReturnType.Set(type.of<T>());

        public VirtualMethod Virtual(string name, Action body) =>
            Virtual(name)
                .Body.Set((_, _) =>
                {
                    body();
                    return null;
                });

        public VirtualMethod Virtual<TReturn>(string name, Func<TReturn> body) =>
            Virtual()
                .Name.Set(name)
                .ReturnType.Set(type.of<TReturn>())
                .Body.Set((_, _) => body());
    }
}