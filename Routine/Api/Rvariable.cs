using System.Collections.Generic;
using System.Linq;
using Routine.Core.Service;
using System;

namespace Routine.Api
{
	public class Rvariable
	{
		private const string ANONYMOUS = "__anonymous__";

		private readonly IFactory factory;

		public Rvariable(IFactory factory) { this.factory = factory; }

		private string name;
		private List<Robject> value;
		private bool list;
		private bool @void;

		internal Rvariable With(ValueData data) 
		{
			return With(ANONYMOUS, data.Values.Select(svd => factory.Create<Robject>().With(svd.Reference, svd.Value)), data.IsList, false);
		}
		internal Rvariable With(ReferenceData data) 
		{
			return With(ANONYMOUS, data.References.Select(ord => factory.Create<Robject>().With(ord)), data.IsList, false);
		}
		internal Rvariable Void() { return With(ANONYMOUS, new List<Robject>(), false, true); }

		internal Rvariable Null() { return Null(ANONYMOUS); }
		internal Rvariable Null(string name) { return WithSingle(name, factory.Create<Robject>().Null()); }
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
				Values = value.Select(robj => new SingleValueData {
					Reference = robj.ObjectReferenceData, 
					Value = robj.Value
				}).ToList()
			};
		}

		internal ReferenceData GetReferenceData()
		{
			return new ReferenceData {
				IsList = list,
				References = value.Select(robj => robj.ObjectReferenceData).ToList()
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
					return factory.Create<Robject>().Null();
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
			return factory.Create<Rvariable>().With(name, value, list, @void);
		}

		public Rvariable ToSingle()
		{
			return factory.Create<Rvariable>().WithSingle(name, Object);
		}

		public Rvariable ToList()
		{
			return factory.Create<Rvariable>().WithList(name, List);
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
	}
}
