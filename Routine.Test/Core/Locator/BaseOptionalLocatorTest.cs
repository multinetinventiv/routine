using System.Collections;
using NUnit.Framework;
using Routine.Core;
using Routine.Core.Locator;

namespace Routine.Test.Core.Locator
{
	[TestFixture]
	public class BaseOptionalLocatorTest : CoreTestBase
	{
		public override string[] DomainTypeRootNamespaces{get{return new[]{"Routine.Test.Core.Locator"};}}

		private class TestLocator : BaseOptionalLocator<TestLocator>
		{
			protected override object Locate(TypeInfo type, string id){ return null; }
		}

		private TestLocator testing;
		private IOptionalLocator testingInterface;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			testingInterface = testing = new TestLocator();
		}

		[Test]
		public void CanLocate_DurumBelirtilmemisseTrueDoner()
		{
			Assert.IsTrue(testingInterface.CanLocate(type.of<string>(), "any"));
		}

		[Test]
		public void CanLocate_TypeVeIdBelirtilmisDurumaUygunsaTrueDoner()
		{
			testing.When((t, id) => t == type.of<string>() && id == "valid");

			Assert.IsTrue(testingInterface.CanLocate(type.of<string>(), "valid"));
		}

		[Test]
		public void CanLocate_TypeVeIdBelirtilmisDurumaUygunDegilseFalseDoner()
		{
			testing.When((t, id) => t == type.of<string>() && id == "valid");

			Assert.IsFalse(testingInterface.CanLocate(type.of<string>(), "invalid"));
		}

		[Test]
		public void Locate_ThrowsCannotLocateExceptionWhenItCantLocateGivenTypeAndId()
		{
			testing.WhenType(t => t != type.of<string>());
			try
			{
				testingInterface.Locate(type.of<string>(), "a_string");
				Assert.Fail("exception not thrown");
			}
			catch(CannotLocateException ex)
			{
				Assert.IsTrue(ex.Message.Contains(type.of<string>().Name) && ex.Message.Contains("a_string"), ex.Message);
			}
		}

		[Test]
		public void Locate_ThrowsCannotLocateExceptionWhenResultIsNullAndLocatorDoesNotAcceptNull()
		{
			testing.AcceptNullResult(true).WhenTypeIs<string>();

			var actual = testingInterface.Locate(type.of<string>(), "dummy");
			Assert.IsNull(actual);

			testing.AcceptNullResult(false).WhenTypeIs<string>();

			try
			{
				testingInterface.Locate(type.of<string>(), "dummy");
				Assert.Fail("exception not thrown");
			}
			catch(CannotLocateException){}
		}

		[Test]
		public void Facade_WhenTypeIs_Generic()
		{
			testing.WhenTypeIs<string>();

			Assert.IsTrue(testingInterface.CanLocate(type.of<string>(), "any"));
			Assert.IsFalse(testingInterface.CanLocate(type.of<int>(), "1"));
		}

		[Test]
		public void Facade_WhenTypeIs()
		{
			testing.WhenTypeIs(type.of<string>());

			Assert.IsTrue(testingInterface.CanLocate(type.of<string>(), "any"));
			Assert.IsFalse(testingInterface.CanLocate(type.of<int>(), "1"));
		}

		[Test]
		public void Facade_WhenTypeCanBe_Generic()
		{
			testing.WhenTypeCanBe<IList>();

			Assert.IsTrue(testingInterface.CanLocate(type.of<string[]>(), "[1,2]"));
			Assert.IsFalse(testingInterface.CanLocate(type.of<string>(), "1"));
		}

		[Test]
		public void Facade_WhenTypeCanBe()
		{
			testing.WhenTypeCanBe(type.of<IList>());

			Assert.IsTrue(testingInterface.CanLocate(type.of<string[]>(), "[1,2]"));
			Assert.IsFalse(testingInterface.CanLocate(type.of<string>(), "1"));
		}

		[Test]
		public void Facade_WhenType()
		{			
			testing.WhenType(t => t == type.of<string>());

			Assert.IsTrue(testingInterface.CanLocate(type.of<string>(), "any"));
			Assert.IsFalse(testingInterface.CanLocate(type.of<int>(), "1"));
		}

		[Test]
		public void Facade_WhenId()
		{			
			testing.WhenId(id => id == "1");

			Assert.IsTrue(testingInterface.CanLocate(type.of<string>(), "1"));
			Assert.IsTrue(testingInterface.CanLocate(type.of<int>(), "1"));
			Assert.IsFalse(testingInterface.CanLocate(type.of<int>(), "2"));
		}
	}
}

