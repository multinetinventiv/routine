using Routine.Service;

namespace Routine.Test.Service;

[TestFixture]
public class MimeTypeMapTest
{
    [Test]
    public void Returns_mime_type_for_known_extensions()
    {
        Assert.That(MimeTypeMap.GetMimeType(".png"), Is.EqualTo("image/png"));
        Assert.That(MimeTypeMap.GetMimeType(".txt"), Is.EqualTo("text/plain"));
        Assert.That(MimeTypeMap.GetMimeType(".xml"), Is.EqualTo("text/xml"));
    }

    [Test]
    public void Accepts_extension_parameter_without_a_leading_dot()
    {
        Assert.That(MimeTypeMap.GetMimeType("png"), Is.EqualTo("image/png"));
    }

    [Test]
    public void Throws_argument_null_when_no_extension_was_given()
    {
        Assert.That(() => MimeTypeMap.GetMimeType(null), Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void Returns_default_mime_type_when_an_unknown_extension_was_given()
    {
        Assert.That(MimeTypeMap.GetMimeType(".unknown-extension"), Is.EqualTo("application/octet-stream"));
    }
}
