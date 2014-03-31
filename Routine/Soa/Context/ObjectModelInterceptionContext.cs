using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Routine.Core;

namespace Routine.Soa.Context
{
	public class ObjectModelInterceptionContext : InterceptionContext
	{
		protected readonly ICoreContext coreContext;

		public ObjectModelInterceptionContext(ICoreContext coreContext, string objectModelId)
		{
			this.coreContext = coreContext;

			ObjectModelId = objectModelId;
		}

		public string ObjectModelId{ get; private set; }

		public DomainType DomainType { get { return coreContext.GetDomainType(ObjectModelId); } }
	}
}
