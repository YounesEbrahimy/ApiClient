using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using ApiClientLib;
using System.Text;
using System;

public class ApiClientLogRequestFromCacheTests : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetAsyncNoBody_CheckFromCacheLog() => UniTask.ToCoroutine(async () =>
    {
        await TestNonCachedHttpRequestFromCacheLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.GetAsync("api/test")
        );
    });

    [UnityTest]
    public IEnumerator GetAsyncWithBody_CheckFromCacheLog() => UniTask.ToCoroutine(async () =>
    {
        await TestNonCachedHttpRequestFromCacheLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.GetAsync<string>("api/test")
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncNoBody_CheckFromCacheLog() => UniTask.ToCoroutine(async () =>
    {
        await TestNonCachedHttpRequestFromCacheLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PostAsync("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncWithBody_CheckFromCacheLog() => UniTask.ToCoroutine(async () =>
    {
        await TestNonCachedHttpRequestFromCacheLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PostAsync<string>("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncNoBody_CheckFromCacheLog() => UniTask.ToCoroutine(async () =>
    {
        await TestNonCachedHttpRequestFromCacheLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PutAsync("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncWithBody_CheckFromCacheLog() => UniTask.ToCoroutine(async () =>
    {
        await TestNonCachedHttpRequestFromCacheLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PutAsync<string>("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncNoBody_CheckFromCacheLog() => UniTask.ToCoroutine(async () =>
    {
        await TestNonCachedHttpRequestFromCacheLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PatchAsync("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncWithBody_CheckFromCacheLog() => UniTask.ToCoroutine(async () =>
    {
        await TestNonCachedHttpRequestFromCacheLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PatchAsync<string>("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncNoBody_CheckFromCacheLog() => UniTask.ToCoroutine(async () =>
    {
        await TestNonCachedHttpRequestFromCacheLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.DeleteAsync("api/test")
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncWithBody_CheckFromCacheLog() => UniTask.ToCoroutine(async () =>
    {
        await TestNonCachedHttpRequestFromCacheLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.DeleteAsync<string>("api/test")
        );
    });

    [UnityTest]
    public IEnumerator GetSpriteAsync_CheckFromCacheLog() => UniTask.ToCoroutine(async () =>
    {
        await TestNonCachedHttpRequestFromCacheLog(
            MockServer,
            RealPngBytes,
            Client,
            () => Client.GetSpriteAsync("test.png")
        );
    });

    [UnityTest]
    public IEnumerator GetAudioClipAsync_CheckFromCacheLog() => UniTask.ToCoroutine(async () =>
    {
        await TestNonCachedHttpRequestFromCacheLog(
            MockServer,
            RealMp3Bytes,
            Client,
            () => Client.GetAudioClipAsync("test.mp3")
        );
    });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_CheckFromCacheLog() => UniTask.ToCoroutine(async () =>
    {
        await TestCachedHttpRequestFromCacheLog(
            MockServer,
            RealPngBytes,
            Client,
            () => Client.GetCachedSpriteAsync("test.png")
        );
    });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_CheckFromCacheLog() => UniTask.ToCoroutine(async () =>
    {
        await TestCachedHttpRequestFromCacheLog(
            MockServer,
            RealMp3Bytes,
            Client,
            () => Client.GetCachedAudioClipAsync("test.mp3")
        );
    });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static async UniTask TestNonCachedHttpRequestFromCacheLog(MockHttpServer server, byte[] responseBytes,
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
            Assert.IsFalse(data.FromCache, "This request shouldn't have used cache.");
        }
    }

    private static async UniTask TestCachedHttpRequestFromCacheLog(MockHttpServer server, byte[] responseBytes,
        ApiClient client, Func<UniTask> requestFunc)
    {
        // Arrange
        var requestIndex = 0;
        var logCount = 0;
        server.ResponseStatusCode = 200;
        server.ResponseBytes = responseBytes;
        client.OnRequestCompleted += CheckLog;

        // Act: Send two requests and check cache logs
        try
        {
            await requestFunc();
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

        // Assert: Exactly two logs should have been registered.
        Assert.AreEqual(2, logCount, $"Expected exactly two logs, but got {logCount}");

        return;

        // Assertions
        void CheckLog(ApiEventData data)
        {
            logCount++;

            if (requestIndex == 0)
                Assert.IsFalse(data.FromCache, "First request shouldn't have used cache.");
            else
                Assert.IsTrue(data.FromCache, "Second request should have used cache.");

            requestIndex++;
        }
    }
}