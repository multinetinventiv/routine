using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Routine.Core.Reflection;
using System.Diagnostics;

namespace Routine
{
	public abstract class TypeInfo
	{
		#region Assembly Browsing

		public static List<TypeInfo> GetAllDomainTypes()
		{
			var assemblies = GetAllDomainAssemblies();

			List<TypeInfo> types = new List<TypeInfo>();
			foreach (var assembly in assemblies)
			{
				try
				{
					foreach (var type in assembly.GetTypes().Where(t => t.IsPublic))
					{
						try
						{
							types.Add(TypeInfo.Get(type));
						}
						catch (Exception ex)
						{
							Debug.WriteLine("TypeInfo.GetAllDomainTypes() -> " + ex.Message);
						}
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine("TypeInfo.GetAllDomainTypes() -> " + ex.Message);
				}
			}

			return types;
		}

		private static List<System.Reflection.Assembly> GetAllDomainAssemblies()
		{
			var result = new List<System.Reflection.Assembly>();

			var state = new Dictionary<string, bool>();
			var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => MayHaveDomainTypeRootNamespace(a.GetName()));

			foreach(var assembly in assemblies)
			{
				state.Add(assembly.FullName, true);
			}
			foreach(var assembly in assemblies)
			{
				result.AddRange(GetReferencedAssemblies(assembly, state));
			}

			return result;
		}

		private static List<System.Reflection.Assembly> GetReferencedAssemblies(System.Reflection.Assembly assembly, Dictionary<string, bool> state)
		{
			var result = new List<System.Reflection.Assembly> {assembly};

			var references = assembly
				.GetReferencedAssemblies()
				.Where(an => !state.ContainsKey(an.FullName) && MayHaveDomainTypeRootNamespace(an));

			foreach (var reference in references)
			{
				state.Add(reference.FullName, true);
				System.Reflection.Assembly referenceAssembly = null;
				try 
				{
					referenceAssembly = System.Reflection.Assembly.Load(reference);
				} 
				catch(Exception ex)
				{
					Debug.WriteLine("TypeInfo.GetReferencedAssembly() -> Could not load assembly: " + assembly);
					Debug.WriteLine("Exception Details: " + ex);
				}

				if(referenceAssembly != null)
				{
					result.AddRange(GetReferencedAssemblies(referenceAssembly, state));
				}
			}

			return result;
		}

		public static bool MayHaveDomainTypeRootNamespace(System.Reflection.AssemblyName name)
		{
			return domainTypeRootNamespaces.Any(ns => name.FullName.StartsWith(ns.Before(".")));
		}
		#endregion

		#region Factory Methods

		private static readonly Dictionary<Type, TypeInfo> typeCache = new Dictionary<Type, TypeInfo>();
		private static readonly List<string> domainTypeRootNamespaces;

		private static Func<Type, bool> proxyMatcher;
		private static Func<Type, Type> actualTypeGetter;

		static TypeInfo()
		{
			domainTypeRootNamespaces = new List<string>();
			proxyMatcher = t => false;
			actualTypeGetter = t => t;
		}

		public static void AddDomainTypeRootNamespace(params string[] rootNamespaces)
		{
			domainTypeRootNamespaces.AddRange(rootNamespaces);

			var invalidationList = new List<TypeInfo>();
			foreach(var type in typeCache.Values)
			{
				if(!type.IsDomainType && ShouldBeDomainType(type.type))
				{
					invalidationList.Add(type);
				}
			}

			foreach(var type in invalidationList)
			{
				typeCache.Remove(type.type);
				TypeInfo.Get(type.type);
			}
		}

		public static void SetProxyMatcher(Func<Type, bool> proxyMatcher, Func<Type, Type> actualTypeGetter)
		{
			if (proxyMatcher == null) { throw new ArgumentNullException("matcher"); }
			if (actualTypeGetter == null) { throw new ArgumentNullException("actualTypeGetter"); }

			TypeInfo.proxyMatcher = proxyMatcher;
			TypeInfo.actualTypeGetter = actualTypeGetter;
		}

