using System.Linq;
using NUnit.Framework;
using Routine.Core;
using Routine.Core.DomainApi;
using Routine.Core.Selector;

namespace Routine.Test.Core.Selector
{
	[TestFixture]
	public class DelegateSelectorTest : CoreTestBase
	{
		public override string[] DomainTypeRootNamespaces{get{return new[]{"Routine.Test.Core.Selector"};}}

		private class TestClass
		{
			public string Prop1{get;set;}
			public string Prop2{get;set;}
			private string Prop3{get;set;}
		}

		[Test]
		public void Selects_directly_using_given_delegate()
		{
			var testing = new DelegateSelector<TypeInfo, IMember>(t => t.GetPublicProperties().Select(p => new PropertyMember(p) as IMember));
			var testingInterface = (IOptionalSelector<TypeInfo, IMember>)testing;

			var actual = testingInterface.Select(type.of<TestClass>());

			Assert.AreEqual(2, actual.Count);
			Assert.AreEqual("Prop1", actual[0].Name);
			Assert.AreEqual("Prop2", actual[1].Name);
		}
	}
}

