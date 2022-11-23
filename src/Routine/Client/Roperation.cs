using Routine.Core;

namespace Routine.Client;

public class Roperation : Rparametric
{
    private readonly OperationModel _model;

    public Rtype ResultType { get; }

    public Roperation(OperationModel model, Rtype type)
        : base(model.Name, model.GroupCount, model.Parameters, model.Marks, type)
    {
        _model = model;

        ResultType = model.Result.IsVoid
            ? Rtype.Void
            : Application[model.Result.ViewModelId];
    }

    public string Name => _model.Name;
    public bool ResultIsVoid => _model.Result.IsVoid;
    public bool ResultIsList => _model.Result.IsList;

    public Rvariable Perform(Robject target, List<Rvariable> parameterVariables)
    {
        var parameterValues = BuildParameters(parameterVariables);

        var resultData = Application.Service.Do(target.ReferenceData, _model.Name, parameterValues);

        return Result(resultData);
    }

    public async Task<Rvariable> PerformAsync(Robject target, List<Rvariable> parameterVariables)
    {
        var parameterValues = BuildParameters(parameterVariables);

        var resultData = await Application.Service.DoAsync(target.ReferenceData, _model.Name, parameterValues);

        return Result(resultData);
    }

    private Dictionary<string, ParameterValueData> BuildParameters(List<Rvariable> parameterVariables)
    {
        var parameterValues = new Dictionary<string, ParameterValueData>();
        foreach (var parameterVariable in parameterVariables)
        {
            var rparam = Parameter[parameterVariable.Name];
            var parameterValue = rparam.CreateParameterValueData(parameterVariable.List);
            parameterValues.Add(rparam.Name, parameterValue);
        }

        return parameterValues;
    }

    private Rvariable Result(VariableData resultData) =>
        ResultIsVoid
            ? new Rvariable(true)
            : new Rvariable(Application, resultData, ResultType.Id);

    #region Equality & Hashcode

    protected bool Equals(Roperation other)
    {
        return Equals(Type, other.Type) && Equals(_model, other._model);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((Roperation)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ (_model != null ? _model.GetHashCode() : 0);
        }
    }

    #endregion
}
