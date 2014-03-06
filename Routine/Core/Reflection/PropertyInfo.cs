using System;

namespace Routine.Core.Reflection
{
	public abstract class PropertyInfo
	{
		internal static PropertyInfo Reflected(System.Reflection.PropertyInfo propertyInfo)
		{
			return new ReflectedPropertyInfo(propertyInfo).Load();
		}

		internal static PropertyInfo Preloaded(System.Reflection.PropertyInfo propertyInfo)
		{
			return new PreloadedPropertyInfo(propertyInfo).Load();
		}

		protected readonly System.Reflection.PropertyInfo propertyInfo;

		protected PropertyInfo(System.Reflection.PropertyInfo propertyInfo)
		{
			this.propertyInfo = propertyInfo;
		}

		public System.Reflection.PropertyInfo GetActualProperty() { return propertyInfo; }

		protected abstract PropertyInfo Load();

		public abstract string Name{ get;}
		public abstract TypeInfo DeclaringType{get;}
		public abstract TypeInfo ReflectedType{get;}
		public abstract TypeInfo PropertyType{get;}

		public abstract MethodInfo GetGetMethod();
		public abstract MethodInfo GetSetMethod();
		public abstract ParameterInfo[] GetIndexParameters();
		public abstract TypeInfo GetFirstDeclaringType();

		public abstract object GetValue(object target, params object[] index);
		public abstract object GetStaticValue(params object[] index);
		public abstract void SetValue(object target, object value, params object[] index);
		public abstract void SetStaticValue(object value, params object[] index);

		public bool IsPubliclyReadable
		{
			get
			{
				return GetGetMethod() != null && GetGetMethod().IsPublic;
			}
		}

		public bool IsPubliclyWritable
		{
			get
			{
				return GetSetMethod() != null && GetSetMethod().IsPublic;
			}
		}

		public bool IsOnReflected() { return IsOnReflected(false);}
		public bool IsOnReflected(bool onlyOnReflected)
		{
			var declaring = DeclaringType;
			if(onlyOnReflected) { declaring = GetFirstDeclaringType(); }

			return declaring == ReflectedType;
		}

		public bool IsWithinRootNamespace() { return IsWithinRootNamespace(false);}
		public bool IsWithinRootNamespace(bool onlyWithinRootNamespace)
		{
			var reflectedNamespace = ReflectedType.Namespace;
			var declaringNamespace = DeclaringType.Namespace;

			if(onlyWithinRootNamespace) { declaringNamespace = GetFirstDeclaringType().Namespace; }

			if(reflectedNamespace == null || declaringNamespace == null) { return false; }

			return declaringNamespace.Before(".") == reflectedNamespace.Before(".");
		}

		public bool IsIndexer { get { return GetIndexParameters().Length > 0; } }

		public bool Returns<T>() { return Returns(type.of<T>()); }
		public bool Returns(TypeInfo returnType)
		{
			return PropertyType.CanBe(returnType);
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
			return PropertyType.CanBeCollection(itemType);
		}

		public bool ReturnsCollection(string name) { return ReturnsCollection<object>(name); }
		public bool ReturnsCollection<T>(string name) { return ReturnsCollection(type.of<T>(), name); }
		public bool ReturnsCollection(TypeInfo itemType, string name)
		{
			return ReturnsCollection(itemType) && Name == name;
		}
	}
}

