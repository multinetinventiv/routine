using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace Routine.Test
{
	[TestFixture]
	public class EnumerableExtensionsTest
	{
		[Test]
		public void Test_IEnumerable_ToItemString()
		{
			Assert.AreEqual("[a,b]", new []{ "a", "b" }.ToItemString());
		}

		[Test]
		public void Test_IEnumerable_ItemEquals()
		{
			Assert.IsTrue(((IEnumerable)null).ItemEquals((IEnumerable)null));
			Assert.IsFalse(((IEnumerable)null).ItemEquals(new string[0]));
			Assert.IsFalse(new string[0].ItemEquals(((IEnumerable)null)));

			Assert.IsTrue(new string[0].ItemEquals(new List<string>()));
			Assert.IsTrue(new object[0].ItemEquals(new List<string>()));

			Assert.IsTrue(new []{"a"}.ItemEquals(new List<string>{"a"}));

			Assert.IsFalse(new []{"a"}.ItemEquals(new List<string>{"a", "b"}));
			Assert.IsFalse(new []{"a", "b"}.ItemEquals(new List<string>{"a"}));
		}

		[Test]
		public void Test_IEnumerable_GetItemHashCode()
		{
			Assert.AreEqual(new string[0].GetItemHashCode(), new List<string>().GetItemHashCode());

			Assert.AreEqual(new string[]{"a"}.GetItemHashCode(), new List<string>{"a"}.GetItemHashCode());
			Assert.AreNotEqual(new string[]{"a"}.GetItemHashCode(), new List<string>{"a", "b"}.GetItemHashCode());
		}
	}
}

