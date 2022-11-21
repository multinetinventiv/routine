namespace Routine.Core.Configuration;

public class SingleConfiguration<TConfiguration, TItem>
{
    private readonly TConfiguration _configuration;
    private readonly string _name;
    private readonly bool _required;
    private bool _valueSet;

    public SingleConfiguration(TConfiguration configuration, string name) : this(configuration, name, false) { }
    public SingleConfiguration(TConfiguration configuration, string name, bool required)
    {
        _configuration = configuration;
        _name = name;
        _required = required;
    }

    private TItem _value;

    public TConfiguration SetDefault() => Set(default(TItem));
    public TConfiguration Set(Func<TConfiguration, TItem> valueDelegate) => Set(valueDelegate(_configuration));
    public TConfiguration Set(TItem value)
    {
        _value = value;

        _valueSet = true;

        return _configuration;
    }

    public TItem Get()
    {
        if (_required && !_valueSet)
        {
            throw new ConfigurationException(_name);
        }

        return _value;
    }
}
