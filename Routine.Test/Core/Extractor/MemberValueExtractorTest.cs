using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Routine.Core;
using Routine.Core.DomainApi;
using Routine.Core.Extractor;
using Routine.Test.Core.Extractor.Domain;
using Routine.Core.Builder;

namespace Routine.Test.Core.Extractor.Domain
{
	public class ResultClass
	{
		public void VoidMethod() { }
		public string ParameterMethod(string parameter) { return "ParameterMethod"; }
		public string StringMethod() { return "StringMethod"; }
		public string StringMethod2() { return "StringMethod2"; }
		public int IntMethod() { return 1; }
		public List<string> ListMethod() { return new List<string> { "ListMethod1", "ListMethod2" }; }
	}
}

namespace Routine.Test.Core.Extractor
{
	[TestFixture]
	public class MemberValueExtractorTest : CoreTestBase
	{
		public override string[] DomainTypeRootNamespaces{get{return new[]{"Routine.Test.Core.Extractor.Domain"};}}

		private IOptionalExtractor<ResultClass, TResult> Extractor<TResult>(Func<ExtractorBuilder<ResultClass, TResult>, IOptionalExtractor<ResultClass, TResult>> builder)
		{
			return builder(BuildRoutine.Extractor<ResultClass, TResult>());
		}

		[Test]
		public void When_no_member_was_found_using_given_filter__CanExtract_returns_false()
		{
			var testing = Extractor<string>(e => e.ByPublicMethod(m => m.Name == "NonExistingMethod"));

			Assert.IsFalse(testing.CanExtract(new ResultClass()));
		}

		[Test]
		public void When_given_object_is_null__CanExtract_returns_false()
		{
			var testing = Extractor<string>(e => e.ByPublicMethod(m => m.Name == "StringMethod"));

			Assert.IsFalse(testing.CanExtract(null));
		}

		[Test]
		public void When_given_filter_finds_inappropriate_method__CanExtract_returns_false()
		{
			var testing = Extractor<string>(e => e.ByPublicMethod(m => m.Name == "ParameterMethod"));

			Assert.IsFalse(testing.CanExtract(new ResultClass()));

			testing = Extractor<string>(e => e.ByPublicMethod(m => m.Name == "VoidMethod"));

			Assert.IsFalse(testing.CanExtract(new ResultClass()));
		}

		[Test]
		public void When_given_filter_finds_more_than_one_method__first_one_is_used()
		{
			var testing = Extractor<string>(e => e.ByPublicMethod(m => m.Name.Contains("StringMethod")));

			Assert.AreEqual("StringMethod", testing.Extract(new ResultClass()));
		}

		[Test]
		public void When_given_filter_finds_both_appropriate_and_inappropriate_methods__first_appropriate_method_is_used()
		{
			var testing = Extractor<string>(e => e.ByPublicMethod(m => m.Name.Contains("Method")));

			Assert.AreEqual("StringMethod", testing.Extract(new ResultClass()));
		}

		[Test]
		public void Extractor_can_convert_member_result()
		{
			var testing = Extractor<string>(e => e.ByPublicMethod(m => m.Name == "IntMethod")
												  .Return(v => "int:" + (int)v));

			Assert.AreEqual("int:1", testing.Extract(new ResultClass()));
		}
		
		[Test]
		public void Conversion_delegate_optionally_can_pass_target_object()
		{
			var testing = Extractor<string>(e => e.ByPublicMethod(m => m.Name == "IntMethod")
												  .Return((v, o) => o.StringMethod() + ":int:" + (int)v));

			Assert.AreEqual("StringMethod:int:1", testing.Extract(new ResultClass()));
		}

		[Test]
		public void Facade_ReturnCastedList()
		{
			var testing = Extractor<List<string>>(e => e.ByPublicMethod(m => m.Name == "ListMethod")
												  .ReturnCastedList());

			List<string> actual = testing.Extract(new ResultClass());

			Assert.AreEqual(2, actual.Count);
			Assert.AreEqual("ListMethod1", actual[0]);
			Assert.AreEqual("ListMethod2", actual[1]);
		}

		[Test]
		public void Facade_ReturnAsString()
		{
			var testing = Extractor<string>(e => e.ByPublicMethod(m => m.Name == "IntMethod")
												  .ReturnAsString());

			Assert.AreEqual("1", testing.Extract(new ResultClass()));
		}
	}
}

