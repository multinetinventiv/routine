using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Routine.Test.Core.DomainApi
{
	[TestFixture]
	public class MethodMemberTest : CoreTestBase
	{
		public override string[] DomainTypeRootNamespaces { get { return new[] { "Routine.Test.Core.DomainApi.Domain" }; } }

		[Test][Ignore]
		public void Write_tests()
		{
			Assert.Fail();
		}
	}
}
