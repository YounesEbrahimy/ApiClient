using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using ApiClientLib;
using System.Linq;
using System.Text;
using System;

public class ApiClientLogResponseHeadersTests : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetAsyncNoBody_CheckResponseHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseHeadersLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.GetAsync("api/test")
        );
    });

    [UnityTest]
    public IEnumerator GetAsyncWithBody_CheckResponseHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseHeadersLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.GetAsync<string>("api/test")
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncNoBody_CheckResponseHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseHeadersLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PostAsync("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncWithBody_CheckResponseHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseHeadersLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PostAsync<string>("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncNoBody_CheckResponseHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseHeadersLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PutAsync("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncWithBody_CheckResponseHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseHeadersLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PutAsync<string>("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncNoBody_CheckResponseHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseHeadersLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PatchAsync("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncWithBody_CheckResponseHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseHeadersLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PatchAsync<string>("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncNoBody_CheckResponseHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseHeadersLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.DeleteAsync("api/test")
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncWithBody_CheckResponseHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseHeadersLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.DeleteAsync<string>("api/test")
        );
    });

    [UnityTest]
    public IEnumerator GetSpriteAsync_CheckResponseHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseHeadersLog(
            MockServer,
            RealPngBytes,
            Client,
            () => Client.GetSpriteAsync("test.png")
        );
    });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_CheckResponseHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseHeadersLog(
            MockServer,
            RealPngBytes,
            Client,
            () => Client.GetCachedSpriteAsync("test.png")
        );
    });

    [UnityTest]
    public IEnumerator GetAudioClipAsync_CheckResponseHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseHeadersLog(
            MockServer,
            RealMp3Bytes,
            Client,
            () => Client.GetAudioClipAsync("test.mp3")
        );
    });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_CheckResponseHeadersLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseHeadersLog(
            MockServer,
            RealMp3Bytes,
            Client,
            () => Client.GetCachedAudioClipAsync("test.mp3")
        );
    });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static async UniTask TestHttpResponseHeadersLog(MockHttpServer server, byte[] responseBytes,
        ApiClient client, Func<UniTask> requestFunc)
    {
        // Arrange
        Dictionary<string, string> ResponseHeaders() => new Dictionary<string, string>()
        {
            { "k1", "v1" },
            { "k2", "v2" }
        };

        var logCount = 0;
        server.ResponseStatusCode = 200;
        server.ResponseBytes = responseBytes;
        server.ResponseHeaders = ResponseHeaders();
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
            Assert.IsTrue(ResponseHeaders().All(pair =>
                    data.ResponseHeaders.TryGetValue(pair.Key, out var value) &&
                    EqualityComparer<string>.Default.Equals(value, pair.Value)),
                "Response headers of server and log didn't match.");
        }
    }
}