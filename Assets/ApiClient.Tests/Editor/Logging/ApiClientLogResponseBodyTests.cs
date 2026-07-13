using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using Newtonsoft.Json;
using NUnit.Framework;
using ApiClientLib;
using System.Text;
using System;

public class ApiClientLogResponseBodyTests : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetAsyncNoBody_CheckResponseBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseBodyLog(
            MockServer,
            JsonResponsePayLoadBytes,
            Client,
            () => Client.GetAsync("api/test"),
            false
        );
    });

    [UnityTest]
    public IEnumerator GetAsyncWithBody_CheckResponseBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseBodyLog(
            MockServer,
            JsonResponsePayLoadBytes,
            Client,
            () => Client.GetAsync<string>("api/test"),
            true
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncNoBody_CheckResponseBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseBodyLog(
            MockServer,
            JsonResponsePayLoadBytes,
            Client,
            () => Client.PostAsync("api/test", string.Empty),
            false
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncWithBody_CheckResponseBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseBodyLog(
            MockServer,
            JsonResponsePayLoadBytes,
            Client,
            () => Client.PostAsync<string>("api/test", string.Empty),
            true
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncNoBody_CheckResponseBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseBodyLog(
            MockServer,
            JsonResponsePayLoadBytes,
            Client,
            () => Client.PutAsync("api/test", string.Empty),
            false
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncWithBody_CheckResponseBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseBodyLog(
            MockServer,
            JsonResponsePayLoadBytes,
            Client,
            () => Client.PutAsync<string>("api/test", string.Empty),
            true
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncNoBody_CheckResponseBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseBodyLog(
            MockServer,
            JsonResponsePayLoadBytes,
            Client,
            () => Client.PatchAsync("api/test", string.Empty),
            false
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncWithBody_CheckResponseBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseBodyLog(
            MockServer,
            JsonResponsePayLoadBytes,
            Client,
            () => Client.PatchAsync<string>("api/test", string.Empty),
            true
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncNoBody_CheckResponseBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseBodyLog(
            MockServer,
            JsonResponsePayLoadBytes,
            Client,
            () => Client.DeleteAsync("api/test"),
            false
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncWithBody_CheckResponseBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseBodyLog(
            MockServer,
            JsonResponsePayLoadBytes,
            Client,
            () => Client.DeleteAsync<string>("api/test"),
            true
        );
    });

    [UnityTest]
    public IEnumerator GetSpriteAsync_CheckResponseBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseBodyLog(
            MockServer,
            RealPngBytes,
            Client,
            () => Client.GetSpriteAsync("test.png"),
            false
        );
    });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_CheckResponseBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseBodyLog(
            MockServer,
            RealPngBytes,
            Client,
            () => Client.GetCachedSpriteAsync("test.png"),
            false
        );
    });

    [UnityTest]
    public IEnumerator GetAudioClipAsync_CheckResponseBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseBodyLog(
            MockServer,
            RealMp3Bytes,
            Client,
            () => Client.GetAudioClipAsync("test.mp3"),
            false
        );
    });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_CheckResponseBodyLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpResponseBodyLog(
            MockServer,
            RealMp3Bytes,
            Client,
            () => Client.GetCachedAudioClipAsync("test.mp3"),
            false
        );
    });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static string JsonResponsePayLoadString =>
        JsonConvert.SerializeObject(TestPayload.GetPayloadByKey(TestPayload.PayloadKey.Leon));

    private static byte[] JsonResponsePayLoadBytes =>
        Encoding.UTF8.GetBytes(JsonResponsePayLoadString);

    private static async UniTask TestHttpResponseBodyLog(MockHttpServer server, byte[] responseBytes, ApiClient client,
        Func<UniTask> requestFunc, bool mustLogResponseBody)
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
            if (mustLogResponseBody)
                Assert.AreEqual(JsonResponsePayLoadString, data.ResponseBody, "Body of response and log didn't match.");
            else
                Assert.IsNull(data.ResponseBody, "This request response shouldn't have had any body.");
        }
    }
}