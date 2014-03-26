using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Routine.Core.Reflection.Optimization;

namespace Routine.Test.Core.Reflection.Optimization
{
	[TestFixture]
	public class ReflectionMethodInvokerTest
	{
		public void Throw(Exception ex)
		{
			throw ex;
		}

		[Test]
		public void WhenExceptionOccursInvokerShouldThrowActualException()
		{
			var expected = new Exception("message");

			var testing = new ReflectionMethodInvoker(GetType().GetMethod("Throw"));

			try
			{
				testing.Invoke(this, expected);
				Assert.Fail("exception not thrown");
			}
			catch (Exception actual)
			{
				Assert.AreSame(expected, actual);
			}
		}
	}
}
