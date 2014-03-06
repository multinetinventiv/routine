using NUnit.Framework;
using Routine.Core;
using Routine.Core.Locator;

namespace Routine.Test.Core.Locator
{
	[TestFixture]
	public class DelegateLocatorTest : CoreTestBase
	{
		public override string[] DomainTypeRootNamespaces{get{return new[]{"Routine.Test.Core.Locator"};}}

		[Test]
		public void Locate_BelirtilenYontemiKullanarakVerilenTypeVeIdyiNesneyeDonusturur()
		{
			var testing = new DelegateLocator((t, id) => "located: " + id);
			var testingInterface = (IOptionalLocator)testing;

			Assert.AreEqual("located: test", testingInterface.Locate(type.of<string>(), "test"));
		}
	}
}

