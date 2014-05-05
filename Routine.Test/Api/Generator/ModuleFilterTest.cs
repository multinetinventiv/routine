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
		public void Includes_when_no_filter_was_given()
		{
			Assert.IsTrue(testing.IsModuleIncluded("dummy"));
		}

		[Test]
		public void Includes_when_given_string_conforms_to_given_include_filter()
		{
			testing.Include("i.*");
			Assert.IsTrue(testing.IsModuleIncluded("included"));
			Assert.IsFalse(testing.IsModuleIncluded("excluded"));
		}

		[Test]
		public void Excludes_when_given_string_conforms_to_given_exclude_filter()
		{
			testing.Exclude("e.*");
			Assert.IsTrue(testing.IsModuleIncluded("included"));
			Assert.IsFalse(testing.IsModuleIncluded("excluded"));
		}

		[Test]
		public void Includes_when_any_of_include_filters_is_ok()
		{
			testing.Include("i.*");
			testing.Include("c.*");
			Assert.IsTrue(testing.IsModuleIncluded("included"));
			Assert.IsTrue(testing.IsModuleIncluded("checkswell"));
			Assert.IsFalse(testing.IsModuleIncluded("excluded"));
		}

		[Test]
		public void Excludes_when_any_of_exclude_filters_is_ok()
		{
			testing.Exclude("e.*");
			testing.Exclude("c.*");
			Assert.IsFalse(testing.IsModuleIncluded("excluded"));
			Assert.IsFalse(testing.IsModuleIncluded("checksbad"));
			Assert.IsTrue(testing.IsModuleIncluded("included"));
		}

		[Test]
		public void Includes_when_any_of_include_filters_is_ok_and_none_of_exclude_filters_is_ok()
		{
			testing.Include(".*well");
			testing.Include("i.*");
			testing.Exclude(".*e");
			testing.Exclude("does.*");
			Assert.IsTrue(testing.IsModuleIncluded("included"));
			Assert.IsTrue(testing.IsModuleIncluded("checkswell"));
			Assert.IsFalse(testing.IsModuleIncluded("exclude"));
			Assert.IsFalse(testing.IsModuleIncluded("notincludednotexcluded"));
			Assert.IsFalse(testing.IsModuleIncluded("doesthisdowell"));
		}
	}
}

