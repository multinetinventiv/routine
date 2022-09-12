using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using System;

namespace Routine.Test;

[TestFixture]
public class EnumerableExtensionsTest
{
    [Test]
    public void Test_IEnumerable_ToItemString()
    {
        Assert.AreEqual("[a,b]", new[] { "a", "b" }.ToItemString());
    }

    [Test]
    public void Test_IEnumerable_ItemEquals()
    {
        Assert.IsTrue(((IEnumerable)null).ItemEquals(null));
        Assert.IsFalse(((IEnumerable)null).ItemEquals(Array.Empty<string>()));
        Assert.IsFalse(Array.Empty<string>().ItemEquals(null));

        Assert.IsTrue(Array.Empty<string>().ItemEquals(new List<string>()));
        Assert.IsTrue(Array.Empty<object>().ItemEquals(new List<string>()));

        Assert.IsTrue(new[] { "a" }.ItemEquals(new List<string> { "a" }));

        Assert.IsFalse(new[] { "a" }.ItemEquals(new List<string> { "a", "b" }));
        Assert.IsFalse(new[] { "a", "b" }.ItemEquals(new List<string> { "a" }));
    }

    [Test]
    public void Test_IEnumerable_GetItemHashCode()
    {
        Assert.AreEqual(Array.Empty<string>().GetItemHashCode(), new List<string>().GetItemHashCode());

        Assert.AreEqual(new[] { "a" }.GetItemHashCode(), new List<string> { "a" }.GetItemHashCode());
        Assert.AreNotEqual(new[] { "a" }.GetItemHashCode(), new List<string> { "a", "b" }.GetItemHashCode());
    }
}
