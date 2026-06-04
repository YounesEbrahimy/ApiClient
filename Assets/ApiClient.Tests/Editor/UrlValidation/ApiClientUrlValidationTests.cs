using NUnit.Framework;
using ApiClientLib;

[TestFixture]
public class ApiClientUrlValidationTests
{
    // --- Valid URLs: should return the URL as-is ---

    [TestCase("http://example.com")]
    [TestCase("https://example.com")]
    [TestCase("https://example.com/path/to/resource")]
    [TestCase("https://example.com/path?query=1&other=2")]
    [TestCase("https://example.com/path#fragment")]
    [TestCase("https://example.com:8080")]
    [TestCase("https://example.com:8080/path?query=1")]
    [TestCase("https://subdomain.example.com")]
    [TestCase("https://sub.sub.example.co.uk/path")]
    [TestCase("https://192.168.1.1")]
    [TestCase("https://192.168.1.1:9000/api")]
    [TestCase("https://user:password@example.com")] // auth in URL
    [TestCase("https://example.com/path%20with%20spaces")] // percent-encoded
    [TestCase("https://EXAMPLE.COM")] // uppercase host
    public void ValidatedUrl_ValidUrl_ReturnsUrl(string url)
    {
        var result = ApiClient.ValidatedUrl(string.Empty, url);
        Assert.AreEqual(result, url);
    }

    // --- Invalid URLs: wrong scheme, relative, malformed, null, empty and whitespace ---

    [TestCase("ftp://example.com", TestName = "FtpScheme")]
    [TestCase("file:///etc/hosts", TestName = "FileScheme")]
    [TestCase("ws://example.com", TestName = "WebSocketScheme")]
    [TestCase("mailto:user@example.com", TestName = "MailtoScheme")]
    [TestCase("//example.com/path", TestName = "SchemelessUrl")]
    [TestCase("/relative/path", TestName = "RelativePath")]
    [TestCase("relative/path", TestName = "RelativePathNoSlash")]
    [TestCase("example.com", TestName = "MissingScheme")]
    [TestCase("http//example.com", TestName = "MissingColonInScheme")]
    [TestCase("https:/example.com", TestName = "SingleSlashAfterScheme")]
    [TestCase("https://", TestName = "EmptyHost")]
    [TestCase("not a url at all", TestName = "RandomString")]
    [TestCase("12345", TestName = "NumbersOnly")]
    [TestCase("https:// example.com", TestName = "SpaceInHost")]
    [TestCase(" ", TestName = "SingleSpace")]
    [TestCase("   ", TestName = "MultipleSpaces")]
    [TestCase("\t", TestName = "Tab")]
    [TestCase("\n", TestName = "Newline")]
    public void ValidatedUrl_InvalidUrl_ThrowsArgumentException(string url)
    {
        var ex = Assert.Throws<InvalidUrlException>(() => ApiClient.ValidatedUrl(string.Empty, url));
        Assert.That(ex.Message, Does.Contain("Invalid URL"));
    }
}