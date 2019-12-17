using NUnit.Framework;
using Routine.Core.Configuration.Convention;

namespace Routine.Test.Core.Configuration.Conventions
{
	[TestFixture]
	public class DelegateBasedConventionTest : CoreTestBase
	{
		[Test]
		public void Applies_given_delegate_to_given_object()
		{
			IConvention<object, string> testing =
				new DelegateBasedConvention<object, string>()
					.Return(o => o.ToString());

			Assert.AreEqual("1", testing.Apply(1));
		}

		[Test]
		public void When_no_delegate_was_given_returns_default_value()
		{
			IConvention<int, string> testing1 = new DelegateBasedConvention<int, string>();
			Assert.AreEqual(null, testing1.Apply(1));

			IConvention<string, int> testing2 = new DelegateBasedConvention<string, int>();
			Assert.AreEqual(0, testing2.Apply("string"));
		}

		[Test]
		public void Facade__Can_return_constant_result_no_matter_what()
		{
			IConvention<string, string> testing = BuildRoutine.Convention<string, string>().Constant("constant_result");

			Assert.AreEqual("constant_result", testing.Apply("test1"));
			Assert.AreEqual("constant_result", testing.Apply("test2"));
		}
	}
}