using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using ApiClientLib;
using System.Text;
using System;

public class ApiClientMethodsInvalidUrlTests : ApiClientTestBase
{
    [UnityTest]
    [TestCase("%zz/bad url", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("%zz/bad url", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator GetAsyncNoBody_IfInvalidUrl_ThrowsInvalidUrlException(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithInvalidUrl(MockServer, Encoding.UTF8.GetBytes(string.Empty),
                () => Client.GetAsync(url, urlType: urlType));
        });

    [UnityTest]
    [TestCase("%zz/bad url", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("%zz/bad url", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator GetAsyncWithBody_IfInvalidUrl_ThrowsInvalidUrlException(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithInvalidUrl(MockServer, Encoding.UTF8.GetBytes(string.Empty),
                () => Client.GetAsync<string>(url, urlType: urlType));
        });

    [UnityTest]
    [TestCase("%zz/bad url", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("%zz/bad url", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator PostAsyncNoBody_IfInvalidUrl_ThrowsInvalidUrlException(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithInvalidUrl(MockServer, Encoding.UTF8.GetBytes(string.Empty),
                () => Client.PostAsync(url, string.Empty, urlType: urlType));
        });

    [UnityTest]
    [TestCase("%zz/bad url", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("%zz/bad url", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator PostAsyncWithBody_IfInvalidUrl_ThrowsInvalidUrlException(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithInvalidUrl(MockServer, Encoding.UTF8.GetBytes(string.Empty),
                () => Client.PostAsync<string>(url, string.Empty, urlType: urlType));
        });

    [UnityTest]
    [TestCase("%zz/bad url", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("%zz/bad url", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator PutAsyncNoBody_IfInvalidUrl_ThrowsInvalidUrlException(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithInvalidUrl(MockServer, Encoding.UTF8.GetBytes(string.Empty),
                () => Client.PutAsync(url, string.Empty, urlType: urlType));
        });

    [UnityTest]
    [TestCase("%zz/bad url", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("%zz/bad url", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator PutAsyncWithBody_IfInvalidUrl_ThrowsInvalidUrlException(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithInvalidUrl(MockServer, Encoding.UTF8.GetBytes(string.Empty),
                () => Client.PutAsync<string>(url, string.Empty, urlType: urlType));
        });

    [UnityTest]
    [TestCase("%zz/bad url", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("%zz/bad url", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator PatchAsyncNoBody_IfInvalidUrl_ThrowsInvalidUrlException(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithInvalidUrl(MockServer, Encoding.UTF8.GetBytes(string.Empty),
                () => Client.PatchAsync(url, string.Empty, urlType: urlType));
        });

    [UnityTest]
    [TestCase("%zz/bad url", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("%zz/bad url", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator PatchAsyncWithBody_IfInvalidUrl_ThrowsInvalidUrlException(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithInvalidUrl(MockServer, Encoding.UTF8.GetBytes(string.Empty),
                () => Client.PatchAsync<string>(url, string.Empty, urlType: urlType));
        });

    [UnityTest]
    [TestCase("%zz/bad url", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("%zz/bad url", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator DeleteAsyncNoBody_IfInvalidUrl_ThrowsInvalidUrlException(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithInvalidUrl(MockServer, Encoding.UTF8.GetBytes(string.Empty),
                () => Client.DeleteAsync(url, urlType: urlType));
        });

    [UnityTest]
    [TestCase("%zz/bad url", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("%zz/bad url", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator DeleteAsyncWithBody_IfInvalidUrl_ThrowsInvalidUrlException(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithInvalidUrl(MockServer, Encoding.UTF8.GetBytes(string.Empty),
                () => Client.DeleteAsync<string>(url, urlType: urlType));
        });

    [UnityTest]
    [TestCase("%zz/bad url.png", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("%zz/bad url.png", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator GetSpriteAsync_IfInvalidUrl_ThrowsInvalidUrlException(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithInvalidUrl(MockServer, RealPngBytes,
                () => Client.GetSpriteAsync(url, urlType: urlType));
        });

    [UnityTest]
    [TestCase("%zz/bad url.png", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("%zz/bad url.png", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator GetCachedSpriteAsync_IfInvalidUrl_ThrowsInvalidUrlException(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithInvalidUrl(MockServer, RealPngBytes,
                () => Client.GetCachedSpriteAsync(url, urlType: urlType));
        });

    [UnityTest]
    [TestCase("%zz/bad url.mp3", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("%zz/bad url.mp3", UrlType.Relative, ExpectedResult = null)]
    [TestCase("%zz/bad url", UrlType.Absolute, ExpectedResult = null)] // No extension url
    [TestCase("%zz/bad url", UrlType.Relative, ExpectedResult = null)] // No extension url
    public IEnumerator GetAudioClipAsync_IfInvalidUrl_ThrowsInvalidUrlException(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithInvalidUrl(MockServer, RealMp3Bytes,
                () => Client.GetAudioClipAsync(url, urlType: urlType));
        });

    [UnityTest]
    [TestCase("%zz/bad url.mp3", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("%zz/bad url.mp3", UrlType.Relative, ExpectedResult = null)]
    [TestCase("%zz/bad url", UrlType.Absolute, ExpectedResult = null)] // No extension url
    [TestCase("%zz/bad url", UrlType.Relative, ExpectedResult = null)] // No extension url
    public IEnumerator GetCachedAudioClipAsync_IfInvalidUrl_ThrowsInvalidUrlException(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithInvalidUrl(MockServer, RealMp3Bytes,
                () => Client.GetCachedAudioClipAsync(url, urlType: urlType));
        });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static async UniTask RunTestRequestWithInvalidUrl(MockHttpServer server, byte[] serverResponse,
        Func<UniTask> requestFunc)
    {
        // Arrange:
        var requestHitTheServer = false;
        server.ResponseStatusCode = 200;
        server.ResponseBytes = serverResponse;
        server.OnRequestReceived = _ => requestHitTheServer = true;

        try
        {
            // Act
            await requestFunc();
        }
        catch (InvalidUrlException e)
        {
            // Assert: This is what we want
            Assert.Pass();
        }
        catch (Exception e)
        {
            // Assert: Wrong type of Exception was thrown
            Assert.Fail("Only InvalidUrlException should have been thrown.");
        }

        // Assert: Request should not hit the server because of it's invalid url
        Assert.IsFalse(requestHitTheServer, "Server should not have been hit.");

        // Assert: No Exception was thrown
        Assert.Fail("No Exception was thrown.");
    }
}