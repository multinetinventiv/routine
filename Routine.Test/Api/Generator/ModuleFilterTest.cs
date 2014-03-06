using System;
using NUnit.Framework;
using Routine.Api.Generator;

namespace Routine.Test.Api.Generator
{
	[TestFixture]
	public class ModuleFilterTest
	{
		private ModuleFilter testing;

		[SetUp]
		public void SetUp()
		{
			testing = new ModuleFilter();
		}

		[Test]
		public void IncludesWhenNoFilterWasGiven()
		{
			Assert.IsTrue(testing.Check("dummy"));
		}

		[Test]
		public void IncludesWhenGivenStringConformsToGivenIncludeFilter()
		{
			testing.Include("i.*");
			Assert.IsTrue(testing.Check("included"));
			Assert.IsFalse(testing.Check("excluded"));
		}

		[Test]
		public void ExcludesWhenGivenStringConformsToGivenExcludeFilter()
		{
			testing.Exclude("e.*");
			Assert.IsTrue(testing.Check("included"));
			Assert.IsFalse(testing.Check("excluded"));
		}

		[Test]
		public void IncludesWhenAnyOfIncludeFiltersIsOk()
		{
			testing.Include("i.*");
			testing.Include("c.*");
			Assert.IsTrue(testing.Check("included"));
			Assert.IsTrue(testing.Check("checkswell"));
			Assert.IsFalse(testing.Check("excluded"));
		}

		[Test]
		public void ExcludesWhenAnyOfExcludeFiltersIsOk()
		{
			testing.Exclude("e.*");
			testing.Exclude("c.*");
			Assert.IsFalse(testing.Check("excluded"));
			Assert.IsFalse(testing.Check("checksbad"));
			Assert.IsTrue(testing.Check("included"));
		}

		[Test]
		public void IncludesWhenAnyOfIncludeFiltersIsOkAndNoneOfExcludeFiltersIsOk()
		{
			testing.Include(".*well");
			testing.Include("i.*");
			testing.Exclude(".*e");
			testing.Exclude("does.*");
			Assert.IsTrue(testing.Check("included"));
			Assert.IsTrue(testing.Check("checkswell"));
			Assert.IsFalse(testing.Check("exclude"));
			Assert.IsFalse(testing.Check("notincludednotexcluded"));
			Assert.IsFalse(testing.Check("doesthisdowell"));
		}
	}
}

