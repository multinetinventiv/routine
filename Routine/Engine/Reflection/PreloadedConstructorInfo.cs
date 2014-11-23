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

		internal PreloadedConstructorInfo(System.Reflection.ConstructorInfo constructorInfo)
			: base(constructorInfo) { }

		protected override ConstructorInfo Load()
		{
			isPublic = constructorInfo.IsPublic;
			declaringType = TypeInfo.Get(constructorInfo.DeclaringType);
			reflectedType = TypeInfo.Get(constructorInfo.ReflectedType);

			parameters = constructorInfo.GetParameters().Select(p => ParameterInfo.Preloaded(this, p)).ToArray();
			customAttributes = constructorInfo.GetCustomAttributes(true);

			return this;
		}

		public override bool IsPublic { get { return isPublic; } }
		public override TypeInfo DeclaringType { get { return declaringType; } }
		public override TypeInfo ReflectedType { get { return reflectedType; } }

		public override ParameterInfo[] GetParameters() { return parameters; }
		public override object[] GetCustomAttributes() { return customAttributes; }

		private readonly object invokerLock = new object();
		private IMethodInvoker _invoker;
		private IMethodInvoker Invoker
		{
			get
			{
				if (_invoker == null)
				{
					lock (invokerLock)
					{
						if (_invoker == null)
						{
							_invoker = constructorInfo.CreateInvoker();
						}
					}
				}

				return _invoker;
			}
		}
		public override object Invoke(params object[] parameters)
		{
			return Invoker.Invoke(null, parameters);
		}
	}
}
