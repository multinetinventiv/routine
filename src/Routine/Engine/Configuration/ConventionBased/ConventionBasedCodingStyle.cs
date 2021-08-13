using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core.Configuration;
using Routine.Engine.Virtual;

namespace Routine.Engine.Configuration.ConventionBased
{
    public class ConventionBasedCodingStyle : LayeredBase<ConventionBasedCodingStyle>, ICodingStyle
    {
        private readonly List<IType> types;

        public SingleConfiguration<ConventionBasedCodingStyle, int> MaxFetchDepth { get; private set; }

        public ConventionBasedConfiguration<ConventionBasedCodingStyle, object, IType> Type { get; private set; }
        public ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, bool> TypeIsValue { get; private set; }
        public ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, bool> TypeIsView { get; private set; }
        public ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, IIdExtractor> IdExtractor { get; private set; }
        public ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, IValueExtractor> ValueExtractor { get; private set; }
        public ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, ILocator> Locator { get; private set; }
        public ConventionBasedListConfiguration<ConventionBasedCodingStyle, IType, IConverter> Converters { get; private set; }
        public ConventionBasedListConfiguration<ConventionBasedCodingStyle, IType, object> StaticInstances { get; private set; }
        public ConventionBasedListConfiguration<ConventionBasedCodingStyle, IType, IConstructor> Initializers { get; private set; }
        public ConventionBasedListConfiguration<ConventionBasedCodingStyle, IType, IProperty> Datas { get; private set; }
        public ConventionBasedListConfiguration<ConventionBasedCodingStyle, IType, IMethod> Operations { get; private set; }

        public ConventionBasedConfiguration<ConventionBasedCodingStyle, IProperty, bool> DataFetchedEagerly { get; private set; }

        public ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, string> Module { get; private set; }
        public ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, string> TypeName { get; private set; }
        public ConventionBasedConfiguration<ConventionBasedCodingStyle, IProperty, string> DataName { get; private set; }
        public ConventionBasedConfiguration<ConventionBasedCodingStyle, IMethod, string> OperationName { get; private set; }
        public ConventionBasedConfiguration<ConventionBasedCodingStyle, IParameter, string> ParameterName { get; private set; }

        public ConventionBasedListConfiguration<ConventionBasedCodingStyle, IType, string> TypeMarks { get; private set; }
        public ConventionBasedListConfiguration<ConventionBasedCodingStyle, IConstructor, string> InitializerMarks { get; private set; }
        public ConventionBasedListConfiguration<ConventionBasedCodingStyle, IProperty, string> DataMarks { get; private set; }
        public ConventionBasedListConfiguration<ConventionBasedCodingStyle, IMethod, string> OperationMarks { get; private set; }
        public ConventionBasedListConfiguration<ConventionBasedCodingStyle, IParameter, string> ParameterMarks { get; private set; }

