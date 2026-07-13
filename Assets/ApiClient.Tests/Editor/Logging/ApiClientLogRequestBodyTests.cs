using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using Newtonsoft.Json;
using NUnit.Framework;
using ApiClientLib;
using System.Text;
using System;

public class ApiClientLogRequestBodyTests : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetAsyncNoBody_CheckRequestBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestBodyLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.GetAsync("api/test"),
            false
        );
    });

    [UnityTest]
    public IEnumerator GetAsyncWithBody_CheckRequestBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestBodyLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.GetAsync<string>("api/test"),
            false
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncNoBody_CheckRequestBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestBodyLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.PostAsync("api/test", x),
            true
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncWithBody_CheckRequestBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestBodyLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.PostAsync<string>("api/test", x),
            true
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncNoBody_CheckRequestBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestBodyLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.PutAsync("api/test", x),
            true
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncWithBody_CheckRequestBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestBodyLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.PutAsync<string>("api/test", x),
            true
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncNoBody_CheckRequestBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestBodyLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.PatchAsync("api/test", x),
            true
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncWithBody_CheckRequestBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestBodyLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.PatchAsync<string>("api/test", x),
            true
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncNoBody_CheckRequestBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestBodyLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.DeleteAsync("api/test"),
            false
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncWithBody_CheckRequestBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestBodyLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.DeleteAsync<string>("api/test"),
            false
        );
    });

    [UnityTest]
    public IEnumerator GetSpriteAsync_CheckRequestBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestBodyLog(
            MockServer,
            RealPngBytes,
            Client,
            x => Client.GetSpriteAsync("test.png"),
            false
        );
    });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_CheckRequestBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestBodyLog(
            MockServer,
            RealPngBytes,
            Client,
            x => Client.GetCachedSpriteAsync("test.png"),
            false
        );
    });

    [UnityTest]
    public IEnumerator GetAudioClipAsync_CheckRequestBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestBodyLog(
            MockServer,
            RealMp3Bytes,
            Client,
            x => Client.GetAudioClipAsync("test.mp3"),
            false
        );
    });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_CheckRequestBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestBodyLog(
            MockServer,
            RealMp3Bytes,
            Client,
            x => Client.GetCachedAudioClipAsync("test.mp3"),
            false
        );
    });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static async UniTask TestHttpRequestBodyLog(MockHttpServer server, byte[] responseBytes, ApiClient client,
        Func<TestPayload, UniTask> requestFunc, bool canHaveRequestBody)
    {
        // Arrange
        TestPayload RequestPayLoad() => TestPayload.GetPayloadByKey(TestPayload.PayloadKey.Leon);
        var logCount = 0;
        server.ResponseStatusCode = 200;
        server.ResponseBytes = responseBytes;
        client.OnRequestCompleted += CheckLog;

        // Act
        try
        {
            await requestFunc(RequestPayLoad());
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
            if (canHaveRequestBody)
                Assert.AreEqual(JsonConvert.SerializeObject(RequestPayLoad()), data.RequestBody,
                    "Body of Request and log didn't match.");
            else
                Assert.IsNull(data.RequestBody, "This Request shouldn't have had any body.");
        }
    }
}