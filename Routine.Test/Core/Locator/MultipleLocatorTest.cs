using System;
using Moq;
using NUnit.Framework;
using Routine.Core;
using Routine.Core.CodingStyle;
using Routine.Core.Locator;

namespace Routine.Test.Core.Locator
{
	[TestFixture]
	public class MultipleLocatorTest : CoreTestBase
	{
		public override string[] DomainTypeRootNamespaces{get{return new[]{"Routine.Test.Core.Locator"};}}

		private Mock<IOptionalLocator> locatorMock1;
		private Mock<IOptionalLocator> locatorMock2;
		private Mock<IOptionalLocator> locatorMock3;

		private MultipleLocator<GenericCodingStyle> testing;
		private ILocator testingInterface;
		private MultipleLocator<GenericCodingStyle> testingOther;
		//private ILocator testingOtherInterface;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			locatorMock1 = new Mock<IOptionalLocator>();
			locatorMock2 = new Mock<IOptionalLocator>();
			locatorMock3 = new Mock<IOptionalLocator>();

			testingInterface = testing = new MultipleLocator<GenericCodingStyle>(new GenericCodingStyle());
			/*testingOtherInterface =*/ testingOther = new MultipleLocator<GenericCodingStyle>(new GenericCodingStyle());

			testing.Add(locatorMock1.Object)
				   .Done(locatorMock2.Object);

			testingOther.Done(locatorMock3.Object);
		}

		private void LocatorMockReturns(Mock<IOptionalLocator> locatorMock, object result)
		{
			locatorMock.Setup(o => o.CanLocate(It.IsAny<TypeInfo>(), It.IsAny<string>())).Returns(true);
			locatorMock.Setup(o => o.Locate(It.IsAny<TypeInfo>(), It.IsAny<string>())).Returns(result);

			object outparam = result;
			locatorMock.Setup(o => o.TryLocate(It.IsAny<TypeInfo>(), It.IsAny<string>(), out outparam)).Returns(true);
		}

		private void LocatorMockThrows(Mock<IOptionalLocator> locatorMock, Exception exception)
		{
			locatorMock.Setup(o => o.CanLocate(It.IsAny<TypeInfo>(), It.IsAny<string>())).Returns(true);
			locatorMock.Setup(o => o.Locate(It.IsAny<TypeInfo>(), It.IsAny<string>())).Throws(exception);

			object outparam = null;
			locatorMock.Setup(o => o.TryLocate(It.IsAny<TypeInfo>(), It.IsAny<string>(), out outparam)).Throws(exception);
		}

		[Test]
		public void Locate_AltLocatorlardanIlkUygunOlanKullanilarakLocateYapilir()
		{
			LocatorMockReturns(locatorMock2, "locator2");

			Assert.AreEqual("locator2", testingInterface.Locate(type.of<string>(), "dummy"));
		}

		[Test]
		public void Locate_HicbirAltLocatorUygunDegilseHataFirlatilir()
		{
			try
			{
				testingInterface.Locate(type.of<string>(), "dummy");
				Assert.Fail("exception not thrown");
			}
			catch(CannotLocateException ex)
			{
				Assert.IsTrue(ex.Message.Contains("dummy") && ex.Message.Contains(type.of<string>().Name), ex.Message);
			}
		}

		[Test]
		public void Locate_BasarisizLocatingIcinDefaultDegerSetEdilebilir()
		{
			testing.OnFailReturn("default");

			Assert.AreEqual("default", testingInterface.Locate(type.of<string>(), "dummy"));
		}

		[Test]
		public void Locate_BasarisizLocatingIcinFirlatilacakHataDegistirilebilir()
		{
			var expected = new CannotLocateException(type.of<string>(), "expected");
			testing.OnFailThrow(expected);

			try
			{
				testingInterface.Locate(type.of<string>(), "dummy");
				Assert.Fail("exception not thrown");
			}
			catch(CannotLocateException actual)
			{
				Assert.AreSame(expected, actual);
			}
		}

		[Test]
		public void Locate_BasarisizLocatingIcinFirlatilacakHataNesneKullanilarakOlusturulabilir()
		{
			testing.OnFailThrow((t, id) => new CannotLocateException(t, "id is -> " + id));

			try
			{
				testingInterface.Locate(type.of<string>(), "dummy");
				Assert.Fail("exception not thrown");
			}
			catch(CannotLocateException ex)
			{
				Assert.IsTrue(ex.Message.Contains("id is -> "), ex.Message);
			}
		}

		[Test]
		public void Locate_UygunOlanBirLocatorHataFirlatirsaHataPaketlenerekTekrarFirlatilir()
		{
			LocatorMockThrows(locatorMock1, new Exception("inner exception"));

			try
			{
				testingInterface.Locate(type.of<string>(), "dummy");
				Assert.Fail("exception not thrown");
			}
			catch(CannotLocateException ex)
			{
				Assert.AreEqual("inner exception", ex.InnerException.Message);
			}
		}

		[Test]
		public void Locate_UygunOlanBirLocateCannotLocateHatasiFirlatirsaHataPaketlenmedenTekrarFirlatilir()
		{
			var expected = new CannotLocateException(type.of<string>(), "expected");

			LocatorMockThrows(locatorMock1, expected);

			try
			{
				testingInterface.Locate(type.of<string>(), "dummy");
				Assert.Fail("exception not thrown");
			}
			catch(CannotLocateException ex)
			{
				Assert.AreSame(expected, ex);
			}
		}

		[Test]
		public void Merge_BaskaBirMultipleLocatorIcerisindekiLocatorlarGelisSirasinaGoreSonaEklenir()
		{
			LocatorMockReturns(locatorMock3, "locator3");

			testing.Merge(testingOther);

			Assert.AreEqual("locator3", testingInterface.Locate(type.of<string>(), "dummy"));
		}
	}
}