        public ConventionBasedCodingStyle()
        {
            types = new List<IType>();

            MaxFetchDepth = new SingleConfiguration<ConventionBasedCodingStyle, int>(this, "MaxFetchDepth", true);

            Type = new ConventionBasedConfiguration<ConventionBasedCodingStyle, object, IType>(this, "Type");
            TypeIsValue = new ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, bool>(this, "TypeIsValue");
            TypeIsView = new ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, bool>(this, "TypeIsView");
            IdExtractor = new ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, IIdExtractor>(this, "IdExtractor");
            ValueExtractor = new ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, IValueExtractor>(this, "ValueExtractor");
            Locator = new ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, ILocator>(this, "Locator");
            Converters = new ConventionBasedListConfiguration<ConventionBasedCodingStyle, IType, IConverter>(this, "Converters");
            StaticInstances = new ConventionBasedListConfiguration<ConventionBasedCodingStyle, IType, object>(this, "StaticInstances");
            Initializers = new ConventionBasedListConfiguration<ConventionBasedCodingStyle, IType, IConstructor>(this, "Initializers");
            Datas = new ConventionBasedListConfiguration<ConventionBasedCodingStyle, IType, IProperty>(this, "Datas");
            Operations = new ConventionBasedListConfiguration<ConventionBasedCodingStyle, IType, IMethod>(this, "Operations");

            DataFetchedEagerly = new ConventionBasedConfiguration<ConventionBasedCodingStyle, IProperty, bool>(this, "DataFetchedEagerly");

            Module = new ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, string>(this, "Module", true);
            TypeName = new ConventionBasedConfiguration<ConventionBasedCodingStyle, IType, string>(this, "TypeName", true);
            DataName = new ConventionBasedConfiguration<ConventionBasedCodingStyle, IProperty, string>(this, "DataName", true);
            OperationName = new ConventionBasedConfiguration<ConventionBasedCodingStyle, IMethod, string>(this, "OperationName", true);
            ParameterName = new ConventionBasedConfiguration<ConventionBasedCodingStyle, IParameter, string>(this, "ParameterName", true);

            TypeMarks = new ConventionBasedListConfiguration<ConventionBasedCodingStyle, IType, string>(this, "TypeMarks");
            InitializerMarks = new ConventionBasedListConfiguration<ConventionBasedCodingStyle, IConstructor, string>(this, "InitializerMarks");
            DataMarks = new ConventionBasedListConfiguration<ConventionBasedCodingStyle, IProperty, string>(this, "DataMarks");
            OperationMarks = new ConventionBasedListConfiguration<ConventionBasedCodingStyle, IMethod, string>(this, "OperationMarks");
            ParameterMarks = new ConventionBasedListConfiguration<ConventionBasedCodingStyle, IParameter, string>(this, "ParameterMarks");
        }

        public ConventionBasedCodingStyle Merge(ConventionBasedCodingStyle other)
        {
            AddTypes(other.types);

            Type.Merge(other.Type);
            TypeIsValue.Merge(other.TypeIsValue);
            TypeIsView.Merge(other.TypeIsView);
            IdExtractor.Merge(other.IdExtractor);
            ValueExtractor.Merge(other.ValueExtractor);
            Locator.Merge(other.Locator);
            Converters.Merge(other.Converters);
            StaticInstances.Merge(other.StaticInstances);
            Initializers.Merge(other.Initializers);
            Datas.Merge(other.Datas);
            Operations.Merge(other.Operations);

            DataFetchedEagerly.Merge(other.DataFetchedEagerly);

            Module.Merge(other.Module);
            TypeName.Merge(other.TypeName);
            DataName.Merge(other.DataName);
            OperationName.Merge(other.OperationName);
            ParameterName.Merge(other.ParameterName);

            TypeMarks.Merge(other.TypeMarks);
            InitializerMarks.Merge(other.InitializerMarks);
            DataMarks.Merge(other.DataMarks);
            OperationMarks.Merge(other.OperationMarks);
            ParameterMarks.Merge(other.ParameterMarks);

            return this;
        }

        public ConventionBasedCodingStyle RecognizeProxyTypesBy(Func<Type, bool> proxyMatcher, Func<Type, Type> actualTypeGetter)
        {
            TypeInfo.SetProxyMatcher(proxyMatcher, actualTypeGetter);

            return this;
        }

        public ConventionBasedCodingStyle AddTypes(IEnumerable<System.Reflection.Assembly> assemblies) { return AddTypes(assemblies, t => true); }
        public ConventionBasedCodingStyle AddTypes(IEnumerable<System.Reflection.Assembly> assemblies, Func<Type, bool> typeFilter)
        {
            foreach (var assembly in assemblies)
            {
                AddTypes(assembly, typeFilter);
            }

            return this;
        }

        public ConventionBasedCodingStyle AddTypes(System.Reflection.Assembly assembly) { return AddTypes(assembly, t => true); }
        public ConventionBasedCodingStyle AddTypes(System.Reflection.Assembly assembly, Func<Type, bool> typeFilter) { return AddTypes(assembly.GetTypes().Where(typeFilter)); }
        public ConventionBasedCodingStyle AddTypes(IEnumerable<Type> types) { return AddTypes(types.ToArray()); }
        public ConventionBasedCodingStyle AddTypes(params Type[] types)
        {
            types = types.Where(t => !t.IsByRefLike).ToArray();

            TypeInfo.Optimize(types);

            NeedRefresh();

            AddTypes(types.Select(t => TypeInfo.Get(t) as IType));

            AddNullableTypes(types);

            return this;
        }

