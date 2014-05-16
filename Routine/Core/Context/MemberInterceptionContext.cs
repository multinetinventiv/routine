using System.Linq;

namespace Routine.Core.Context
{
	public class MemberInterceptionContext : ObjectReferenceInterceptionContext
	{
		public MemberInterceptionContext(IObjectService objectService, ObjectReferenceData targetReference, string memberModelId)
			: base(objectService, targetReference)
		{
			MemberModelId = memberModelId;
		}

		public string MemberModelId { get; private set; }

		public MemberModel GetMemberModel() { return GetViewModel().Members.Single(m => m.Id == MemberModelId); }
	}
}
