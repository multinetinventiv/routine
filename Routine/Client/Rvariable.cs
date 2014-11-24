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
		internal Rvariable(Rapplication application, ValueData data)
			: this(ANONYMOUS, data.Values.Select(od =>
				{
					if (od.Reference.IsNull)
					{
						return new Robject();
					}

					return new Robject(od, application[od.Reference.ActualModelId], application[od.Reference.ViewModelId]);
				}), data.IsList, false) { }
		public Rvariable(string name, Robject single) : this(name, new[] { single }, false, false) { }
		public Rvariable(string name, IEnumerable<Robject> list) : this(name, list, true, false) { }
		private Rvariable(string name, IEnumerable<Robject> value, bool list, bool @void)
		{
			this.name = name;
			this.value = value.ToList();
			this.list = list;
			this.@void = @void;
		}

		internal ValueData GetValueData()
		{
			return new ValueData
			{
				IsList = list,
				Values = value.Select(robj => new ObjectData
				{
					Reference = robj.ObjectReferenceData,
					Value = robj.Value
				}).ToList()
			};
		}

		internal ParameterValueData GetParameterValueData()
		{
			return new ParameterValueData
			{
				IsList = list,
				Values = value.Select(robj => robj.GetParameterData()).ToList()
			};
		}

		public string Name { get { return name; } }
		public bool IsList { get { return list; } }
		public bool IsVoid { get { return @void; } }
		public bool IsNull { get { return Object.IsNull; } }

		public Robject Object
		{
			get
			{
				if (value.Count <= 0)
				{
					return new Robject();
				}

				return value[0];
			}
		}

		public List<Robject> List
		{
			get
			{
				return value;
			}
		}

		public Rvariable CreateAlias(string name)
		{
			return new Rvariable(name, value, list, @void);
		}

		public Rvariable ToSingle()
		{
			return new Rvariable(name, Object);
		}

		public Rvariable ToList()
		{
			return new Rvariable(name, List);
		}

		public T As<T>(Func<Robject, T> converter)
		{
			return RobjectAs(Object, converter);
		}

		public List<T> AsList<T>(Func<Robject, T> converter)
		{
			return List.Select(o => RobjectAs(o, converter)).ToList();
		}

		private T RobjectAs<T>(Robject robject, Func<Robject, T> converter)
		{
			if (robject.IsNull)
			{
				return default(T);
			}

			return converter(robject);
		}

		public override string ToString()
		{
			if (IsList)
			{
				return List.ToItemString();
			}

			return Object.ToString();
		}
	}
}
