using System.Collections;
using System.Linq;
using System;

namespace Routine.Engine.Reflection
{
    public abstract class PreloadedTypeInfo : TypeInfo
    {
        private string name;
        private string fullName;
        private string @namespace;
        private TypeInfo baseType;
        private TypeInfo[] genericArguments;
        private TypeInfo[] interfaces;
        private TypeInfo[] assignableTypes;
        private object[] customAttributes;

        public override string Name => name;
        public override string FullName => fullName;
        public override string Namespace => @namespace;
        public override TypeInfo BaseType => baseType;

        protected PreloadedTypeInfo(Type type)
            : base(type) { }

        protected override void Load()
        {
            name = type.Name;
            fullName = type.FullName;
            @namespace = type.Namespace;
            baseType = Get(type.BaseType);

            genericArguments = type.GetGenericArguments().Select(Get).ToArray();
            interfaces = type.GetInterfaces().Select(Get).ToArray();
            assignableTypes = base.GetAssignableTypes();

            customAttributes = type.GetCustomAttributes(true);
        }

        protected override TypeInfo[] GetAssignableTypes() => assignableTypes;

        protected override TypeInfo[] GetGenericArguments() => genericArguments;
        protected override TypeInfo[] GetInterfaces() => interfaces;
        public override bool CanBe(TypeInfo other) => assignableTypes.Any(t => t == other);
        protected override TypeInfo GetElementType() => null;
        public override ConstructorInfo[] GetAllConstructors() => Array.Empty<ConstructorInfo>();
        public override PropertyInfo[] GetAllProperties() => Array.Empty<PropertyInfo>();
        public override PropertyInfo[] GetAllStaticProperties() => Array.Empty<PropertyInfo>();
        public override MethodInfo[] GetAllMethods() => Array.Empty<MethodInfo>();
        public override MethodInfo[] GetAllStaticMethods() => Array.Empty<MethodInfo>();
        public override object[] GetCustomAttributes() => customAttributes;

        protected override MethodInfo GetParseMethod() => null;

        public override object CreateInstance() => Activator.CreateInstance(type);
        public override IList CreateListInstance(int length) => (IList)Activator.CreateInstance(type, length);
    }
}
