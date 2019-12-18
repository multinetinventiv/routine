using System.Linq;
using Routine.Core.Reflection;

namespace Routine.Engine.Reflection
{
	public class PreloadedConstructorInfo : ConstructorInfo
	{
		private bool isPublic;
		private TypeInfo declaringType;
		private TypeInfo reflectedType;

		private ParameterInfo[] parameters;
		private object[] customAttributes;

		private IMethodInvoker invoker;

		internal PreloadedConstructorInfo(System.Reflection.ConstructorInfo constructorInfo)
			: base(constructorInfo) { }

		protected override ConstructorInfo Load()
		{
			isPublic = constructorInfo.IsPublic;
			declaringType = TypeInfo.Get(constructorInfo.DeclaringType);
			reflectedType = TypeInfo.Get(constructorInfo.ReflectedType);

			parameters = constructorInfo.GetParameters().Select(p => ParameterInfo.Preloaded(this, p)).ToArray();
			customAttributes = constructorInfo.GetCustomAttributes(true);

			invoker = constructorInfo.CreateInvoker();

			return this;
		}

		public override bool IsPublic { get { return isPublic; } }
		public override TypeInfo DeclaringType { get { return declaringType; } }
		public override TypeInfo ReflectedType { get { return reflectedType; } }

		public override ParameterInfo[] GetParameters() { return parameters; }
		public override object[] GetCustomAttributes() { return customAttributes; }

		
		public override object Invoke(params object[] parameters)
		{
			return invoker.Invoke(null, parameters);
		}
	}
}
