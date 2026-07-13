using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using ApiClientLib;
using System.Text;
using System;

public class ApiClientLogRequestMethodTests : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetAsyncNoBody_CheckRequestMethodLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.GetAsync("api/test"),
            RequestMethod.GET
        );
    });

    [UnityTest]
    public IEnumerator GetAsyncWithBody_CheckRequestMethodLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.GetAsync<string>("api/test"),
            RequestMethod.GET
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncNoBody_CheckRequestMethodLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PostAsync("api/test", string.Empty),
            RequestMethod.POST
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncWithBody_CheckRequestMethodLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PostAsync<string>("api/test", string.Empty),
            RequestMethod.POST
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncNoBody_CheckRequestMethodLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PutAsync("api/test", string.Empty),
            RequestMethod.PUT
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncWithBody_CheckRequestMethodLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PutAsync<string>("api/test", string.Empty),
            RequestMethod.PUT
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncNoBody_CheckRequestMethodLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PatchAsync("api/test", string.Empty),
            RequestMethod.PATCH
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncWithBody_CheckRequestMethodLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PatchAsync<string>("api/test", string.Empty),
            RequestMethod.PATCH
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncNoBody_CheckRequestMethodLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.DeleteAsync("api/test"),
            RequestMethod.DELETE
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncWithBody_CheckRequestMethodLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.DeleteAsync<string>("api/test"),
            RequestMethod.DELETE
        );
    });

    [UnityTest]
    public IEnumerator GetSpriteAsync_CheckRequestMethodLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodLog(
            MockServer,
            RealPngBytes,
            Client,
            () => Client.GetSpriteAsync("test.png"),
            RequestMethod.GET_SPRITE
        );
    });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_CheckRequestMethodLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodLog(
            MockServer,
            RealPngBytes,
            Client,
            () => Client.GetCachedSpriteAsync("test.png"),
            RequestMethod.GET_SPRITE
        );
    });

    [UnityTest]
    public IEnumerator GetAudioClipAsync_CheckRequestMethodLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodLog(
            MockServer,
            RealMp3Bytes,
            Client,
            () => Client.GetAudioClipAsync("test.mp3"),
            RequestMethod.GET_AUDIOCLIP
        );
    });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_CheckRequestMethodLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestMethodLog(
            MockServer,
            RealMp3Bytes,
            Client,
            () => Client.GetCachedAudioClipAsync("test.mp3"),
            RequestMethod.GET_AUDIOCLIP
        );
    });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static async UniTask TestHttpRequestMethodLog(MockHttpServer server, byte[] responseBytes,
        ApiClient client, Func<UniTask> requestFunc, RequestMethod method)
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
            Assert.AreEqual(method, data.Method, $"Expected method {method}");
        }
    }
}