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

		public string Id { get { return Member.Id; } }
		public string Text { get { return Configuration.GetDisplayName(Member.Id); } }

		public bool IsList { get { return Member.IsList; } }
		public bool IsSimple { get { return Configuration.IsSimple(this); } }
		public bool IsTable { get { return Configuration.IsTable(this); } }

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
	}
}
