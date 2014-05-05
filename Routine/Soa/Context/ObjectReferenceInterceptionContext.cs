using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Routine.Core;

namespace Routine.Soa.Context
{
	public class ObjectReferenceInterceptionContext : InterceptionContext
	{
		protected readonly ICoreContext coreContext;

		public ObjectReferenceInterceptionContext(ICoreContext coreContext, ObjectReferenceData targetReference)
		{
			this.coreContext = coreContext;

			TargetReference = targetReference;
		}

		public ObjectReferenceData TargetReference { get; private set; }

		public DomainType TargetType { get { return coreContext.GetDomainType(TargetReference.ViewModelId); } }
	}
}
