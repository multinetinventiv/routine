using System;
using System.Linq;

namespace Routine.Core.Reflection
{
	public abstract class MethodInfo
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

		public abstract string Name { get; }
		public abstract bool IsPublic { get; }
		public abstract TypeInfo DeclaringType { get; }
		public abstract TypeInfo ReflectedType { get; }
		public abstract TypeInfo ReturnType { get; }

		public abstract ParameterInfo[] GetParameters();
		public abstract TypeInfo GetFirstDeclaringType();
		public abstract object[] GetCustomAttributes();
		public abstract object[] GetReturnTypeCustomAttributes();

		public abstract object Invoke(object target, params object[] parameters);
		public abstract object InvokeStatic(params object[] parameters);

		protected virtual TypeInfo SearchFirstDeclaringType()
		{
			var parameters = GetParameters();
			var result = methodInfo.GetBaseDefinition().DeclaringType;
			foreach(var interfaceType in result.GetInterfaces())
			{
				foreach(var interfaceMethodInfo in interfaceType.GetMethods())
				{
					if(interfaceMethodInfo.Name != methodInfo.Name){continue;}
					if(interfaceMethodInfo.GetParameters().Length != parameters.Length){continue;}
					if(parameters.Length == 0){return TypeInfo.Get(interfaceType);}

					var interfaceMethodParameters = interfaceMethodInfo.GetParameters();
					for(int i = 0; i<parameters.Length; i++)
					{
						if(parameters[i].ParameterType.GetActualType() != interfaceMethodParameters[i].ParameterType)
						{
							break;
						}

						if(i == parameters.Length - 1)
						{
							return TypeInfo.Get(interfaceType);
						}
					}
				}
			}

			return TypeInfo.Get(result);
		}

		public bool IsOnReflected() { return IsOnReflected(false); }
		public bool IsOnReflected(bool onlyOnReflected)
		{
			var declaring = DeclaringType;
			if(onlyOnReflected) { declaring = GetFirstDeclaringType(); }

			return declaring == ReflectedType;
		}

		public bool IsWithinRootNamespace() { return IsWithinRootNamespace(false); }
		public bool IsWithinRootNamespace(bool onlyWithinRootNamespace)
		{
			var reflectedNamespace = ReflectedType.Namespace;
			var declaringNamespace = DeclaringType.Namespace;

			if(onlyWithinRootNamespace) { declaringNamespace = GetFirstDeclaringType().Namespace; }

			if(reflectedNamespace == null || declaringNamespace == null) { return false; }

			return declaringNamespace.Before(".") == reflectedNamespace.Before(".");
		}

		public bool HasNoParameters() { return HasParameters(); }
		public bool HasParameters<T>() { return HasParameters(type.of<T>()); }
		public bool HasParameters<T1, T2>() { return HasParameters(type.of<T1>(), type.of<T2>()); }
		public bool HasParameters<T1, T2, T3>() { return HasParameters(type.of<T1>(), type.of<T2>(), type.of<T3>()); }
		public bool HasParameters<T1, T2, T3, T4>() { return HasParameters(type.of<T1>(), type.of<T2>(), type.of<T3>(), type.of<T4>()); }
		public bool HasParameters(params TypeInfo[] parameterTypes)
		{
			if(GetParameters().Length != parameterTypes.Length) { return false; }

			for(int i = 0; i < GetParameters().Length; i++)
			{
				if(!parameterTypes[i].CanBe(GetParameters()[i].ParameterType)) 
				{
					return false; 
				}
			}

			return true;
		}

		public bool ReturnsVoid()
		{
			return ReturnType.IsVoid;
		}

		public bool Returns<T>() { return Returns(type.of<T>()); }
		public bool Returns(TypeInfo returnType)
		{
			return ReturnType.CanBe(returnType);
		}

		public bool Returns<T>(string name) { return Returns(type.of<T>(), name); }
		public bool Returns(TypeInfo returnType, string name)
		{
			return Returns(returnType) && Name == name;
		}

		public bool ReturnsCollection() { return ReturnsCollection<object>(); }
		public bool ReturnsCollection<T>() { return ReturnsCollection(type.of<T>()); }
		public bool ReturnsCollection(TypeInfo itemType)
		{
			return ReturnType.CanBeCollection(itemType);
		}

		public bool ReturnsCollection(string name) { return ReturnsCollection<object>(name); }
		public bool ReturnsCollection<T>(string name) { return ReturnsCollection(type.of<T>(), name); }
		public bool ReturnsCollection(TypeInfo itemType, string name)
		{
			return ReturnsCollection(itemType) && Name == name;
		}

		public bool Has<TAttribute>() where TAttribute : Attribute { return Has(type.of<TAttribute>()); }
		public bool Has(TypeInfo attributeType)
		{
			return GetCustomAttributes().Any(a => a.GetTypeInfo() == attributeType);
		}

		public bool ReturnTypeHas<TAttribute>() where TAttribute : Attribute { return Has(type.of<TAttribute>()); }
		public bool ReturnTypeHas(TypeInfo attributeType)
		{
			return GetReturnTypeCustomAttributes().Any(a => a.GetTypeInfo() == attributeType);
		}
	}
}

