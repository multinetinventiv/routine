using Routine.Core;

namespace Routine.Client;

public abstract class Rparametric
{
    private readonly Dictionary<string, Rparameter> _parameters;

    public Rtype Type { get; }
    public List<List<Rparameter>> Groups { get; }
    public HashSet<string> Marks { get; }

    protected Rparametric(string name, int groupCount, List<ParameterModel> parameterModels, HashSet<string> marks, Rtype type)
    {
        Marks = new(marks);

        Type = type;

        _parameters = new();
        Groups = Enumerable.Range(0, groupCount).Select(_ => new List<Rparameter>()).ToList();

        foreach (var parameterModel in parameterModels)
        {
            _parameters[parameterModel.Name] = new(parameterModel, this);
        }

        foreach (var paramId in _parameters.Keys)
        {
            var param = _parameters[paramId];

            foreach (var group in param.Groups)
            {
                if (group >= Groups.Count)
                {
                    throw new InvalidOperationException(
                        $"Parameter '{param.Name}' has a group '{group}' that does not exist on '{type.Name}.{name}'." +
                        $"There are only {Groups.Count} groups."
                    );
                }

                Groups[group].Add(param);
            }
        }
    }

    public Rapplication Application => Type.Application;
    public Dictionary<string, Rparameter> Parameter => _parameters;
    public List<Rparameter> Parameters => _parameters.Values.ToList();
    public Roperation Operation => this as Roperation;
    public Rinitializer Initializer => this as Rinitializer;

    public bool IsOperation() => this is Roperation;
    public bool IsInitializer() => this is Rinitializer;

    public bool MarkedAs(string mark) => Marks.Contains(mark);
}
