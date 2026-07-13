using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using ApiClientLib;
using System.Linq;
using System.Text;
using System;

public class ApiClientLogRequestHeadersTests : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetAsyncNoBody_CheckRequestHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestHeadersLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.GetAsync("api/test", customHeaders: x)
        );
    });

    [UnityTest]
    public IEnumerator GetAsyncWithBody_CheckRequestHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestHeadersLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.GetAsync<string>("api/test", customHeaders: x)
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncNoBody_CheckRequestHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestHeadersLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.PostAsync("api/test", string.Empty, customHeaders: x)
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncWithBody_CheckRequestHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestHeadersLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.PostAsync<string>("api/test", string.Empty, customHeaders: x)
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncNoBody_CheckRequestHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestHeadersLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.PutAsync("api/test", string.Empty, customHeaders: x)
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncWithBody_CheckRequestHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestHeadersLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.PutAsync<string>("api/test", string.Empty, customHeaders: x)
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncNoBody_CheckRequestHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestHeadersLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.PatchAsync("api/test", string.Empty, customHeaders: x)
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncWithBody_CheckRequestHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestHeadersLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.PatchAsync<string>("api/test", string.Empty, customHeaders: x)
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncNoBody_CheckRequestHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestHeadersLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.DeleteAsync("api/test", customHeaders: x)
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncWithBody_CheckRequestHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestHeadersLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.DeleteAsync<string>("api/test", customHeaders: x)
        );
    });

    [UnityTest]
    public IEnumerator GetSpriteAsync_CheckRequestHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestHeadersLog(
            MockServer,
            RealPngBytes,
            Client,
            x => Client.GetSpriteAsync("test.png", customHeaders: x)
        );
    });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_CheckRequestHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestHeadersLog(
            MockServer,
            RealPngBytes,
            Client,
            x => Client.GetCachedSpriteAsync("test.png", customHeaders: x)
        );
    });

    [UnityTest]
    public IEnumerator GetAudioClipAsync_CheckRequestHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestHeadersLog(
            MockServer,
            RealMp3Bytes,
            Client,
            x => Client.GetAudioClipAsync("test.mp3", customHeaders: x)
        );
    });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_CheckRequestHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestHeadersLog(
            MockServer,
            RealMp3Bytes,
            Client,
            x => Client.GetCachedAudioClipAsync("test.mp3", customHeaders: x)
        );
    });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static async UniTask TestHttpRequestHeadersLog(MockHttpServer server, byte[] responseBytes,
        ApiClient client, Func<IReadOnlyDictionary<string, string>, UniTask> requestFunc)
    {
        // Arrange
        Dictionary<string, string> RequestHeaders() => new Dictionary<string, string>()
        {
            { "k1", "v1" },
            { "k2", "v2" }
        };

        var logCount = 0;
        server.ResponseStatusCode = 200;
        server.ResponseBytes = responseBytes;
        client.OnRequestCompleted += CheckLog;

        // Act
        try
        {
            await requestFunc(RequestHeaders());
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
            Assert.IsTrue(RequestHeaders().SequenceEqual(data.RequestHeaders),
                "Request headers of request and log didn't match.");
        }
    }
}