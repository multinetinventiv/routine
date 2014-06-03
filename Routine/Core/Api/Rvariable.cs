using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core;

namespace Routine.Core.Api
{
	public class Rvariable
	{
		private const string ANONYMOUS = "__anonymous__";

		private readonly IApiContext context;

        public Rvariable(IApiContext context) { this.context = context; }

		private string name;
		private List<Robject> value;
		private bool list;
		private bool @void;

		internal Rvariable With(ValueData data) 
		{
			return With(ANONYMOUS, data.Values.Select(od => context.CreateRobject().With(od)), data.IsList, false);
		}
		internal Rvariable With(ParameterValueData data) 
		{
			return With(ANONYMOUS, data.Values.Select(pd => context.CreateRobject().With(pd)), data.IsList, false);
		}
		internal Rvariable Void() { return With(ANONYMOUS, new List<Robject>(), false, true); }

		internal Rvariable Null() { return Null(ANONYMOUS); }
        internal Rvariable Null(string name) { return WithSingle(name, context.CreateRobject().Null()); }
		public Rvariable WithSingle(string name, Robject single) { return With(name, new []{ single }, false, false); }
		public Rvariable WithList(string name, params Robject[] list) { return WithList(name, (IEnumerable<Robject>)list); }
		public Rvariable WithList(string name, IEnumerable<Robject> list) { return With(name, list, true, false); }
		private Rvariable With(string name, IEnumerable<Robject> value, bool list, bool @void)
		{
			this.name = name;
			this.value = value.ToList();
			this.list = list;
			this.@void = @void;
			return this;
		}

		internal ValueData GetValueData()
		{
			return new ValueData {
				IsList = list, 
				Values = value.Select(robj => new ObjectData {
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
				Values = value.Select(robj => robj.ParameterData).ToList()
			};
		}

		public string Name{get{return name;}}
		public bool IsList{get{return list;}}
		public bool IsVoid {get{return @void;}}
		public bool IsNull {get{return Object.IsNull;}}

		public Robject Object
		{
			get
			{
				if(value.Count <= 0)
				{
                    return context.CreateRobject().Null();
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
            return context.CreateRvariable().With(name, value, list, @void);
		}

		public Rvariable ToSingle()
		{
            return context.CreateRvariable().WithSingle(name, Object);
		}

		public Rvariable ToList()
		{
            return context.CreateRvariable().WithList(name, List);
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
			if(robject.IsNull)
			{
				return default(T);
			}

			return converter(robject);
		}

		public override string ToString()
		{
			return List.ToItemString();
		}
	}
}