		private static bool ShouldBeDomainType(Type type)
		{
			return type.Namespace != null && domainTypeRootNamespaces.Any(ns => type.Namespace.StartsWith(ns));
		}

		protected const System.Reflection.BindingFlags ALL_STATIC = System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public;
		protected const System.Reflection.BindingFlags ALL_INSTANCE = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public;

		public static TypeInfo Void()
		{
			return Get(typeof(void));
		}

		public static TypeInfo Get<T>()
		{
			return Get(typeof(T));
		}

		public static TypeInfo Get(Type type)
		{
			if(type == null)
			{
				return null;
			}

			TypeInfo result = null;
			
			if(!typeCache.TryGetValue(type, out result))
			{
				lock(typeCache)
				{
					if(!typeCache.TryGetValue(type, out result))
					{
						if (proxyMatcher(type)) { type = actualTypeGetter(type); }

						if(type == typeof(void))
						{
							result = new VoidTypeInfo();
						}
						else if(type.GetMethod("Parse", new []{typeof(string)}) != null && type.GetMethod("Parse", new []{typeof(string)}).ReturnType == type)
						{
							result = new ParseableTypeInfo(type);
						}
						else if(type.IsArray)
						{
							result = new ArrayTypeInfo(type);
						}
						else if(type.IsEnum)
						{
							result = new EnumTypeInfo(type);
						}
						else if (type.ContainsGenericParameters)
						{
							result = new ReflectedTypeInfo(type);
						}
						else if (ShouldBeDomainType(type))
						{
							result = new DomainTypeInfo(type);
						}
						else
						{
							result = new ReflectedTypeInfo(type);
						}

						typeCache.Add(type, result);

						result.Load();
					}
				}
			}

			return result;
		}

		#endregion

		protected readonly Type type;
		protected TypeInfo(Type type)
		{
			this.type = type;

			IsPublic = type.IsPublic;
			IsAbstract = type.IsAbstract;
			IsInterface = type.IsInterface;
			IsGenericType = type.IsGenericType;
			IsPrimitive = type.IsPrimitive;
		}

		public Type GetActualType(){return type;}

		public bool IsPublic { get; protected set; }
		public bool IsAbstract { get; protected set; }
		public bool IsInterface { get; protected set; }
		public bool IsGenericType { get; protected set; }
		public bool IsPrimitive { get; protected set; }
		
		public bool IsVoid { get; protected set;}
		public bool IsEnum { get; protected set; }
		public bool IsArray { get; protected set; }
		public bool IsDomainType { get; protected set; }

		public abstract string Name{get;}
		public abstract string FullName{get;}
		public abstract string Namespace{get;}
		public abstract TypeInfo BaseType{get;}

		public abstract PropertyInfo[] GetAllProperties();
		public abstract PropertyInfo[] GetAllStaticProperties();
		public abstract MethodInfo[] GetAllMethods();
		public abstract MethodInfo[] GetAllStaticMethods();
		public abstract object[] GetCustomAttributes();
		protected abstract TypeInfo[] GetGenericArguments();
		protected abstract TypeInfo GetElementType();
		protected abstract TypeInfo[] GetInterfaces();
		public abstract bool CanBe(TypeInfo other);

		protected abstract MethodInfo GetParseMethod();
		protected abstract void Load();

		public abstract object CreateInstance();

		public virtual ICollection<PropertyInfo> GetPublicProperties() { return GetPublicProperties(false);}
		public virtual ICollection<PropertyInfo> GetPublicProperties(bool onlyPublicReadableAndWritables)
		{
			if(onlyPublicReadableAndWritables)
			{
				return GetAllProperties().Where(p => p.IsPubliclyReadable && p.IsPubliclyWritable).ToList();
			}

			return GetAllProperties().Where(p => p.IsPubliclyReadable).ToList();
		}

		public virtual ICollection<PropertyInfo> GetPublicStaticProperties(){return GetPublicStaticProperties(false);}
		public virtual ICollection<PropertyInfo> GetPublicStaticProperties(bool onlyPublicReadableAndWritables)
		{
			if(onlyPublicReadableAndWritables)
			{
				return GetAllStaticProperties().Where(p => p.IsPubliclyReadable && p.IsPubliclyWritable).ToList();
			}

			return GetAllStaticProperties().Where(p => p.IsPubliclyReadable).ToList();
		}

