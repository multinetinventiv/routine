using Routine.Client;

namespace Routine.Api
{
	public class MemberCodeModel
	{
		public Rmember Member { get; private set; }

		public ObjectCodeModel ReturnModel { get; private set; }

		public MemberCodeModel(ApplicationCodeModel application, Rmember member)
		{
			Member = member;

			ReturnModel = application.GetModel(member.MemberType, member.IsList);
		}

		public string Id { get { return Member.Id; } }
		public bool IsList { get { return Member.IsList; } }

		public bool MarkedAs(string mark)
		{
			return Member.MarkedAs(mark);
		}
	}
}
