using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Routine.Core;
using Routine.Core.Service;

namespace Routine.Soa.Context
{
	public class MemberInterceptionContext : ObjectReferenceInterceptionContext
	{
		public MemberInterceptionContext(ICoreContext coreContext, ObjectReferenceData targetReference, string memberModelId)
			: base(coreContext, targetReference)
		{
			MemberModelId = memberModelId;
		}

		public string MemberModelId { get; private set; }

		public DomainMember TargetMember { get { return TargetType.Member[MemberModelId]; } }
	}
}
