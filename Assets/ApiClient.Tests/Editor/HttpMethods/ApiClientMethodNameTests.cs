using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using System.Text;
using System;

public class ApiClientMethodNameTests : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetAsyncNoBody_CheckMethodName() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodName(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            () => Client.GetAsync("api/test"),
            "GET"
        );
    });

    [UnityTest]
    public IEnumerator GetAsyncWithBody_CheckMethodName() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodName(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            () => Client.GetAsync<string>("api/test"),
            "GET"
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncNoBody_CheckMethodName() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodName(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            () => Client.PostAsync("api/test", string.Empty),
            "POST"
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncWithBody_CheckMethodName() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodName(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            () => Client.PostAsync<string>("api/test", string.Empty),
            "POST"
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncNoBody_CheckMethodName() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodName(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            () => Client.PutAsync("api/test", string.Empty),
            "PUT"
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncWithBody_CheckMethodName() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodName(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            () => Client.PutAsync<string>("api/test", string.Empty),
            "PUT"
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncNoBody_CheckMethodName() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodName(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            () => Client.PatchAsync("api/test", string.Empty),
            "PATCH"
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncWithBody_CheckMethodName() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodName(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            () => Client.PatchAsync<string>("api/test", string.Empty),
            "PATCH"
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncNoBody_CheckMethodName() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodName(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            () => Client.DeleteAsync("api/test"),
            "DELETE"
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncWithBody_CheckMethodName() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodName(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            () => Client.DeleteAsync<string>("api/test"),
            "DELETE"
        );
    });

    [UnityTest]
    public IEnumerator GetSpriteAsync_CheckMethodName() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodName(
            MockServer,
            RealPngBytes,
            () => Client.GetSpriteAsync("test.png"),
            "GET"
        );
    });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_CheckMethodName() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodName(
            MockServer,
            RealPngBytes,
            () => Client.GetCachedSpriteAsync("test.png"),
            "GET"
        );
    });

    [UnityTest]
    public IEnumerator GetAudioClipAsync_CheckMethodName() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodName(
            MockServer,
            RealMp3Bytes,
            () => Client.GetAudioClipAsync("test.mp3"),
            "GET"
        );
    });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_CheckMethodName() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodName(
            MockServer,
            RealMp3Bytes,
            () => Client.GetCachedAudioClipAsync("test.mp3"),
            "GET"
        );
    });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static async UniTask TestHttpRequestMethodName(MockHttpServer server, byte[] responseBytes,
        Func<UniTask> requestFunc, string method)
    {
        // Arrange
        var spyHookTriggered = false;
        server.ResponseStatusCode = 200;
        server.ResponseBytes = responseBytes;
        server.OnRequestReceived = request =>
        {
            spyHookTriggered = true;
            Assert.AreEqual(request.HttpMethod.ToLower(), method.ToLower());
        };

        // Act
        await requestFunc();

        // Assert
        Assert.IsTrue(spyHookTriggered, "The MockServer spy hook was never triggered.");
    }
}