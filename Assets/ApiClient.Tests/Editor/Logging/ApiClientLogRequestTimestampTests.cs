using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using ApiClientLib;
using System.Text;
using System;

public class ApiClientLogRequestTimestampTests : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetAsyncNoBody_CheckRequestTimestampLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimestampLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.GetAsync("api/test")
        );
    });

    [UnityTest]
    public IEnumerator GetAsyncWithBody_CheckRequestTimestampLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimestampLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.GetAsync<string>("api/test")
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncNoBody_CheckRequestTimestampLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimestampLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PostAsync("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncWithBody_CheckRequestTimestampLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimestampLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PostAsync<string>("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncNoBody_CheckRequestTimestampLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimestampLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PutAsync("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncWithBody_CheckRequestTimestampLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimestampLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PutAsync<string>("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncNoBody_CheckRequestTimestampLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimestampLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PatchAsync("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncWithBody_CheckRequestTimestampLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimestampLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PatchAsync<string>("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncNoBody_CheckRequestTimestampLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimestampLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.DeleteAsync("api/test")
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncWithBody_CheckRequestTimestampLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimestampLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.DeleteAsync<string>("api/test")
        );
    });

    [UnityTest]
    public IEnumerator GetSpriteAsync_CheckRequestTimestampLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimestampLog(
            MockServer,
            RealPngBytes,
            Client,
            () => Client.GetSpriteAsync("test.png")
        );
    });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_CheckRequestTimestampLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimestampLog(
            MockServer,
            RealPngBytes,
            Client,
            () => Client.GetCachedSpriteAsync("test.png")
        );
    });

    [UnityTest]
    public IEnumerator GetAudioClipAsync_CheckRequestTimestampLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimestampLog(
            MockServer,
            RealMp3Bytes,
            Client,
            () => Client.GetAudioClipAsync("test.mp3")
        );
    });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_CheckRequestTimestampLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestTimestampLog(
            MockServer,
            RealMp3Bytes,
            Client,
            () => Client.GetCachedAudioClipAsync("test.mp3")
        );
    });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static async UniTask TestHttpRequestTimestampLog(MockHttpServer server, byte[] responseBytes,
        ApiClient client, Func<UniTask> requestFunc)
    {
        // Arrange
        var logCount = 0;
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
            Assert.IsTrue(Math.Abs((data.Timestamp - DateTime.Now).TotalMilliseconds) <= 10,
                "Timestamp of request and log didn't match.");
        }
    }
}