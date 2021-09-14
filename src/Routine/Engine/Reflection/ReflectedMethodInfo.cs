using System.Linq;
using Routine.Core.Reflection;

namespace Routine.Engine.Reflection
{
	public class ReflectedMethodInfo : MethodInfo
	{
		internal ReflectedMethodInfo(System.Reflection.MethodInfo methodInfo)
			: base(methodInfo) { }

		protected override MethodInfo Load() => this;

        public override ParameterInfo[] GetParameters() => methodInfo.GetParameters().Select(ParameterInfo.Reflected).ToArray();
        public override object[] GetCustomAttributes() => methodInfo.GetCustomAttributes(true);
        public override object[] GetReturnTypeCustomAttributes() => methodInfo.ReturnTypeCustomAttributes.GetCustomAttributes(true);

        public override TypeInfo GetFirstDeclaringType() => SearchFirstDeclaringType();

        public override object Invoke(object target, params object[] parameters) => new ReflectionMethodInvoker(methodInfo).Invoke(target, parameters);
        public override object InvokeStatic(params object[] parameters) => new ReflectionMethodInvoker(methodInfo).Invoke(null, parameters);

        public override string Name => methodInfo.Name;
        public override bool IsPublic => methodInfo.IsPublic;
        public override bool IsStatic => methodInfo.IsStatic;

        public override TypeInfo DeclaringType => TypeInfo.Get(methodInfo.DeclaringType);
        public override TypeInfo ReflectedType => TypeInfo.Get(methodInfo.ReflectedType);
        public override TypeInfo ReturnType => TypeInfo.Get(methodInfo.ReturnType);
    }
}

