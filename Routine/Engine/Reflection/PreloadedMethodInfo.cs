using System.Linq;
using Routine.Core.Reflection;

namespace Routine.Engine.Reflection
{
	public class PreloadedMethodInfo : MethodInfo
	{
		private string name;
		private bool isPublic;
		private TypeInfo declaringType;
		private TypeInfo reflectedType;
		private TypeInfo returnType;
		private ParameterInfo[] parameters;
		private TypeInfo firstDeclaringType;
		private object[] customAttributes;
		private object[] returnTypeCustomAttributes;

		internal PreloadedMethodInfo(System.Reflection.MethodInfo methodInfo)
			: base(methodInfo) {}

		protected override MethodInfo Load()
		{
			name = methodInfo.Name;
			isPublic = methodInfo.IsPublic;
			declaringType = TypeInfo.Get(methodInfo.DeclaringType);
			reflectedType = TypeInfo.Get(methodInfo.ReflectedType);
			returnType = TypeInfo.Get(methodInfo.ReturnType);
			parameters = methodInfo.GetParameters().Select(p => ParameterInfo.Preloaded(this, p)).ToArray();
			firstDeclaringType = SearchFirstDeclaringType();
			customAttributes = methodInfo.GetCustomAttributes(true);
			returnTypeCustomAttributes = methodInfo.ReturnTypeCustomAttributes.GetCustomAttributes(true);

			return this;
		}

		public override string Name { get { return name; } }
		public override bool IsPublic { get { return isPublic; } }
		public override TypeInfo DeclaringType { get { return declaringType; } }
		public override TypeInfo ReflectedType { get { return reflectedType; } }
		public override TypeInfo ReturnType { get { return returnType; } }

		public override ParameterInfo[] GetParameters() {return parameters;}
		public override object[] GetCustomAttributes() { return customAttributes; }
		public override object[] GetReturnTypeCustomAttributes() { return returnTypeCustomAttributes; }

		#region private IMethodInvoker invoker;
		// ReSharper disable InconsistentNaming
		private readonly object invokerLock = new object();
		private IMethodInvoker _invoker;
		private IMethodInvoker invoker
		{
			get
			{
				if (_invoker == null)
				{
					lock (invokerLock)
					{
						if (_invoker == null)
						{
							_invoker = methodInfo.CreateInvoker();
						}
					}
				}

				return _invoker;
			}
		}
		// ReSharper restore InconsistentNaming
		#endregion

		public override object Invoke(object target, params object[] parameters)
		{
			return invoker.Invoke(target, parameters);
		}

		public override object InvokeStatic(params object[] parameters)
		{
			return invoker.Invoke(null, parameters);
		}

		public override TypeInfo GetFirstDeclaringType()
		{
			return firstDeclaringType;
		}
	}
}
