using System.Collections.Generic;
using System.Linq;
using Routine.Client;
using Routine.Engine;

namespace Routine.Api
{
	public class MemberCodeModel
	{
		private readonly ApplicationCodeModel application;
		public Rmember Member { get; private set; }

		public TypeCodeModel ReturnModel { get; private set; }

		public MemberCodeModel(ApplicationCodeModel application, Rmember member)
		{
			this.application = application;
			Member = member;

			ReturnModel = application.GetModel(member.MemberType, member.IsList);
		}

		public ApplicationCodeModel Application { get { return application; } }
		public string Id { get { return Member.Id; } }
		public bool IsList { get { return Member.IsList; } }

		public string GetName(int mode)
		{
			return application.Configuration.GetName(this, mode);
		}

		public List<IType> GetAttributes(int mode)
		{
			return application.Configuration.GetAttributes(this, mode);
		}

		public string RenderAttributes(int mode)
		{
			return string.Join("\r\n", GetAttributes(mode).Select(t => string.Format("[{0}]", t.ToCSharpString())));
		}

		public bool MarkedAs(string mark)
		{
			return Member.MarkedAs(mark);
		}

		#region Equality & Hashcode

		protected bool Equals(MemberCodeModel other)
		{
			return Equals(Member, other.Member);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((MemberCodeModel)obj);
		}

		public override int GetHashCode()
		{
			return (Member != null ? Member.GetHashCode() : 0);
		}

		#endregion
	}
}
