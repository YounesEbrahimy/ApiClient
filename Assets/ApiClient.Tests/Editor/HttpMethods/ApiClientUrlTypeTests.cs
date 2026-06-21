using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using ApiClientLib;
using System.Text;

[TestFixture]
public class ApiClientUrlTypeTests : ApiClientTestBase
{
    [UnityTest]
    [TestCase(UrlType.Relative, ExpectedResult = null)]
    [TestCase(UrlType.Absolute, ExpectedResult = null)]
    public IEnumerator GetAsync_RelativeAndAbsoluteUrl_CombinesCorrectly(UrlType type) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestUrlType(
                MockServer,
                Client,
                type,
                ServerFromStringResponse,
                Client.GetAsync(
                    type == UrlType.Absolute ? MockServer.ServerUrl + ServerRelativePath : ServerRelativePath,
                    urlType: type),
                ServerRelativePath
            );
        });

    [UnityTest]
    [TestCase(UrlType.Relative, ExpectedResult = null)]
    [TestCase(UrlType.Absolute, ExpectedResult = null)]
    public IEnumerator GetAsyncWithBody_RelativeAndAbsoluteUrl_CombinesCorrectly(UrlType type) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestUrlType(
                MockServer,
                Client,
                type,
                ServerFromStringResponse,
                Client.GetAsync<string>(
                    type == UrlType.Absolute ? MockServer.ServerUrl + ServerRelativePath : ServerRelativePath,
                    urlType: type),
                ServerRelativePath
            );
        });

    [UnityTest]
    [TestCase(UrlType.Relative, ExpectedResult = null)]
    [TestCase(UrlType.Absolute, ExpectedResult = null)]
    public IEnumerator PostAsync_RelativeAndAbsoluteUrl_CombinesCorrectly(UrlType type) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestUrlType(
                MockServer,
                Client,
                type,
                ServerFromStringResponse,
                Client.PostAsync(
                    type == UrlType.Absolute ? MockServer.ServerUrl + ServerRelativePath : ServerRelativePath,
                    DummyRequestBody, urlType: type),
                ServerRelativePath
            );
        });

    [UnityTest]
    [TestCase(UrlType.Relative, ExpectedResult = null)]
    [TestCase(UrlType.Absolute, ExpectedResult = null)]
    public IEnumerator PostAsyncWithBody_RelativeAndAbsoluteUrl_CombinesCorrectly(UrlType type) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestUrlType(
                MockServer,
                Client,
                type,
                ServerFromStringResponse,
                Client.PostAsync<string>(
                    type == UrlType.Absolute ? MockServer.ServerUrl + ServerRelativePath : ServerRelativePath,
                    DummyRequestBody, urlType: type),
                ServerRelativePath
            );
        });

    [UnityTest]
    [TestCase(UrlType.Relative, ExpectedResult = null)]
    [TestCase(UrlType.Absolute, ExpectedResult = null)]
    public IEnumerator PutAsync_RelativeAndAbsoluteUrl_CombinesCorrectly(UrlType type) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestUrlType(
                MockServer,
                Client,
                type,
                ServerFromStringResponse,
                Client.PutAsync(
                    type == UrlType.Absolute ? MockServer.ServerUrl + ServerRelativePath : ServerRelativePath,
                    DummyRequestBody, urlType: type),
                ServerRelativePath
            );
        });

    [UnityTest]
    [TestCase(UrlType.Relative, ExpectedResult = null)]
    [TestCase(UrlType.Absolute, ExpectedResult = null)]
    public IEnumerator PutAsyncWithBody_RelativeAndAbsoluteUrl_CombinesCorrectly(UrlType type) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestUrlType(
                MockServer,
                Client,
                type,
                ServerFromStringResponse,
                Client.PutAsync<string>(
                    type == UrlType.Absolute ? MockServer.ServerUrl + ServerRelativePath : ServerRelativePath,
                    DummyRequestBody, urlType: type),
                ServerRelativePath
            );
        });

    [UnityTest]
    [TestCase(UrlType.Relative, ExpectedResult = null)]
    [TestCase(UrlType.Absolute, ExpectedResult = null)]
    public IEnumerator PatchAsync_RelativeAndAbsoluteUrl_CombinesCorrectly(UrlType type) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestUrlType(
                MockServer,
                Client,
                type,
                ServerFromStringResponse,
                Client.PatchAsync(
                    type == UrlType.Absolute ? MockServer.ServerUrl + ServerRelativePath : ServerRelativePath,
                    DummyRequestBody, urlType: type),
                ServerRelativePath
            );
        });

    [UnityTest]
    [TestCase(UrlType.Relative, ExpectedResult = null)]
    [TestCase(UrlType.Absolute, ExpectedResult = null)]
    public IEnumerator PatchAsyncWithBody_RelativeAndAbsoluteUrl_CombinesCorrectly(UrlType type) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestUrlType(
                MockServer,
                Client,
                type,
                ServerFromStringResponse,
                Client.PatchAsync<string>(
                    type == UrlType.Absolute ? MockServer.ServerUrl + ServerRelativePath : ServerRelativePath,
                    DummyRequestBody, urlType: type),
                ServerRelativePath
            );
        });

    [UnityTest]
    [TestCase(UrlType.Relative, ExpectedResult = null)]
    [TestCase(UrlType.Absolute, ExpectedResult = null)]
    public IEnumerator DeleteAsync_RelativeAndAbsoluteUrl_CombinesCorrectly(UrlType type) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestUrlType(
                MockServer,
                Client,
                type,
                ServerFromStringResponse,
                Client.DeleteAsync(
                    type == UrlType.Absolute ? MockServer.ServerUrl + ServerRelativePath : ServerRelativePath,
                    urlType: type),
                ServerRelativePath
            );
        });

    [UnityTest]
    [TestCase(UrlType.Relative, ExpectedResult = null)]
    [TestCase(UrlType.Absolute, ExpectedResult = null)]
    public IEnumerator DeleteAsyncWithBody_RelativeAndAbsoluteUrl_CombinesCorrectly(UrlType type) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestUrlType(
                MockServer,
                Client,
                type,
                ServerFromStringResponse,
                Client.DeleteAsync<string>(
                    type == UrlType.Absolute ? MockServer.ServerUrl + ServerRelativePath : ServerRelativePath,
                    urlType: type),
                ServerRelativePath
            );
        });

    [UnityTest]
    [TestCase(UrlType.Relative, ExpectedResult = null)]
    [TestCase(UrlType.Absolute, ExpectedResult = null)]
    public IEnumerator GetSpriteAsync_RelativeAndAbsoluteUrl_CombinesCorrectly(UrlType type) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestUrlType(
                MockServer,
                Client,
                type,
                RealPngBytes,
                Client.GetSpriteAsync(
                    type == UrlType.Absolute
                        ? MockServer.ServerUrl + ServerRelativePathSprite
                        : ServerRelativePathSprite,
                    urlType: type),
                ServerRelativePathSprite
            );
        });

    [UnityTest]
    [TestCase(UrlType.Relative, ExpectedResult = null)]
    [TestCase(UrlType.Absolute, ExpectedResult = null)]
    public IEnumerator GetCachedSpriteAsync_RelativeAndAbsoluteUrl_CombinesCorrectly(UrlType type) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestUrlType(
                MockServer,
                Client,
                type,
                RealPngBytes,
                Client.GetCachedSpriteAsync(
                    type == UrlType.Absolute
                        ? MockServer.ServerUrl + ServerRelativePathSprite
                        : ServerRelativePathSprite,
                    urlType: type),
                ServerRelativePathSprite
            );
        });

    [UnityTest]
    [TestCase(UrlType.Relative, ExpectedResult = null)]
    [TestCase(UrlType.Absolute, ExpectedResult = null)]
    public IEnumerator GetAudioClipAsync_RelativeAndAbsoluteUrl_CombinesCorrectly(UrlType type) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestUrlType(
                MockServer,
                Client,
                type,
                RealMp3Bytes,
                Client.GetAudioClipAsync(
                    type == UrlType.Absolute
                        ? MockServer.ServerUrl + ServerRelativePathAudioClip
                        : ServerRelativePathAudioClip,
                    urlType: type),
                ServerRelativePathAudioClip
            );
        });

    [UnityTest]
    [TestCase(UrlType.Relative, ExpectedResult = null)]
    [TestCase(UrlType.Absolute, ExpectedResult = null)]
    public IEnumerator GetCachedAudioClipAsync_RelativeAndAbsoluteUrl_CombinesCorrectly(UrlType type) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestUrlType(
                MockServer,
                Client,
                type,
                RealMp3Bytes,
                Client.GetCachedAudioClipAsync(
                    type == UrlType.Absolute
                        ? MockServer.ServerUrl + ServerRelativePathAudioClip
                        : ServerRelativePathAudioClip,
                    urlType: type),
                ServerRelativePathAudioClip
            );
        });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static byte[] ServerFromStringResponse => Encoding.UTF8.GetBytes("Server-Response");
    private const string ServerRelativePath = "api/test";
    private const string ServerRelativePathSprite = "api/test.png";
    private const string ServerRelativePathAudioClip = "api/test.mp3";
    private const string DummyRequestBody = "DummyRequestBody";

    private async UniTask TestRequestUrlType(MockHttpServer server, ApiClient client, UrlType urlType,
        byte[] serverResponse, UniTask act, string relativeUrl)
    {
        // Arrange
        string requestPath = null;
        server.ResponseStatusCode = 200;
        server.ResponseBytes = serverResponse;
        server.OnRequestReceived = req => { requestPath = req.Url.AbsoluteUri; };

        var isRelative = urlType == UrlType.Relative;
        if (isRelative)
            client.SetBaseUrl(server.ServerUrl);

        // Act
        await act;

        // Assert
        Assert.IsNotNull(requestPath, "Server spy hook was never triggered.");
        Assert.AreEqual(
            requestPath,
            server.ServerUrl + relativeUrl,
            isRelative ? "Base url and relative url were not combined correctly." : "Absolute url was not correct."
        );
    }
}