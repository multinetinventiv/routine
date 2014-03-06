using System;
using System.Collections.Generic;
using NUnit.Framework;
using Routine.Core;
using Routine.Core.Selector;

namespace Routine.Test.Core.Selector
{
	[TestFixture]
	public class BaseOptionalSelectorTest : CoreTestBase
	{
		public override string[] DomainTypeRootNamespaces{get{return new[]{"Routine.Test.Core.Selector"};}}

		private class TestSelector<TResult> : BaseOptionalSelector<TestSelector<TResult>, TypeInfo, TResult>
		{
			protected override List<TResult> Select(TypeInfo type){ return null; }
		}

		private TestSelector<IMember> testing;
		private IOptionalSelector<TypeInfo, IMember> testingInterface;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			testingInterface = testing = new TestSelector<IMember>();
		}

		[Test]
		public void CanSelect_DurumBelirtilmemisseTrueDoner()
		{
			Assert.IsTrue(testingInterface.CanSelect(type.of<string>()));
		}

		[Test]
		public void CanSelect_TypeBelirtilmisDurumaUygunsaTrueDoner()
		{
			testing.When(t => t == type.of<string>());

			Assert.IsTrue(testingInterface.CanSelect(type.of<string>()));
		}

		[Test]
		public void CanSelect_TypeBelirtilmisDurumaUygunDegilseFalseDoner()
		{
			testing.When(t => t != type.of<string>());

			Assert.IsFalse(testingInterface.CanSelect(type.of<string>()));
		}

		[Test]
		public void Select_ThrowsCannotSelectExceptionWhenItCantSelectFromGivenObject()
		{
			testing.When(t => t != type.of<string>());

			try
			{
				testingInterface.Select(type.of<string>());
				Assert.Fail("exception not thrown");
			}
			catch(CannotSelectException ex)
			{
				Assert.IsTrue(ex.Message.Contains(type.of<string>().Name), ex.Message);
			}
		}
	}
}

