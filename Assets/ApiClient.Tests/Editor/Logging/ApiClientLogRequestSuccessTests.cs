using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using ApiClientLib;
using System.Text;
using System;

public class ApiClientLogRequestSuccessTests : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetAsyncNoBody_CheckSuccessLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.GetAsync("api/test")
        );
    });

    [UnityTest]
    public IEnumerator GetAsyncWithBody_CheckSuccessLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.GetAsync<string>("api/test")
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncNoBody_CheckSuccessLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PostAsync("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncWithBody_CheckSuccessLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PostAsync<string>("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncNoBody_CheckSuccessLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PutAsync("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncWithBody_CheckSuccessLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PutAsync<string>("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncNoBody_CheckSuccessLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PatchAsync("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncWithBody_CheckSuccessLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PatchAsync<string>("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncNoBody_CheckSuccessLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.DeleteAsync("api/test")
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncWithBody_CheckSuccessLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.DeleteAsync<string>("api/test")
        );
    });

    [UnityTest]
    public IEnumerator GetSpriteAsync_CheckSuccessLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            RealPngBytes,
            Client,
            () => Client.GetSpriteAsync("test.png")
        );
    });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_CheckSuccessLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            RealPngBytes,
            Client,
            () => Client.GetCachedSpriteAsync("test.png")
        );
    });

    [UnityTest]
    public IEnumerator GetAudioClipAsync_CheckSuccessLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            RealMp3Bytes,
            Client,
            () => Client.GetAudioClipAsync("test.mp3")
        );
    });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_CheckSuccessLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            RealMp3Bytes,
            Client,
            () => Client.GetCachedAudioClipAsync("test.mp3")
        );
    });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static async UniTask TestHttpRequestSuccessLog(MockHttpServer server, byte[] responseBytes,
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
            Assert.IsTrue(data.Success, "Expected a successful request.");
            Assert.IsNull(data.Exception, "Expected Exception to be null.");
        }
    }
}