using System;
using System.Linq;
using Routine.Core.Reflection.Optimization;

namespace Routine.Core.Reflection
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

		internal PreloadedMethodInfo(System.Reflection.MethodInfo methodInfo)
			: base(methodInfo) {}

		protected override MethodInfo Load()
		{
			name = methodInfo.Name;
			isPublic = methodInfo.IsPublic;
			declaringType = TypeInfo.Get(methodInfo.DeclaringType);
			reflectedType = TypeInfo.Get(methodInfo.ReflectedType);
			returnType = TypeInfo.Get(methodInfo.ReturnType);
			parameters = methodInfo.GetParameters().Select(p => ParameterInfo.Preloaded(p)).ToArray();
			firstDeclaringType = SearchFirstDeclaringType();


			return this;
		}

		public override string Name { get { return name; } }
		public override bool IsPublic { get { return isPublic; } }
		public override TypeInfo DeclaringType { get { return declaringType; } }
		public override TypeInfo ReflectedType { get { return reflectedType; } }
		public override TypeInfo ReturnType { get { return returnType; } }

		public override ParameterInfo[] GetParameters() {return parameters;}

		private readonly object invokerLock = new object();
		private IMethodInvoker _invoker;
		private IMethodInvoker Invoker
		{
			get
			{
				if(_invoker == null)
				{
					lock(invokerLock)
					{
						if(_invoker == null)
						{
							_invoker = methodInfo.CreateInvoker();
						}
					}
				}

				return _invoker;
			}
		}
		public override object Invoke(object target, params object[] parameters)
		{
			return Invoker.Invoke(target, parameters);
		}

		public override object InvokeStatic(params object[] parameters)
		{
			return Invoker.Invoke(null, parameters);
		}

		public override TypeInfo GetFirstDeclaringType()
		{
			return firstDeclaringType;
		}
	}
}
