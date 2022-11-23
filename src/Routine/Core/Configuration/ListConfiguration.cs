namespace Routine.Core.Configuration;

public class ListConfiguration<TConfiguration, TItem>
{
    private readonly TConfiguration _configuration;
    private readonly string _name;
    private readonly List<TItem> _list;

    public ListConfiguration(TConfiguration configuration, string name)
    {
        _configuration = configuration;
        _name = name;

        _list = new();
    }

    public TConfiguration Add(params Func<TConfiguration, TItem>[] itemDelegates) => Add(itemDelegates.Select(d => d(_configuration)));
    public TConfiguration Add(params TItem[] items) => Add(items as IEnumerable<TItem>);
    public TConfiguration Add(Func<TConfiguration, IEnumerable<TItem>> itemsDelegate) => Add(itemsDelegate(_configuration));
    public TConfiguration Add(IEnumerable<TItem> items)
    {
        _list.AddRange(items);

        return _configuration;
    }

    public List<TItem> Get() => _list;

    public TConfiguration Merge(ListConfiguration<TConfiguration, TItem> other)
    {
        _list.AddRange(other._list);

        return _configuration;
    }
}
