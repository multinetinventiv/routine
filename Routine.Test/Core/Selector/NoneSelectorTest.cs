using System;
using NUnit.Framework;
using Routine.Core;
using Routine.Core.Selector;

namespace Routine.Test.Core.Selector
{
	[TestFixture]
	public class NoneSelectorTest : CoreTestBase
	{
		public override string[] DomainTypeRootNamespaces{get{return new[]{"Routine.Test.Core.Selector"};}}

		private class TestClass
		{
			public string Prop1{get;set;}
			public string Prop2{get;set;}
			private string Prop3{get;set;}
		}

		[Test]
		public void Select_ThrowsNoMoreItemsShouldBeSelectedException()
		{
			var testing = new NoneSelector<TypeInfo, IMember>();
			var testingInterface = (IOptionalSelector<TypeInfo, IMember>)testing;

			try
			{
				testingInterface.Select(type.of<TestClass>());
				Assert.Fail("exception not thrown");
			}
			catch(NoMoreItemsShouldBeSelectedException) {}
		}	
	}
}