        public ConventionBasedCodingStyle AddTypes(params IType[] types) { return AddTypes(types.AsEnumerable()); }
        public ConventionBasedCodingStyle AddTypes(IEnumerable<IType> types)
        {
            foreach (var type in types)
            {
                AddType(type);
            }

            return this;
        }

        private void AddType(IType type)
        {
            if (types.Contains(type)) { return; }

            types.Add(type);

            if (type is VirtualType)
            {
                AddTypes(type.AssignableTypes);
            }
        }

        private bool needsRefresh;
        private void NeedRefresh() { needsRefresh = true; }
        private void RefreshIfNecessary()
        {
            if (!needsRefresh) { return; }

            needsRefresh = false;

            //TODO refactor - TypeInfo should handle this by itself. Proxy instances should be given, so that domain type changes affects immediately
            for (int i = 0; i < types.Count; i++)
            {
                var type = types[i];

                var typeInfo = type as TypeInfo;
                if (typeInfo == null) { continue; }

                types[i] = TypeInfo.Get(typeInfo.GetActualType());
            }
        }

        private void AddNullableTypes(Type[] types)
        {
            if (types.Length <= 0) { return; }

            AddTypes(types
                .Where(t => t.IsValueType &&
                            t != typeof(void) &&
                            !(t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>)))
                .Select(t => typeof(Nullable<>).MakeGenericType(t))
            );
        }

        #region ICodingStyle implementation

        int ICodingStyle.GetMaxFetchDepth() { return MaxFetchDepth.Get(); }

        List<IType> ICodingStyle.GetTypes() { RefreshIfNecessary(); return types; }
        bool ICodingStyle.ContainsType(IType type) { RefreshIfNecessary(); return types.Contains(type); }

        IType ICodingStyle.GetType(object @object) { return Type.Get(@object); }

        bool ICodingStyle.IsValue(IType type) { return TypeIsValue.Get(type); }
        bool ICodingStyle.IsView(IType type) { return TypeIsView.Get(type); }
        IIdExtractor ICodingStyle.GetIdExtractor(IType type) { return IdExtractor.Get(type); }
        IValueExtractor ICodingStyle.GetValueExtractor(IType type) { return ValueExtractor.Get(type); }
        ILocator ICodingStyle.GetLocator(IType type) { return Locator.Get(type); }
        List<IConverter> ICodingStyle.GetConverters(IType type) { return Converters.Get(type); }
        List<object> ICodingStyle.GetStaticInstances(IType type) { return StaticInstances.Get(type); }
        List<IConstructor> ICodingStyle.GetInitializers(IType type) { return Initializers.Get(type); }
        List<IProperty> ICodingStyle.GetDatas(IType type) { return Datas.Get(type); }
        List<IMethod> ICodingStyle.GetOperations(IType type) { return Operations.Get(type); }

        bool ICodingStyle.IsFetchedEagerly(IProperty property) { return DataFetchedEagerly.Get(property); }

        string ICodingStyle.GetModule(IType type) { return Module.Get(type); }
        string ICodingStyle.GetName(IType type) { return TypeName.Get(type); }
        string ICodingStyle.GetName(IProperty property) { return DataName.Get(property); }
        string ICodingStyle.GetName(IMethod method) { return OperationName.Get(method); }
        string ICodingStyle.GetName(IParameter parameter) { return ParameterName.Get(parameter); }

        List<string> ICodingStyle.GetMarks(IType type) { return TypeMarks.Get(type); }
        List<string> ICodingStyle.GetMarks(IConstructor constructor) { return InitializerMarks.Get(constructor); }
        List<string> ICodingStyle.GetMarks(IProperty property) { return DataMarks.Get(property); }
        List<string> ICodingStyle.GetMarks(IMethod method) { return OperationMarks.Get(method); }
        List<string> ICodingStyle.GetMarks(IParameter parameter) { return ParameterMarks.Get(parameter); }

        #endregion
    }
}

