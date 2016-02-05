using System.Collections.Generic;
using System.Linq;
using Routine.Client;

namespace Routine.Ui
{
	public class DataViewModel : ViewModelBase
	{
		public Robject.DataValue Value { get; private set; }
		public Rdata Data { get { return Value.Data; } }

		public DataViewModel(IMvcConfiguration configuration, Robject.DataValue data)
			: base(configuration)
		{
			Value = data;
		}

		public override string SpecificViewName
		{
			get { return string.Format("{0}{1}{2}", Data.Type.Name, Configuration.GetViewNameSeparator(), Id); }
		}

		public string Id { get { return Data.Name; } }
		public string Text { get { return Configuration.GetDisplayName(Data.Name); } }
		public bool IsList { get { return Data.IsList; } }
		public bool IsRendered { get { return Configuration.IsRendered(this); } }
		
		public ObjectViewModel Object
		{
			get
			{
				return new ObjectViewModel(Configuration, Value.Get().Object);
			}
		}

		public List<ObjectViewModel> List
		{
			get
			{
				return Value
					.Get().List
					.Select(robj => new ObjectViewModel(Configuration, robj))
					.ToList();
			}
		}

		public bool Is(DataLocations types)
		{
			return Configuration.GetDataLocations(this).HasFlag(types);
		}

		public int GetOrder() { return GetOrder(DataLocations.None); }
		public int GetOrder(DataLocations dataLocations)
		{
			return Configuration.GetOrder(this, dataLocations);
		}

		public bool MarkedAs(string mark)
		{
			return Data.MarkedAs(mark);
		}
	}
}
