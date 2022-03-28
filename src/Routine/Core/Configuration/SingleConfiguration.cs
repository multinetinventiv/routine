using System;

namespace Routine.Core.Configuration
{
    public class SingleConfiguration<TConfiguration, TItem>
    {
        private readonly TConfiguration configuration;
        private readonly string name;
        private readonly bool required;
        private bool valueSet;

        public SingleConfiguration(TConfiguration configuration, string name) : this(configuration, name, false) { }
        public SingleConfiguration(TConfiguration configuration, string name, bool required)
        {
            this.configuration = configuration;
            this.name = name;
            this.required = required;
        }

        private TItem value;

        public TConfiguration SetDefault() => Set(default(TItem));
        public TConfiguration Set(Func<TConfiguration, TItem> valueDelegate) => Set(valueDelegate(configuration));
        public TConfiguration Set(TItem value)
        {
            this.value = value;

            valueSet = true;

            return configuration;
        }

        public TItem Get()
        {
            if (required && !valueSet)
            {
                throw new ConfigurationException(name);
            }

            return value;
        }
    }
}
