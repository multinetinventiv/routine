using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Routine.Core;
using Routine.Core.Member;
using Routine.Core.Extractor;
using Routine.Test.Core.Extractor.Domain;

namespace Routine.Test.Core.Extractor.Domain
{
	public class ResultClass
	{
		public string StringMethod() {return "StringMethod";}
		public string StringMethod2() {return "StringMethod2";}
		public int IntMethod() {return 1;}
		public string ParameterMethod(string parameter) {return "ParameterMethod";}
		public void VoidMethod() {}
		public List<string> ListMethod(){return new List<string>{"ListMethod1", "ListMethod2"};}
	}
}

namespace Routine.Test.Core.Extractor
{
	[TestFixture]
	public class ReferenceValueExtractorTest : CoreTestBase
	{
		public override string[] DomainTypeRootNamespaces{get{return new[]{"Routine.Test.Core.Extractor.Domain"};}}

		private Mock<ISelector<TypeInfo, IMember>> selectorMock;

		private IOptionalExtractor<ResultClass, string> testingInterface;
		private ReferenceValueExtractor<ResultClass, string> testing;
		private IOptionalExtractor<Tuple<ResultClass, string>, string> testingSubObjectInterface;
		private ReferenceValueExtractor<Tuple<ResultClass, string>, string> testingSubObject;
		private IOptionalExtractor<ResultClass, List<object>> testingListInterface;
		private ReferenceValueExtractor<ResultClass, List<object>> testingList;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			selectorMock = new Mock<ISelector<TypeInfo, IMember>>();

			testingInterface = testing = new ReferenceValueExtractor<ResultClass, string>(o => selectorMock.Object);
			testingSubObjectInterface = testingSubObject = new ReferenceValueExtractor<Tuple<ResultClass, string>, string>(o => selectorMock.Object);
			testingListInterface = testingList = new ReferenceValueExtractor<ResultClass, List<object>>(o => selectorMock.Object);

			SelectorReturnsEmpty();
		}

		private void SelectorReturnsEmpty()
		{			
			selectorMock.Setup(o => o.Select(It.IsAny<TypeInfo>()))
				.Returns(new List<IMember>());
		}

		private void SelectorReturns(string firstMethodName, params string[] otherMethodNames)
		{
			var methodNames = new List<string>();
			methodNames.Add(firstMethodName);
			methodNames.AddRange(otherMethodNames);

			selectorMock.Setup(o => o.Select(It.IsAny<TypeInfo>()))
				.Returns(methodNames.Select(n => new MethodMember(type.of<ResultClass>().GetMethod(n)) as IMember).ToList());
		}

		[Test]
		public void CanExtract_BelirtilenSelectorUzerindenMemberBulunamadigindaFalseDoner()
		{
			Assert.IsFalse(testingInterface.CanExtract(new ResultClass()));
		}

		[Test]
		public void CanExtract_VerilenNesneYaDaAltNesneNullOldugundaFalseDoner()
		{
			SelectorReturns("StringMethod");

			Assert.IsFalse(testingInterface.CanExtract(null));

			testingSubObject.Using(o => o.Item1);

			Assert.IsFalse(testingSubObjectInterface.CanExtract(null, "string"));
		}

		[Test]
		public void CanExtract_SelectorYalnizcaParametreliMetodDonerseFalseDoner()
		{
			SelectorReturns("ParameterMethod");

			Assert.IsFalse(testingInterface.CanExtract(new ResultClass()));
		}

		[Test]
		public void CanExtract_SelectorYalnizcaVoidMetodDonerseFalseDoner()
		{
			SelectorReturns("VoidMethod");

			Assert.IsFalse(testingInterface.CanExtract(new ResultClass()));
		}

		[Test]
		public void Extract_VerilenNesneUzerindenSelectorUzerindekiIlkMemberinDegeriniDoner()
		{
			SelectorReturns("StringMethod2", "StringMethod");

			Assert.AreEqual("StringMethod2", testingInterface.Extract(new ResultClass()));
		}

		[Test]
		public void Extract_SelectorBirdenFazlaDonuyorsaUygunOlanIlkMetodKullanilir()
		{
			SelectorReturns("VoidMethod", "ParameterMethod", "StringMethod", "StringMethod2");

			Assert.AreEqual("StringMethod", testingInterface.Extract(new ResultClass()));
		}

		[Test]
		public void Extract_VerilenNesneninBirAltNesnesiKullanilabilir()
		{
			SelectorReturns("StringMethod");

			testingSubObject.Using(o => o.Item1);

			Assert.AreEqual("StringMethod", testingSubObjectInterface.Extract(new ResultClass(), "string"));
		}

		[Test]
		public void Extract_DonulecekDegerConverterAraciligiIleDogrudanDegistirilebilir()
		{
			SelectorReturns("IntMethod");

			testing.Return(v => "int:" + (int)v);

			Assert.AreEqual("int:1", testingInterface.Extract(new ResultClass()));
		}
		
		[Test]
		public void Extract_DonulecekDegerConverterAraciligiIleVerilenNesneKullanilarakDegistirilebilir()
		{
			SelectorReturns("IntMethod");

			testing.Return((v, o) => o.StringMethod() + ":int:" + (int)v);

			Assert.AreEqual("StringMethod:int:1", testingInterface.Extract(new ResultClass()));
		}

		[Test]
		public void Facade_ReturnCastedList()
		{
			SelectorReturns("ListMethod");

			testingList.ReturnCastedList();

			List<object> actual = testingListInterface.Extract(new ResultClass());

			Assert.AreEqual(2, actual.Count);
			Assert.AreEqual("ListMethod1", actual[0]);
			Assert.AreEqual("ListMethod2", actual[1]);
		}

		[Test]
		public void Facade_ReturnAsString()
		{
			SelectorReturns("IntMethod");

			testing.ReturnAsString();

			Assert.AreEqual("1", testingInterface.Extract(new ResultClass()));
		}
	}
}

