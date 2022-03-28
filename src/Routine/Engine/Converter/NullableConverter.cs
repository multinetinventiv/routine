using System.Collections.Generic;
using System;

namespace Routine.Engine.Converter
{
    public class NullableConverter : ConverterBase<NullableConverter>
    {
        protected override List<IType> GetTargetTypes(IType type)
        {
            if (type == null) { return new List<IType>(); }
            if (!type.IsValueType) { return new List<IType>(); }
            if (type.IsVoid) { return new List<IType>(); }
            if (type.IsGenericType) { return new List<IType>(); }
            if (type is not TypeInfo typeInfo) { return new List<IType>(); }

            return new List<IType> { typeof(Nullable<>).MakeGenericType(typeInfo.GetActualType()).ToTypeInfo() };
        }

        protected override object Convert(object @object, IType from, IType to)
        {
            var targetTypeInfo = (TypeInfo)to;

            return Activator.CreateInstance(targetTypeInfo.GetActualType(), @object);
        }
    }
}
