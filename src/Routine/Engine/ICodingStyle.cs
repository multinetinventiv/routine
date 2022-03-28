using System.Collections.Generic;

namespace Routine.Engine
{
    public interface ICodingStyle
    {
        int GetMaxFetchDepth();
        List<IType> GetTypes();
        bool ContainsType(IType type);

        IType GetType(object @object);
        bool IsValue(IType type);
        bool IsView(IType type);
        IIdExtractor GetIdExtractor(IType type);
        IValueExtractor GetValueExtractor(IType type);
        ILocator GetLocator(IType type);
        List<IConverter> GetConverters(IType type);
        List<object> GetStaticInstances(IType type);
        List<IConstructor> GetInitializers(IType type);
        List<IProperty> GetDatas(IType type);
        List<IMethod> GetOperations(IType type);

        bool IsFetchedEagerly(IProperty property);

        bool IsOptional(IParameter parameter);
        object GetDefaultValue(IParameter parameter);

        string GetModule(IType type);
        string GetName(IType type);
        string GetName(IProperty type);
        string GetName(IMethod type);
        string GetName(IParameter type);

        List<string> GetMarks(IType type);
        List<string> GetMarks(IConstructor constructor);
        List<string> GetMarks(IProperty property);
        List<string> GetMarks(IMethod method);
        List<string> GetMarks(IParameter parameter);
    }
}
