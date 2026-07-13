using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using ApiClientLib;
using System.Text;
using System;

public class ApiClientLogRequestTimeoutTests : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetAsyncNoBody_CheckRequestTimeoutLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimeoutLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.GetAsync("api/test", timeout: x)
        );
    });

    [UnityTest]
    public IEnumerator GetAsyncWithBody_CheckRequestTimeoutLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimeoutLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.GetAsync<string>("api/test", timeout: x)
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncNoBody_CheckRequestTimeoutLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimeoutLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.PostAsync("api/test", string.Empty, timeout: x)
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncWithBody_CheckRequestTimeoutLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimeoutLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.PostAsync<string>("api/test", string.Empty, timeout: x)
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncNoBody_CheckRequestTimeoutLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimeoutLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.PutAsync("api/test", string.Empty, timeout: x)
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncWithBody_CheckRequestTimeoutLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimeoutLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.PutAsync<string>("api/test", string.Empty, timeout: x)
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncNoBody_CheckRequestTimeoutLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimeoutLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.PatchAsync("api/test", string.Empty, timeout: x)
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncWithBody_CheckRequestTimeoutLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimeoutLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.PatchAsync<string>("api/test", string.Empty, timeout: x)
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncNoBody_CheckRequestTimeoutLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimeoutLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.DeleteAsync("api/test", timeout: x)
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncWithBody_CheckRequestTimeoutLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimeoutLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.DeleteAsync<string>("api/test", timeout: x)
        );
    });

    [UnityTest]
    public IEnumerator GetSpriteAsync_CheckRequestTimeoutLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimeoutLog(
            MockServer,
            RealPngBytes,
            Client,
            x => Client.GetSpriteAsync("test.png", timeout: x)
        );
    });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_CheckRequestTimeoutLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimeoutLog(
            MockServer,
            RealPngBytes,
            Client,
            x => Client.GetCachedSpriteAsync("test.png", timeout: x)
        );
    });

    [UnityTest]
    public IEnumerator GetAudioClipAsync_CheckRequestTimeoutLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimeoutLog(
            MockServer,
            RealMp3Bytes,
            Client,
            x => Client.GetAudioClipAsync("test.mp3", timeout: x)
        );
    });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_CheckRequestTimeoutLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimeoutLog(
            MockServer,
            RealMp3Bytes,
            Client,
            x => Client.GetCachedAudioClipAsync("test.mp3", timeout: x)
        );
    });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static async UniTask TestHttpRequestTimeoutLog(MockHttpServer server, byte[] responseBytes,
        ApiClient client, Func<int, UniTask> requestFunc)
    {
        // Arrange
        const int requestTimeout = 7;
        var logCount = 0;
        server.ResponseStatusCode = 200;
        server.ResponseBytes = responseBytes;
        client.OnRequestCompleted += CheckLog;

        // Act
        try
        {
            await requestFunc(requestTimeout);
        }
        catch (Exception e)
        {
            // Assert: No Exception should have been thrown.
            Assert.Fail();
        }
        finally
        {
            client.OnRequestCompleted -= CheckLog;
        }

        // Assert: Exactly one log should have been registered.
        Assert.AreEqual(1, logCount, $"Expected exactly one log, but got {logCount}");

        return;

        // Assertions
        void CheckLog(ApiEventData data)
        {
            logCount++;
            Assert.AreEqual(requestTimeout, data.Timeout, $"Expected timeout to be {requestTimeout}");
        }
    }
}