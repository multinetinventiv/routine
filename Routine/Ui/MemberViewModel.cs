using System.Collections.Generic;
using System.Linq;
using Routine.Client;

namespace Routine.Ui
{
	public class MemberViewModel : ViewModelBase
	{
		public Robject.MemberValue Value { get; private set; }
		public Rmember Member { get { return Value.Member; } }

		public MemberViewModel(IMvcConfiguration configuration, Robject.MemberValue member)
			: base(configuration)
		{
			Value = member;
		}

		public override string SpecificViewName
		{
			get { return string.Format("{0}{1}{2}", Member.Type.Name, Configuration.GetViewNameSeparator(), Id); }
		}

		public string Id { get { return Member.Id; } }
		public string Text { get { return Configuration.GetDisplayName(Member.Id); } }
		public bool IsList { get { return Member.IsList; } }
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

		public bool Is(MemberTypes types)
		{
			return Configuration.GetMemberTypes(this).HasFlag(types);
		}

		public int GetOrder() { return GetOrder(MemberTypes.None); }
		public int GetOrder(MemberTypes memberTypes)
		{
			return Configuration.GetOrder(this, memberTypes);
		}

		public bool MarkedAs(string mark)
		{
			return Member.MarkedAs(mark);
		}
	}
}
