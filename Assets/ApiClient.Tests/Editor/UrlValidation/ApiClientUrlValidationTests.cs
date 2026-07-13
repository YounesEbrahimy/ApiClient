using ApiClientLib.Helpers;
using NUnit.Framework;
using ApiClientLib;

[TestFixture]
public class ApiClientUrlValidationTests
{
    // --- Valid URLs: should return the URL as-is in lower case ---
    [TestCase("http://example.com/")]
    [TestCase("https://example.com/")]
    [TestCase("https://example.com/path/to/resource/")]
    [TestCase("https://example.com/path?query=1&other=2/")]
    [TestCase("https://example.com/path#fragment/")]
    [TestCase("https://example.com:8080/")]
    [TestCase("https://example.com:8080/path?query=1/")]
    [TestCase("https://subdomain.example.com/")]
    [TestCase("https://sub.sub.example.co.uk/path/")]
    [TestCase("https://192.168.1.1/")]
    [TestCase("https://192.168.1.1:9000/api/")]
    [TestCase("https://user:password@example.com/")] // auth in URL
    [TestCase("https://EXAMPLE.COM/")] // uppercase host
    public void ValidatedUrl_ValidUrl_ReturnsUrl(string url)
    {
        var result = UrlValidation.ValidatedUrl(url, string.Empty);
        Assert.AreEqual(result, url.ToLower());
    }

    // --- Invalid URLs: wrong scheme, relative, malformed, null, empty and whitespace ---
    [TestCase("ftp://example.com/", TestName = "FtpScheme")]
    [TestCase("file:///etc/hosts/", TestName = "FileScheme")]
    [TestCase("ws://example.com/", TestName = "WebSocketScheme")]
    [TestCase("mailto:user@example.com/", TestName = "MailtoScheme")]
    [TestCase("//example.com/path/", TestName = "SchemelessUrl")]
    [TestCase("/relative/path/", TestName = "RelativePath")]
    [TestCase("relative/path/", TestName = "RelativePathNoSlash")]
    [TestCase("example.com/", TestName = "MissingScheme")]
    [TestCase("http//example.com/", TestName = "MissingColonInScheme")]
    [TestCase("https:/example.com/", TestName = "SingleSlashAfterScheme")]
    [TestCase("https://", TestName = "EmptyHost")]
    [TestCase("not a url at all/", TestName = "RandomString")]
    [TestCase("12345", TestName = "NumbersOnly")]
    [TestCase("https:// example.com/", TestName = "SpaceInHost")]
    [TestCase(" /", TestName = "SingleSpace")]
    [TestCase("   /", TestName = "MultipleSpaces")]
    [TestCase("\t/", TestName = "Tab")]
    [TestCase("\n/", TestName = "Newline")]

    // --- Encoding and Illegal Character Violations (caught by IsWellFormedOriginalString) ---
    [TestCase("https://example.com/%2/", TestName = "IncompletePercentEncoding")]
    [TestCase("https://example.com/%zz/", TestName = "InvalidHexPercentEncoding")]
    [TestCase("https://example.com/path%/", TestName = "TrailingPercent")]
    [TestCase("https://example.com/path with space/", TestName = "UnencodedSpaceInPath")]
    [TestCase("https://example.com/path<br>/", TestName = "UnencodedAngleBrackets")]
    [TestCase("https://example.com/|pipe/", TestName = "UnencodedPipeCharacter")]

    // --- Structural Malformations ---
    [TestCase("https://example.com:999999/", TestName = "InvalidPortNumber")]
    [TestCase("http://[::1/", TestName = "MalformedIPv6")]
    [TestCase("http://:80/", TestName = "MissingHostBeforePort")]
    [TestCase("http://user:pass@/", TestName = "MissingHostAfterCredentials")]
    public void ValidatedUrl_InvalidUrl_ThrowsArgumentException(string url)
    {
        var ex = Assert.Throws<InvalidUrlException>(() => UrlValidation.ValidatedUrl(url, string.Empty));
        Assert.That(ex.Message, Does.Contain("Invalid URL"));
    }
}