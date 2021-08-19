using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core;

namespace Routine.Client
{
	public class Rvariable
	{
		private const string ANONYMOUS = "__anonymous__";

		private readonly string name;
		private readonly List<Robject> value;
		private readonly bool list;
		private readonly bool @void;

		internal Rvariable() : this(false) { }
		internal Rvariable(bool @void) : this(ANONYMOUS, new List<Robject>(), false, @void) { }
		internal Rvariable(string name) : this(name, new Robject()) { }
		internal Rvariable(Rapplication application, VariableData data, string viewModelId)
			: this(ANONYMOUS, data.Values.Select(od => od == null 
                ? new Robject() 
                : new Robject(od, application[od.ModelId], application[viewModelId])
            ), data.IsList, false) { }
		public Rvariable(string name, Robject single) : this(name, new[] { single }, false, false) { }
		public Rvariable(string name, IEnumerable<Robject> list) : this(name, list, true, false) { }
		private Rvariable(string name, IEnumerable<Robject> value, bool list, bool @void)
		{
			this.name = name;
			this.value = value.ToList();
			this.list = list;
			this.@void = @void;
		}

		internal VariableData GetValueData() =>
            new()
            {
                IsList = list,
                Values = value.Select(robj => new ObjectData
                {
                    Id = robj.Id,
                    Display = robj.Display
                }).ToList()
            };

        internal ParameterValueData GetParameterValueData() =>
            new()
            {
                IsList = list,
                Values = value.Select(robj => robj.GetParameterData()).ToList()
            };

        public string Name => name;
        public bool IsList => list;
        public bool IsVoid => @void;
        public bool IsNull => Object.IsNull;

        public Robject Object => value.Count <= 0 ? new Robject() : value[0];
        public List<Robject> List => value;

        public Rvariable CreateAlias(string name) => new(name, value, list, @void);

        public Rvariable ToSingle() => new(name, Object);
        public Rvariable ToList() => new(name, List);

        public T As<T>(Func<Robject, T> converter) => RobjectAs(Object, converter);
        public List<T> AsList<T>(Func<Robject, T> converter) => List.Select(o => RobjectAs(o, converter)).ToList();

        private T RobjectAs<T>(Robject robject, Func<Robject, T> converter) => robject.IsNull ? default : converter(robject);

        public override string ToString() => IsList ? List.ToItemString() : Object.ToString();
    }
}
