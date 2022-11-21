using Routine.Core;

namespace Routine.Client;

public class Rvariable
{
    private const string ANONYMOUS = "__anonymous__";

    private readonly string _name;
    private readonly List<Robject> _value;
    private readonly bool _list;
    private readonly bool @_void;

    internal Rvariable() : this(false) { }
    internal Rvariable(bool @void) : this(ANONYMOUS, new List<Robject>(), false, @void) { }
    internal Rvariable(string name) : this(name, new Robject()) { }
    internal Rvariable(Rapplication application, VariableData data, string viewModelId)
        : this(ANONYMOUS, data.Values.Select(od => od == null
            ? new Robject()
            : new Robject(od, application[od.ModelId], application[viewModelId])
        ), data.IsList, false)
    { }
    public Rvariable(string name, Robject single) : this(name, new[] { single }, false, false) { }
    public Rvariable(string name, IEnumerable<Robject> list) : this(name, list, true, false) { }
    private Rvariable(string name, IEnumerable<Robject> value, bool list, bool @void)
    {
        _name = name;
        _value = value.ToList();
        _list = list;
        @_void = @void;
    }

    internal VariableData GetValueData() =>
        new()
        {
            IsList = _list,
            Values = _value.Select(robj => new ObjectData
            {
                Id = robj.Id,
                Display = robj.Display
            }).ToList()
        };

    internal ParameterValueData GetParameterValueData() =>
        new()
        {
            IsList = _list,
            Values = _value.Select(robj => robj.GetParameterData()).ToList()
        };

    public string Name => _name;
    public bool IsList => _list;
    public bool IsVoid => @_void;
    public bool IsNull => Object.IsNull;

    public Robject Object => _value.Count <= 0 ? new Robject() : _value[0];
    public List<Robject> List => _value;

    public Rvariable CreateAlias(string name) => new(name, _value, _list, @_void);

    public Rvariable ToSingle() => new(_name, Object);
    public Rvariable ToList() => new(_name, List);

    public T As<T>(Func<Robject, T> converter) => RobjectAs(Object, converter);
    public List<T> AsList<T>(Func<Robject, T> converter) => List.Select(o => RobjectAs(o, converter)).ToList();

    private T RobjectAs<T>(Robject robject, Func<Robject, T> converter) => robject.IsNull ? default : converter(robject);

    public override string ToString() => IsList ? List.ToItemString() : Object.ToString();
}
