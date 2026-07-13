using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using ApiClientLib;
using System.Text;
using System;

public class ApiClientLogRequestDurationTests : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetAsyncNoBody_CheckRequestDurationLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestDurationLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.GetAsync("api/test")
        );
    });

    [UnityTest]
    public IEnumerator GetAsyncWithBody_CheckRequestDurationLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestDurationLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.GetAsync<string>("api/test")
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncNoBody_CheckRequestDurationLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestDurationLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PostAsync("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncWithBody_CheckRequestDurationLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestDurationLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PostAsync<string>("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncNoBody_CheckRequestDurationLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestDurationLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PutAsync("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncWithBody_CheckRequestDurationLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestDurationLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PutAsync<string>("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncNoBody_CheckRequestDurationLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestDurationLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PatchAsync("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncWithBody_CheckRequestDurationLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestDurationLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PatchAsync<string>("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncNoBody_CheckRequestDurationLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestDurationLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.DeleteAsync("api/test")
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncWithBody_CheckRequestDurationLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestDurationLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.DeleteAsync<string>("api/test")
        );
    });

    [UnityTest]
    public IEnumerator GetSpriteAsync_CheckRequestDurationLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestDurationLog(
            MockServer,
            RealPngBytes,
            Client,
            () => Client.GetSpriteAsync("test.png")
        );
    });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_CheckRequestDurationLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestDurationLog(
            MockServer,
            RealPngBytes,
            Client,
            () => Client.GetCachedSpriteAsync("test.png")
        );
    });

    [UnityTest]
    public IEnumerator GetAudioClipAsync_CheckRequestDurationLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestDurationLog(
            MockServer,
            RealMp3Bytes,
            Client,
            () => Client.GetAudioClipAsync("test.mp3")
        );
    });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_CheckRequestDurationLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestDurationLog(
            MockServer,
            RealMp3Bytes,
            Client,
            () => Client.GetCachedAudioClipAsync("test.mp3")
        );
    });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static async UniTask TestHttpRequestDurationLog(MockHttpServer server, byte[] responseBytes,
        ApiClient client, Func<UniTask> requestFunc)
    {
        // Arrange
        const int Delay = 100;
        var logCount = 0;
        server.DelayMilliseconds = Delay;
        server.ResponseStatusCode = 200;
        server.ResponseBytes = responseBytes;
        client.OnRequestCompleted += CheckLog;

        // Act
        try
        {
            await requestFunc();
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
            Assert.IsTrue(Math.Abs(data.Duration - (Delay / 1000f)) <= 5, "Timestamp of request and log didn't match.");
        }
    }
}