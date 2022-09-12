using NUnit.Framework;
using Routine.Service;
using System;

namespace Routine.Test.Service;

[TestFixture]
public class MimeTypeMapTest
{
    [Test]
    public void Returns_mime_type_for_known_extensions()
    {
        Assert.AreEqual("image/png", MimeTypeMap.GetMimeType(".png"));
        Assert.AreEqual("text/plain", MimeTypeMap.GetMimeType(".txt"));
        Assert.AreEqual("text/xml", MimeTypeMap.GetMimeType(".xml"));
    }

    [Test]
    public void Accepts_extension_parameter_without_a_leading_dot()
    {
        Assert.AreEqual("image/png", MimeTypeMap.GetMimeType("png"));
    }

    [Test]
    public void Throws_argument_null_when_no_extension_was_given()
    {
        Assert.Throws<ArgumentNullException>(() => MimeTypeMap.GetMimeType(null));
    }

    [Test]
    public void Returns_default_mime_type_when_an_unknown_extension_was_given()
    {
        Assert.AreEqual("application/octet-stream", MimeTypeMap.GetMimeType(".unknown-extension"));
    }
}
