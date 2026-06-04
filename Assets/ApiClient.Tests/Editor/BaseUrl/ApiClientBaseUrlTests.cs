using NUnit.Framework;
using ApiClientLib;
using System;

[TestFixture]
public class ApiClientBaseUrlTests : ApiClientTestBase
{
    [Test]
    public void BaseUrlConstructor_SetsCorrectly_AppendsTrailingSlash()
    {
        // Arrange
        var baseUrl = "https://example.com/api";
        var client = new ApiClient(baseUrl);

        // Assert
        Assert.AreEqual(baseUrl + "/", client.BaseUrl,
            "Base Url must be set and trailing slash should be appended.");
    }

    [Test]
    public void SetBaseUrl_WhenMissingTrailingSlash_AppendsIt()
    {
        // Arrange
        var baseUrl = "https://example.com/api";
        var client = new ApiClient();

        // Act
        client.SetBaseUrl(baseUrl);

        // Assert
        Assert.AreEqual(baseUrl + "/", client.BaseUrl, "Trailing slash should be appended.");
    }

    [Test]
    public void SetBaseUrl_WhenHasTrailingSlash_DoesNotAppendAnother()
    {
        // Arrange
        var baseUrl = "https://example.com/api/";
        var client = new ApiClient();

        // Act
        client.SetBaseUrl(baseUrl);

        // Assert
        Assert.AreEqual(baseUrl, client.BaseUrl, "BaseUrl should remain unchanged.");
    }

    [Test]
    public void BaseUrlConstructor_WhenNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _ = new ApiClient(null), "Should throw on null BaseUrl.");
    }

    [Test]
    public void SetBaseUrl_WhenNull_ThrowsArgumentNullException()
    {
        // Arrange
        var client = new ApiClient();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => client.SetBaseUrl(null), "Should throw on null BaseUrl.");
    }

    [Test]
    public void SetBaseUrl_PlusRelativeUrl_CombinesCorrectly()
    {
        // Arrange
        var baseUrl = "https://example.com/api";
        var relativeUrl = "example/api";
        var client = new ApiClient(baseUrl);

        // Assert
        var finalUrl = client.BaseUrl + relativeUrl;
        Assert.AreEqual(finalUrl, client.CombineUrl(relativeUrl), "BaseUrl and RelativeUrl should append correctly.");
    }
}