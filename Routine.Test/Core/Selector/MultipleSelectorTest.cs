using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Routine.Core;
using Routine.Core.CodingStyle;
using Routine.Core.Selector;

namespace Routine.Test.Core.Selector
{
	[TestFixture]
	public class MultipleSelectorTest : CoreTestBase
	{
		public override string[] DomainTypeRootNamespaces{get{return new[]{"Routine.Test.Core.Selector"};}}

		private Mock<IOptionalSelector<TypeInfo, string>> selectorMock1;
		private Mock<IOptionalSelector<TypeInfo, string>> selectorMock2;
		private Mock<IOptionalSelector<TypeInfo, string>> selectorMock3;

		private MultipleSelector<GenericCodingStyle, TypeInfo, string> testing;
		private ISelector<TypeInfo, string> testingInterface;
		private MultipleSelector<GenericCodingStyle, TypeInfo, string> testingOther;
		//private ISelector<TypeInfo, string> testingOtherInterface;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			selectorMock1 = new Mock<IOptionalSelector<TypeInfo, string>>();
			selectorMock2 = new Mock<IOptionalSelector<TypeInfo, string>>();
			selectorMock3 = new Mock<IOptionalSelector<TypeInfo, string>>();

			testingInterface = testing = new MultipleSelector<GenericCodingStyle, TypeInfo, string>(new GenericCodingStyle());
			/* testingOtherInterface = */ testingOther = new MultipleSelector<GenericCodingStyle, TypeInfo, string>(new GenericCodingStyle());

			testing.Add(selectorMock1.Object)
				   .Done(selectorMock2.Object);

			testingOther.Done(selectorMock3.Object);

			SetUpSelector(selectorMock1);
			SetUpSelector(selectorMock2);
			SetUpSelector(selectorMock3);
		}

		private void SetUpSelector(Mock<IOptionalSelector<TypeInfo, string>> mock, params string[] returnList)
		{
			SetUpSelector(mock, true, returnList);
		}

		private void SetUpSelector(Mock<IOptionalSelector<TypeInfo, string>> mock, bool canSelect, params string[] returnList)
		{
			mock.Setup(o => o.CanSelect(It.IsAny<TypeInfo>())).Returns(canSelect);
			mock.Setup(o => o.Select(It.IsAny<TypeInfo>())).Returns(returnList.ToList());

			var result = returnList.ToList();
			mock.Setup(o => o.TrySelect(It.IsAny<TypeInfo>(), out result)).Returns(canSelect);
		}

		private void SetUpSelector(Mock<IOptionalSelector<TypeInfo, string>> mock, Exception exception)
		{
			mock.Setup(o => o.CanSelect(It.IsAny<TypeInfo>())).Returns(true);
			mock.Setup(o => o.Select(It.IsAny<TypeInfo>())).Throws(exception);

			List<string> result;
			mock.Setup(o => o.TrySelect(It.IsAny<TypeInfo>(), out result)).Throws(exception);
		}

		[Test]
		public void Select_AltSelectorunDonduguListeyiDonulur()
		{
			SetUpSelector(selectorMock1, "result1", "result2");

			var actual = testingInterface.Select(type.of<string>());

			Assert.AreEqual(2, actual.Count);
			Assert.AreEqual("result1", actual[0]);
			Assert.AreEqual("result2", actual[1]);
		}

		[Test]
		public void Select_BirdenFazlaSelectorunDonduguListeBirlestirilir()
		{
			SetUpSelector(selectorMock1, "result1", "result2");
			SetUpSelector(selectorMock2, "result3", "result4");

			var actual = testingInterface.Select(type.of<string>());

			Assert.AreEqual(4, actual.Count);
			Assert.AreEqual("result1", actual[0]);
			Assert.AreEqual("result2", actual[1]);
			Assert.AreEqual("result3", actual[2]);
			Assert.AreEqual("result4", actual[3]);
		}
		
		[Test]
		public void Select_AyniSonuclarTekilOlarakSunulur()
		{
			SetUpSelector(selectorMock1, "result1", "result2");
			SetUpSelector(selectorMock2, "result2", "result3");

			var actual = testingInterface.Select(type.of<string>());

			Assert.AreEqual(3, actual.Count);
			Assert.AreEqual("result1", actual[0]);
			Assert.AreEqual("result2", actual[1]);
			Assert.AreEqual("result3", actual[2]);
		}

		[Test]
		public void Select_UygunOlmayanSelectorlarKullanilmaz()
		{
			SetUpSelector(selectorMock1, false, "result1", "result2");
			SetUpSelector(selectorMock2, "result3", "result4");

			var actual = testingInterface.Select(type.of<string>());

			Assert.AreEqual(2, actual.Count);
			Assert.AreEqual("result3", actual[0]);
			Assert.AreEqual("result4", actual[1]);
		}

		[Test]
		public void Select_KuralBazliItemExcludeEdilebilir()
		{
			SetUpSelector(selectorMock1, "result1", "result2");

			testing.Exclude.Add(s => s.Contains("1"));
			
			var actual = testingInterface.Select(type.of<string>());

			Assert.AreEqual(1, actual.Count);
			Assert.AreEqual("result2", actual[0]);
		}

		[Test]
		public void Select_BirItemExcludeKurallarindanHerhangiBirineUyuyorsaItemExcludeEdilir()
		{
			SetUpSelector(selectorMock1, "result1", "result2", "result3");
			
			testing.Exclude.Add(s => s.Contains("2"))
						   .Done(s => s.EndsWith("3"));

			var actual = testingInterface.Select(type.of<string>());

			Assert.AreEqual(1, actual.Count);
			Assert.AreEqual("result1", actual[0]);
		}

		[Test]
		public void Select_NoMoreItemsHatasiGeldiktenSonrakiSelectorlariKullanmaz()
		{
			SetUpSelector(selectorMock1, "result1");
			SetUpSelector(selectorMock2, new NoMoreItemsShouldBeSelectedException());
			SetUpSelector(selectorMock3, "result2");

			testing.Merge(testingOther);

			var actual = testingInterface.Select(type.of<string>());

			Assert.AreEqual(1, actual.Count);
			Assert.AreEqual("result1", actual[0]);
		}


		[Test]
		public void Merge_BaskaBirMultipleSelectorIcerisindekiSelectorlarGelisSirasinaGoreSonaEklenir()
		{
			SetUpSelector(selectorMock1, "result1");
			SetUpSelector(selectorMock3, "result2");

			testing.Merge(testingOther);

			var actual = testingInterface.Select(type.of<string>());

			Assert.AreEqual(2, actual.Count);
			Assert.AreEqual("result1", actual[0]);
			Assert.AreEqual("result2", actual[1]);
		}

		[Test]
		public void Merge_DigerSelectorunIcindekiExcluderlarMevcutaDahilEdilir()
		{
			SetUpSelector(selectorMock1, "result1");
			SetUpSelector(selectorMock1, "result2");
			SetUpSelector(selectorMock3, "result3");
			SetUpSelector(selectorMock3, "result4");

			testing.Exclude.Done(s => s.EndsWith("3"));
			testingOther.Exclude.Done(s => s.EndsWith("1"));

			testing.Merge(testingOther);

			var actual = testingInterface.Select(type.of<string>());

			Assert.AreEqual(2, actual.Count);
			Assert.AreEqual("result2", actual[0]);
			Assert.AreEqual("result4", actual[1]);
		}
	}
}

