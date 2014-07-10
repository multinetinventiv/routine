using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core.Reflection.Optimization;

namespace Routine.Core.Reflection
{
	internal class DomainTypeInfo : PreloadedTypeInfo
	{
		private IMethodInvoker defaultConstructorInvoker;
		private ConstructorInfo[] allConstructors;
		private PropertyInfo[] allProperties;
		private PropertyInfo[] allStaticProperties;
		private MethodInfo[] allMethods;
		private MethodInfo[] allStaticMethods;
		private MethodInfo parseMethod;

		private MemberIndex<string, PropertyInfo> allPropertiesNameIndex;
		private MemberIndex<string, PropertyInfo> allStaticPropertiesNameIndex;
		private MemberIndex<string, MethodInfo> allMethodsNameIndex;
		private MemberIndex<string, MethodInfo> allStaticMethodsNameIndex;

		internal DomainTypeInfo(Type type) : base(type)
		{
			IsDomainType = true;
		}

		protected override void Load()
		{
			base.Load();

			var constructor = type.GetConstructor(new Type[0]);
			if(constructor != null && !type.IsAbstract)
			{
				defaultConstructorInvoker = constructor.CreateInvoker();
			}

			allConstructors = type.GetConstructors(ALL_INSTANCE).Select(c => ConstructorInfo.Preloaded(c)).ToArray();

			allProperties = type.GetProperties(ALL_INSTANCE).Select(p => PropertyInfo.Preloaded(p)).ToArray();
			allPropertiesNameIndex = MemberIndex.Build(allProperties, p => p.Name);

			allStaticProperties = type.GetProperties(ALL_STATIC).Select(p => PropertyInfo.Preloaded(p)).ToArray();
			allStaticPropertiesNameIndex = MemberIndex.Build(allStaticProperties, p => p.Name);

			allMethods = type.GetMethods(ALL_INSTANCE).Where(m => !m.IsSpecialName).Select(m => MethodInfo.Preloaded(m)).ToArray();
			allMethodsNameIndex = MemberIndex.Build(allMethods, m => m.Name);

			allStaticMethods = type.GetMethods(ALL_STATIC).Where(m => !m.IsSpecialName).Select(m => MethodInfo.Preloaded(m)).ToArray();
			allStaticMethodsNameIndex = MemberIndex.Build(allStaticMethods, m => m.Name);

			parseMethod = allStaticMethods.SingleOrDefault(m => m.HasParameters<string>() && m.Returns(this, "Parse"));
		}

		public override ConstructorInfo[] GetAllConstructors() { return allConstructors; }
		public override PropertyInfo[] GetAllProperties() { return allProperties; }
		public override PropertyInfo[] GetAllStaticProperties() { return allStaticProperties; }
		public override MethodInfo[] GetAllMethods() { return allMethods; }
		public override MethodInfo[] GetAllStaticMethods() { return allStaticMethods; }
		protected override MethodInfo GetParseMethod() { return parseMethod; }

		public override PropertyInfo GetProperty(string name)
		{
			return allPropertiesNameIndex.GetFirstOrDefault(name);
		}

		public override List<PropertyInfo> GetProperties(string name)
		{
			return allPropertiesNameIndex.GetAll(name);
		}

		public override PropertyInfo GetStaticProperty(string name)
		{
			return allStaticPropertiesNameIndex.GetFirstOrDefault(name);
		}

		public override List<PropertyInfo> GetStaticProperties(string name)
		{
			return allStaticPropertiesNameIndex.GetAll(name);
		}

		public override MethodInfo GetMethod(string name)
		{
			return allMethodsNameIndex.GetFirstOrDefault(name);
		}

		public override List<MethodInfo> GetMethods(string name)
		{
			return allMethodsNameIndex.GetAll(name);
		}

		public override MethodInfo GetStaticMethod(string name)
		{
			return allStaticMethodsNameIndex.GetFirstOrDefault(name);
		}

		public override List<MethodInfo> GetStaticMethods(string name)
		{
			return allStaticMethodsNameIndex.GetAll(name);
		}

		public override object CreateInstance()
		{
			if(defaultConstructorInvoker == null)
			{
				throw new MissingMethodException("Default constructor not found!");
			}

			return defaultConstructorInvoker.Invoke(null);
		}
	}
}
