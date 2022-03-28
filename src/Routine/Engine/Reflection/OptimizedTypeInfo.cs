using Routine.Core.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

namespace Routine.Engine.Reflection
{
    internal class OptimizedTypeInfo : PreloadedTypeInfo
    {
        private IMethodInvoker defaultConstructorInvoker;
        private IMethodInvoker listConstructorInvoker;

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

        internal OptimizedTypeInfo(Type type)
            : base(type) { }

        protected override void Load()
        {
            base.Load();

            if (!type.IsAbstract)
            {
                var defaultConstructor = type.GetConstructor(Type.EmptyTypes);
                if (defaultConstructor != null)
                {
                    defaultConstructorInvoker = defaultConstructor.CreateInvoker();
                }

                var listConstructor = type.GetConstructor(new[] { typeof(int) });
                if (listConstructor != null)
                {
                    listConstructorInvoker = listConstructor.CreateInvoker();
                }
            }

            allConstructors = type.GetConstructors(ALL_INSTANCE).Select(ConstructorInfo.Preloaded).ToArray();

            allProperties = type.GetProperties(ALL_INSTANCE).Select(PropertyInfo.Preloaded).ToArray();
            allPropertiesNameIndex = MemberIndex.Build(allProperties, p => p.Name);

            allStaticProperties = type.GetProperties(ALL_STATIC).Select(PropertyInfo.Preloaded).ToArray();
            allStaticPropertiesNameIndex = MemberIndex.Build(allStaticProperties, p => p.Name);

            allMethods = type.GetMethods(ALL_INSTANCE).Where(m => !m.IsSpecialName).Select(MethodInfo.Preloaded).ToArray();
            allMethodsNameIndex = MemberIndex.Build(allMethods, m => m.Name);

            allStaticMethods = type.GetMethods(ALL_STATIC).Where(m => !m.IsSpecialName).Select(MethodInfo.Preloaded).ToArray();
            allStaticMethodsNameIndex = MemberIndex.Build(allStaticMethods, m => m.Name);

            parseMethod = allStaticMethods.SingleOrDefault(m => m.HasParameters<string>() && m.Returns(this, "Parse"));
        }

        public override ConstructorInfo[] GetAllConstructors() => allConstructors;
        public override PropertyInfo[] GetAllProperties() => allProperties;
        public override PropertyInfo[] GetAllStaticProperties() => allStaticProperties;
        public override MethodInfo[] GetAllMethods() => allMethods;
        public override MethodInfo[] GetAllStaticMethods() => allStaticMethods;
        protected override MethodInfo GetParseMethod() => parseMethod;

        public override PropertyInfo GetProperty(string name) => allPropertiesNameIndex.GetFirstOrDefault(name);
        public override List<PropertyInfo> GetProperties(string name) => allPropertiesNameIndex.GetAll(name);
        public override PropertyInfo GetStaticProperty(string name) => allStaticPropertiesNameIndex.GetFirstOrDefault(name);
        public override List<PropertyInfo> GetStaticProperties(string name) => allStaticPropertiesNameIndex.GetAll(name);
        public override MethodInfo GetMethod(string name) => allMethodsNameIndex.GetFirstOrDefault(name);
        public override List<MethodInfo> GetMethods(string name) => allMethodsNameIndex.GetAll(name);
        public override MethodInfo GetStaticMethod(string name) => allStaticMethodsNameIndex.GetFirstOrDefault(name);
        public override List<MethodInfo> GetStaticMethods(string name) => allStaticMethodsNameIndex.GetAll(name);

        public override object CreateInstance()
        {
            if (defaultConstructorInvoker == null)
            {
                throw new MissingMethodException("Default constructor not found!");
            }

            return defaultConstructorInvoker.Invoke(null);
        }

        public override IList CreateListInstance(int length)
        {
            if (listConstructorInvoker == null)
            {
                throw new MissingMethodException("List constructor not found!");
            }

            return (IList)listConstructorInvoker.Invoke(null, length);
        }
    }
}
