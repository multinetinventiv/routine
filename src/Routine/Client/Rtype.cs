﻿using Routine.Core;

using static Routine.Constants;

namespace Routine.Client;

public class Rtype
{
    public static readonly Rtype Void = new();

    private readonly ObjectModel _model;

    public Rapplication Application { get; }

    public List<Rtype> ViewTypes { get; }
    public List<Rtype> ActualTypes { get; }
    public Rinitializer Initializer { get; private set; }
    public Dictionary<string, Rdata> Data { get; }
    public Dictionary<string, Roperation> Operation { get; }

    private Rtype() : this(null, new ObjectModel { Id = MODEL_ID_VOID, IsValueModel = true }) { }
    public Rtype(Rapplication application, ObjectModel model)
    {
        Application = application;
        _model = model;

        ViewTypes = new();
        ActualTypes = new();
        Initializer = null;
        Data = new();
        Operation = new();
    }

    internal void Load()
    {
        foreach (var viewModelId in _model.ViewModelIds)
        {
            ViewTypes.Add(Application[viewModelId]);
        }

        foreach (var actualModelId in _model.ActualModelIds)
        {
            ActualTypes.Add(Application[actualModelId]);
        }

        if (_model.Initializer.GroupCount > 0)
        {
            Initializer = new(_model.Initializer, this);
        }

        foreach (var data in _model.Datas)
        {
            Data.Add(data.Name, new(data, this));
        }

        foreach (var operation in _model.Operations)
        {
            Operation.Add(operation.Name, new(operation, this));
        }
    }

    public string Id => _model.Id;
    public string Name => _model.Name;
    public string Module => _model.Module;
    public bool IsValueType => _model.IsValueModel;
    public bool IsViewType => _model.IsViewModel;
    public bool IsVoid => Id == Constants.MODEL_ID_VOID;
    public bool Initializable => Initializer != null;
    public List<Rdata> Datas => Data.Values.ToList();
    public List<Roperation> Operations => Operation.Values.ToList();

    public HashSet<string> Marks => _model.Marks;

    public bool MarkedAs(string mark) => Marks.Contains(mark);

    public bool CanBe(Rtype viewType)
    {
        if (Equals(this, viewType)) { return true; }

        return ViewTypes.Contains(viewType);
    }

    public List<Robject> StaticInstances =>
        _model
            .StaticInstances
            .Select(od => new Robject(od, Application[od.ModelId], this))
            .ToList();

    public Robject Get(string id) => new(id, this);
    public Robject Get(string id, Rtype viewType) => new(id, this, viewType);

    public Robject Init(params Rvariable[] initializationParameters) => Init(initializationParameters.AsEnumerable());
    public Robject Init(IEnumerable<Rvariable> initializationParameters) => new(initializationParameters, this);

    public override string ToString() => _model.Id;

    #region Equality & Hashcode

    protected bool Equals(Rtype other)
    {
        return Equals(_model, other._model);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) { return false; }
        if (ReferenceEquals(this, obj)) { return true; }
        if (obj.GetType() != GetType()) { return false; }

        return Equals((Rtype)obj);
    }

    public override int GetHashCode()
    {
        return (_model != null ? _model.GetHashCode() : 0);
    }

    #endregion
}
