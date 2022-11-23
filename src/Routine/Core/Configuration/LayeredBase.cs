namespace Routine.Core.Configuration;

public class LayeredBase<TConcrete> : ILayered
    where TConcrete : LayeredBase<TConcrete>
{
    private Layer _currentLayer;

    protected LayeredBase()
    {
        _currentLayer = Layer.LeastSpecific;
    }

    public TConcrete Override(Func<TConcrete, TConcrete> @override)
    {
        var oldLayer = _currentLayer;

        _currentLayer = Layer.MostSpecific;

        @override((TConcrete)this);

        _currentLayer = oldLayer;

        return (TConcrete)this;
    }

    public TConcrete NextLayer()
    {
        _currentLayer = _currentLayer.MoreSpecific();

        return (TConcrete)this;
    }

    #region ILayered implementation

    Layer ILayered.CurrentLayer => _currentLayer;

    #endregion
}
