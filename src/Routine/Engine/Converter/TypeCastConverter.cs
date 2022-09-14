namespace Routine.Engine.Converter;

public class TypeCastConverter : ConverterBase<TypeCastConverter>
{
    private readonly Func<IType, bool> viewTypePredicate;

    public TypeCastConverter(Func<IType, bool> viewTypePredicate)
    {
        this.viewTypePredicate = viewTypePredicate;
    }

    protected override List<IType> GetTargetTypes(IType type) => type.AssignableTypes.Where(viewTypePredicate).ToList();

    protected override object Convert(object @object, IType from, IType to)
    {
        if (!viewTypePredicate(to))
        {
            throw new CannotConvertException(@object, to);
        }

        return from.Cast(@object, to);
    }
}
