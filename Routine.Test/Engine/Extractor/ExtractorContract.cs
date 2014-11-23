using System;
using System.Collections.Generic;
using NUnit.Framework;
using Routine.Core.Configuration.Convention;
using Routine.Engine;
using Routine.Engine.Extractor;
using Routine.Test.Core;

namespace Routine.Test.Engine.Extractor
{
	public class ResultClass
	{
		public void VoidMethod() { }
		public string ParameterMethod(string parameter) { return "ParameterMethod"; }
		public string StringMethod() { return "StringMethod"; }
		public string StringMethod2() { return "StringMethod2"; }
		public int IntMethod() { return 1; }
		public List<string> ListMethod() { return new List<string> { "ListMethod1", "ListMethod2" }; }
		public string NullMethod() { return null; }
	}

	public abstract class ExtractorContract<T> : CoreTestBase
	{
		protected IConvention<IType, T> CreateConventionByPublicOperation(Func<IOperation, bool> operationFilter) { return CreateConventionByPublicOperation(operationFilter, e => e); }
		protected abstract IConvention<IType, T> CreateConventionByPublicOperation(Func<IOperation, bool> operationFilter, Func<MemberValueExtractor, MemberValueExtractor> configurationDelegate);

		protected abstract IConvention<IType, T> CreateConventionByDelegate(Func<object, string> extractorDelegate);

		protected abstract string Extract(T extractor, object obj);

		[Test]
		public void MemberValueExtactor__When_no_member_was_found_using_given_filter__AppliesTo_returns_false()
		{
			var testing = CreateConventionByPublicOperation(o => o.Name == "NonExistingMethod");

			Assert.IsFalse(testing.AppliesTo(type.of<ResultClass>()));
		}
		[Test]
		public void MemberValueExtactor__When_given_filter_finds_inappropriate_method__AppliesTo_returns_false()
		{
			var testing = CreateConventionByPublicOperation(o => o.Name == "ParameterMethod");

			Assert.IsFalse(testing.AppliesTo(type.of<ResultClass>()));

			testing = CreateConventionByPublicOperation(o => o.Name == "VoidMethod");

			Assert.IsFalse(testing.AppliesTo(type.of<ResultClass>()));
		}

		[Test]
		public void MemberValueExtactor__When_given_filter_finds_more_than_one_method__first_one_is_used()
		{
			var testing = CreateConventionByPublicOperation(o => o.Name.Contains("StringMethod"));

			var extractor = testing.Apply(type.of<ResultClass>());

			Assert.AreEqual("StringMethod", Extract(extractor, new ResultClass()));
		}

		[Test]
		public void MemberValueExtactor__When_given_filter_finds_both_appropriate_and_inappropriate_methods__first_appropriate_method_is_used()
		{
			var testing = CreateConventionByPublicOperation(o => o.Name.Contains("Method"));

			var extractor = testing.Apply(type.of<ResultClass>());

			Assert.AreEqual("StringMethod", Extract(extractor, new ResultClass()));
		}

		[Test]
		public void MemberValueExtactor__Extractor_can_convert_member_result()
		{
			var testing = CreateConventionByPublicOperation(o => o.Name == "IntMethod", e => e.Return(v => "int:" + (int)v));

			var extractor = testing.Apply(type.of<ResultClass>());

			Assert.AreEqual("int:1", Extract(extractor, new ResultClass()));
		}

		[Test]
		public void MemberValueExtactor__Conversion_delegate_optionally_can_pass_target_object()
		{
			var testing = CreateConventionByPublicOperation(o => o.Name == "IntMethod", e => e.Return((v, o) => ((ResultClass)o).StringMethod() + ":int:" + (int)v));

			var extractor = testing.Apply(type.of<ResultClass>());

			Assert.AreEqual("StringMethod:int:1", Extract(extractor, new ResultClass()));
		}

		[Test]
		public void MemberValueExtractor__By_default_returns_null_when_member_value_is_null()
		{
			var testing = CreateConventionByPublicOperation(o => o.Name == "NullMethod");

			var extractor = testing.Apply(type.of<ResultClass>());

			Assert.AreEqual(null, Extract(extractor, new ResultClass()));
		}

		[Test]
		public void DelegateBasedExtractor__Extracts_using_given_delegate()
		{
			var testing = CreateConventionByDelegate(o => ((ResultClass)o).StringMethod());

			var extractor = testing.Apply(type.of<ResultClass>());

			Assert.AreEqual("StringMethod", Extract(extractor, new ResultClass()));
		}
	}
}