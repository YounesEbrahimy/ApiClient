using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using ApiClientLib;
using System.Text;
using System;

public class ApiClientNullOrEmptyUrlTests : ApiClientTestBase
{
    [UnityTest]
    [TestCase(null, UrlType.Absolute, ExpectedResult = null)]
    [TestCase(null, UrlType.Relative, ExpectedResult = null)]
    [TestCase("", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator GetAsyncNoBody_IfNullOrEmptyUrl_ThrowsRequiredExceptions(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithNullOrEmptyUrl(MockServer, Encoding.UTF8.GetBytes(ServerJsonResponse),
                () => Client.GetAsync(url, urlType: urlType), url, urlType, false);
        });

    [UnityTest]
    [TestCase(null, UrlType.Absolute, ExpectedResult = null)]
    [TestCase(null, UrlType.Relative, ExpectedResult = null)]
    [TestCase("", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator GetAsyncWithBody_IfNullOrEmptyUrl_ThrowsRequiredExceptions(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithNullOrEmptyUrl(MockServer, Encoding.UTF8.GetBytes(ServerJsonResponse),
                () => Client.GetAsync<string>(url, urlType: urlType), url, urlType, false);
        });

    [UnityTest]
    [TestCase(null, UrlType.Absolute, ExpectedResult = null)]
    [TestCase(null, UrlType.Relative, ExpectedResult = null)]
    [TestCase("", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator PostAsyncNoBody_IfNullOrEmptyUrl_ThrowsRequiredExceptions(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithNullOrEmptyUrl(MockServer, Encoding.UTF8.GetBytes(ServerJsonResponse),
                () => Client.PostAsync(url, string.Empty, urlType: urlType), url, urlType, false);
        });

    [UnityTest]
    [TestCase(null, UrlType.Absolute, ExpectedResult = null)]
    [TestCase(null, UrlType.Relative, ExpectedResult = null)]
    [TestCase("", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator PostAsyncWithBody_IfNullOrEmptyUrl_ThrowsRequiredExceptions(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithNullOrEmptyUrl(MockServer, Encoding.UTF8.GetBytes(ServerJsonResponse),
                () => Client.PostAsync<string>(url, string.Empty, urlType: urlType), url, urlType, false);
        });

    [UnityTest]
    [TestCase(null, UrlType.Absolute, ExpectedResult = null)]
    [TestCase(null, UrlType.Relative, ExpectedResult = null)]
    [TestCase("", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator PutAsyncNoBody_IfNullOrEmptyUrl_ThrowsRequiredExceptions(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithNullOrEmptyUrl(MockServer, Encoding.UTF8.GetBytes(ServerJsonResponse),
                () => Client.PutAsync(url, string.Empty, urlType: urlType), url, urlType, false);
        });

    [UnityTest]
    [TestCase(null, UrlType.Absolute, ExpectedResult = null)]
    [TestCase(null, UrlType.Relative, ExpectedResult = null)]
    [TestCase("", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator PutAsyncWithBody_IfNullOrEmptyUrl_ThrowsRequiredExceptions(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithNullOrEmptyUrl(MockServer, Encoding.UTF8.GetBytes(ServerJsonResponse),
                () => Client.PutAsync<string>(url, string.Empty, urlType: urlType), url, urlType, false);
        });

    [UnityTest]
    [TestCase(null, UrlType.Absolute, ExpectedResult = null)]
    [TestCase(null, UrlType.Relative, ExpectedResult = null)]
    [TestCase("", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator PatchAsyncNoBody_IfNullOrEmptyUrl_ThrowsRequiredExceptions(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithNullOrEmptyUrl(MockServer, Encoding.UTF8.GetBytes(ServerJsonResponse),
                () => Client.PatchAsync(url, string.Empty, urlType: urlType), url, urlType, false);
        });

    [UnityTest]
    [TestCase(null, UrlType.Absolute, ExpectedResult = null)]
    [TestCase(null, UrlType.Relative, ExpectedResult = null)]
    [TestCase("", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator PatchAsyncWithBody_IfNullOrEmptyUrl_ThrowsRequiredExceptions(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithNullOrEmptyUrl(MockServer, Encoding.UTF8.GetBytes(ServerJsonResponse),
                () => Client.PatchAsync<string>(url, string.Empty, urlType: urlType), url, urlType, false);
        });

    [UnityTest]
    [TestCase(null, UrlType.Absolute, ExpectedResult = null)]
    [TestCase(null, UrlType.Relative, ExpectedResult = null)]
    [TestCase("", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator DeleteAsyncNoBody_IfNullOrEmptyUrl_ThrowsRequiredExceptions(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithNullOrEmptyUrl(MockServer, Encoding.UTF8.GetBytes(ServerJsonResponse),
                () => Client.DeleteAsync(url, urlType: urlType), url, urlType, false);
        });

    [UnityTest]
    [TestCase(null, UrlType.Absolute, ExpectedResult = null)]
    [TestCase(null, UrlType.Relative, ExpectedResult = null)]
    [TestCase("", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator DeleteAsyncWithBody_IfNullOrEmptyUrl_ThrowsRequiredExceptions(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithNullOrEmptyUrl(MockServer, Encoding.UTF8.GetBytes(ServerJsonResponse),
                () => Client.DeleteAsync<string>(url, urlType: urlType), url, urlType, false);
        });

    [UnityTest]
    [TestCase(null, UrlType.Absolute, ExpectedResult = null)]
    [TestCase(null, UrlType.Relative, ExpectedResult = null)]
    [TestCase("", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator GetSpriteAsync_IfNullOrEmptyUrl_ThrowsRequiredExceptions(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithNullOrEmptyUrl(MockServer, RealPngBytes,
                () => Client.GetSpriteAsync(url, urlType: urlType), url, urlType, false);
        });

    [UnityTest]
    [TestCase(null, UrlType.Absolute, ExpectedResult = null)]
    [TestCase(null, UrlType.Relative, ExpectedResult = null)]
    [TestCase("", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator GetCachedSpriteAsync_IfNullOrEmptyUrl_ThrowsRequiredExceptions(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithNullOrEmptyUrl(MockServer, RealPngBytes,
                () => Client.GetCachedSpriteAsync(url, urlType: urlType), url, urlType, false);
        });

    [UnityTest]
    [TestCase(null, UrlType.Absolute, ExpectedResult = null)]
    [TestCase(null, UrlType.Relative, ExpectedResult = null)]
    [TestCase("", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator GetAudioClipAsync_IfNullOrEmptyUrl_ThrowsRequiredExceptions(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithNullOrEmptyUrl(MockServer, RealMp3Bytes,
                () => Client.GetAudioClipAsync(url, urlType: urlType), url, urlType, true);
        });

    [UnityTest]
    [TestCase(null, UrlType.Absolute, ExpectedResult = null)]
    [TestCase(null, UrlType.Relative, ExpectedResult = null)]
    [TestCase("", UrlType.Absolute, ExpectedResult = null)]
    [TestCase("", UrlType.Relative, ExpectedResult = null)]
    public IEnumerator GetCachedAudioClipAsync_IfNullOrEmptyUrl_ThrowsRequiredExceptions(string url, UrlType urlType) =>
        UniTask.ToCoroutine(async () =>
        {
            await RunTestRequestWithNullOrEmptyUrl(MockServer, RealMp3Bytes,
                () => Client.GetCachedAudioClipAsync(url, urlType: urlType), url, urlType, true);
        });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private const string ServerJsonResponse = "Hello ApiClient";

    private static async UniTask RunTestRequestWithNullOrEmptyUrl(MockHttpServer server, byte[] serverResponse,
        Func<UniTask> requestFunc, string url, UrlType urlType, bool fileExtensionRequired)
    {
        // Arrange:
        var requestHitTheServer = false;
        server.ResponseStatusCode = 200;
        server.ResponseBytes = serverResponse;
        server.OnRequestReceived = _ => requestHitTheServer = true;

        Exception thrownException = null;

        try
        {
            // Act
            await requestFunc();
        }
        catch (Exception e)
        {
            thrownException = e;
        }

        // Assertions: Resource download with required file extensions
        if (fileExtensionRequired)
        {
            Assert.IsFalse(requestHitTheServer, "Server should not have been hit.");
            Assert.IsNotNull(thrownException, "No Exception was thrown.");
            switch (url)
            {
                case null:
                    Assert.IsTrue(thrownException is ArgumentNullException,
                        $"Expected {nameof(ArgumentNullException)}.");
                    Assert.Pass();
                    break;
                case "":
                    Assert.IsTrue(thrownException is InvalidUrlException, $"Expected {nameof(InvalidUrlException)}.");
                    Assert.Pass();
                    break;
                default:
                    Assert.Fail("Test resolution was not done properly.");
                    break;
            }
        }

        // Assertions: Others
        switch (urlType)
        {
            case UrlType.Relative when url == null:
                Assert.IsFalse(requestHitTheServer, "Server should not have been hit.");
                Assert.IsNotNull(thrownException, "No Exception was thrown.");
                Assert.IsTrue(thrownException is ArgumentNullException, $"Expected {nameof(ArgumentNullException)}.");
                Assert.Pass();
                break;
            case UrlType.Relative when url == string.Empty:
                Assert.IsTrue(requestHitTheServer, "Server should have been hit.");
                Assert.IsNull(thrownException, "No Exception should have been thrown.");
                Assert.Pass();
                break;
            case UrlType.Absolute when url == null:
                Assert.IsFalse(requestHitTheServer, "Server should not have been hit.");
                Assert.IsNotNull(thrownException, "No Exception was thrown.");
                Assert.IsTrue(thrownException is ArgumentNullException, $"Expected {nameof(ArgumentNullException)}.");
                Assert.Pass();
                break;
            case UrlType.Absolute when url == string.Empty:
                Assert.IsFalse(requestHitTheServer, "Server should not have been hit.");
                Assert.IsNotNull(thrownException, "No Exception was thrown.");
                Assert.IsTrue(thrownException is InvalidUrlException, $"Expected {nameof(InvalidUrlException)}.");
                Assert.Pass();
                break;
        }

        Assert.Fail("Test resolution was not done properly.");
    }
}