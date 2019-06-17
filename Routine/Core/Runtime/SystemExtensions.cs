using System;
using System.Reflection;

namespace Routine.Core.Runtime
{
	public static class SystemExtensions
	{
		//TODO couldn't find a better method, .net 4.5 has a fix for this
		public static void PreserveStackTrace(this Exception ex)
		{
			typeof(Exception).GetMethod("PrepForRemoting",
				BindingFlags.NonPublic | BindingFlags.Instance)
				.Invoke(ex, new object[0]);
		}
	}
}