namespace Routine.Engine.Reflection
{
	public abstract class MethodInfo : MethodBase, IMethod
	{
		internal static MethodInfo Reflected(System.Reflection.MethodInfo methodInfo)
		{
			return new ReflectedMethodInfo(methodInfo).Load();
		}

		internal static MethodInfo Preloaded(System.Reflection.MethodInfo methodInfo)
		{
			return new PreloadedMethodInfo(methodInfo).Load();
		}

		protected readonly System.Reflection.MethodInfo methodInfo;

		protected MethodInfo(System.Reflection.MethodInfo methodInfo)
		{
			this.methodInfo = methodInfo;
		}

		public System.Reflection.MethodInfo GetActualMethod()
		{
			return methodInfo;
		}

		protected abstract MethodInfo Load();

		public abstract bool IsStatic { get; }
		public abstract TypeInfo ReturnType { get; }

		public abstract TypeInfo GetFirstDeclaringType();
		public abstract object[] GetReturnTypeCustomAttributes();

		public abstract object Invoke(object target, params object[] parameters);
		public abstract object InvokeStatic(params object[] parameters);

		protected virtual TypeInfo SearchFirstDeclaringType()
		{
			var parameters = GetParameters();
			var result = methodInfo.GetBaseDefinition().DeclaringType;
			foreach (var interfaceType in result.GetInterfaces())
			{
				foreach (var interfaceMethodInfo in interfaceType.GetMethods())
				{
					if (interfaceMethodInfo.Name != methodInfo.Name) { continue; }
					if (interfaceMethodInfo.GetParameters().Length != parameters.Length) { continue; }
					if (parameters.Length == 0) { return TypeInfo.Get(interfaceType); }

					var interfaceMethodParameters = interfaceMethodInfo.GetParameters();
					for (int i = 0; i < parameters.Length; i++)
					{
						if (parameters[i].ParameterType.GetActualType() != interfaceMethodParameters[i].ParameterType)
						{
							break;
						}

						if (i == parameters.Length - 1)
						{
							return TypeInfo.Get(interfaceType);
						}
					}
				}
			}

			return TypeInfo.Get(result);
		}

		#region IReturnable implementation

		IType IReturnable.ReturnType { get { return ReturnType; } }

		#endregion

		#region IMethod implementation

		object IMethod.PerformOn(object target, params object[] parameters)
		{
			if (IsStatic)
			{
				return InvokeStatic(parameters);
			}

			return Invoke(target, parameters);
		}

		IType IMethod.GetDeclaringType(bool firstDeclaringType) { return firstDeclaringType ? GetFirstDeclaringType() : DeclaringType; }

		#endregion
	}
}