		public virtual PropertyInfo GetProperty(string name)
		{
			return GetAllProperties().SingleOrDefault(p => p.Name == name);
		}

		public virtual List<PropertyInfo> GetProperties(string name)
		{
			return GetAllProperties().Where(p => p.Name == name).ToList();
		}

		public virtual PropertyInfo GetStaticProperty(string name)
		{
			return GetAllStaticProperties().SingleOrDefault(p => p.Name == name);
		}

		public virtual List<PropertyInfo> GetStaticProperties(string name)
		{
			return GetAllStaticProperties().Where(p => p.Name == name).ToList();
		}

		public virtual ICollection<MethodInfo> GetPublicMethods()
		{
			return GetAllMethods().Where(m => m.IsPublic).ToList();
		}

		public virtual ICollection<MethodInfo> GetPublicStaticMethods()
		{
			return GetAllStaticMethods().Where(m => m.IsPublic).ToList();
		}

		public virtual MethodInfo GetMethod(string name)
		{
			return GetAllMethods().SingleOrDefault(m => m.Name == name);
		}

		public virtual List<MethodInfo> GetMethods(string name)
		{
			return GetAllMethods().Where(m => m.Name == name).ToList();
		}

		public virtual MethodInfo GetStaticMethod(string name)
		{
			return GetAllStaticMethods().SingleOrDefault(m => m.Name == name);
		}

		public virtual List<MethodInfo> GetStaticMethods(string name)
		{
			return GetAllStaticMethods().Where(m => m.Name == name).ToList();
		}

		public virtual bool CanBe<T>() 
		{
			return CanBe(TypeInfo.Get<T>()); 
		}

		public virtual bool CanBeCollection() { return CanBeCollection<object>(); }
		public virtual bool CanBeCollection<T>() { return CanBeCollection(TypeInfo.Get<T>()); }
		public virtual bool CanBeCollection(TypeInfo itemType) 
		{
			return CanBe<ICollection>() && 
				(IsGenericType && GetGenericArguments()[0].CanBe(itemType)) ||
				(IsArray && GetElementType().CanBe(itemType));
		}

		public virtual TypeInfo GetItemType()
		{
			if(!CanBeCollection()) { throw new ArgumentException("type");}
			if(IsGenericType) { return GetGenericArguments()[0];}
			if(IsArray){return GetElementType();}

			throw new ArgumentException("type");
		}

		public virtual bool CanParse() { return GetParseMethod() != null; }
		public virtual object Parse(string value) { return GetParseMethod().InvokeStatic(value); }

		public bool Has<TAttribute>() where TAttribute : Attribute { return Has(TypeInfo.Get<TAttribute>()); }
		public bool Has(TypeInfo attributeType)
		{
			return GetCustomAttributes().Any(a => a.GetTypeInfo() == attributeType);
		}

		public override string ToString()
		{
			return type.ToString();
		}

		public override bool Equals(object obj)
		{
			if(obj == null){return false;}

			var typeObj = obj as Type;
			if(typeObj != null){return type == typeObj;}

			if(obj is TypeInfo){return object.ReferenceEquals(this, obj);}

			return false;
		}

		public override int GetHashCode()
		{
			return type.GetHashCode();
		}
	}

	public class NoDomainTypeRootNamespaceIsDefinedException : Exception {}

	public static class type
	{
		public static TypeInfo of<T>()
		{
			return TypeInfo.Get<T>();
		}

		public static TypeInfo ofvoid()
		{
			return TypeInfo.Void();
		}
	}

	public static class TypeInfoObjectExtensions
	{
		public static TypeInfo GetTypeInfo(this object source)
		{
			if(source == null){return null;}

			return TypeInfo.Get(source.GetType());
		}

		public static TypeInfo ToTypeInfo(this Type source)
		{
			return TypeInfo.Get(source);
		}
	}

}

