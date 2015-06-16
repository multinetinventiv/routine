using System;
using System.Runtime.Serialization;

namespace Routine.Core.Runtime
{
	public static class SystemExtensions
	{
		public static void PreserveStackTrace(this Exception ex)
		{
			var ctx = new StreamingContext(StreamingContextStates.CrossAppDomain);
			var mgr = new ObjectManager(null, ctx);
			var si = new SerializationInfo(ex.GetType(), new FormatterConverter());

			ex.GetObjectData(si, ctx);
			mgr.RegisterObject(ex, 1, si);
			mgr.DoFixups();
		}
	}
}