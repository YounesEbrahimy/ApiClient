using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using ApiClientLib;
using System.Linq;
using System.Text;
using System;

public class ApiClientLogRequestUrlTests : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetAsyncNoBody_CheckRequestUrlLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestUrlLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.GetAsync(RelativeJsonUrl, queryParams: x),
            MockServer.ServerUrl + RelativeJsonUrl
        );
    });

    [UnityTest]
    public IEnumerator GetAsyncWithBody_CheckRequestUrlLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestUrlLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.GetAsync<string>(RelativeJsonUrl, queryParams: x),
            MockServer.ServerUrl + RelativeJsonUrl
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncNoBody_CheckRequestUrlLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestUrlLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.PostAsync(RelativeJsonUrl, string.Empty, queryParams: x),
            MockServer.ServerUrl + RelativeJsonUrl
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncWithBody_CheckRequestUrlLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestUrlLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.PostAsync<string>(RelativeJsonUrl, string.Empty, queryParams: x),
            MockServer.ServerUrl + RelativeJsonUrl
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncNoBody_CheckRequestUrlLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestUrlLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.PutAsync(RelativeJsonUrl, string.Empty, queryParams: x),
            MockServer.ServerUrl + RelativeJsonUrl
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncWithBody_CheckRequestUrlLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestUrlLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.PutAsync<string>(RelativeJsonUrl, string.Empty, queryParams: x),
            MockServer.ServerUrl + RelativeJsonUrl
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncNoBody_CheckRequestUrlLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestUrlLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.PatchAsync(RelativeJsonUrl, string.Empty, queryParams: x),
            MockServer.ServerUrl + RelativeJsonUrl
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncWithBody_CheckRequestUrlLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestUrlLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.PatchAsync<string>(RelativeJsonUrl, string.Empty, queryParams: x),
            MockServer.ServerUrl + RelativeJsonUrl
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncNoBody_CheckRequestUrlLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestUrlLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.DeleteAsync(RelativeJsonUrl, queryParams: x),
            MockServer.ServerUrl + RelativeJsonUrl
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncWithBody_CheckRequestUrlLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestUrlLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            x => Client.DeleteAsync<string>(RelativeJsonUrl, queryParams: x),
            MockServer.ServerUrl + RelativeJsonUrl
        );
    });

    [UnityTest]
    public IEnumerator GetSpriteAsync_CheckRequestUrlLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestUrlLog(
            MockServer,
            RealPngBytes,
            Client,
            x => Client.GetSpriteAsync(RelativeImageUrl, queryParams: x),
            MockServer.ServerUrl + RelativeImageUrl
        );
    });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_CheckRequestUrlLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestUrlLog(
            MockServer,
            RealPngBytes,
            Client,
            x => Client.GetCachedSpriteAsync(RelativeImageUrl, queryParams: x),
            MockServer.ServerUrl + RelativeImageUrl
        );
    });

    [UnityTest]
    public IEnumerator GetAudioClipAsync_CheckRequestUrlLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestUrlLog(
            MockServer,
            RealMp3Bytes,
            Client,
            x => Client.GetAudioClipAsync(RelativeAudioUrl, queryParams: x),
            MockServer.ServerUrl + RelativeAudioUrl
        );
    });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_CheckRequestUrlLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestUrlLog(
            MockServer,
            RealMp3Bytes,
            Client,
            x => Client.GetCachedAudioClipAsync(RelativeAudioUrl, queryParams: x),
            MockServer.ServerUrl + RelativeAudioUrl
        );
    });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private const string RelativeJsonUrl = "api/test";
    private const string RelativeImageUrl = "api/test.png";
    private const string RelativeAudioUrl = "api/test.mp3";

    private static async UniTask TestHttpRequestUrlLog(MockHttpServer server, byte[] responseBytes, ApiClient client,
        Func<IReadOnlyDictionary<string, string>, UniTask> requestFunc, string expectedUrl)
    {
        // Arrange
        Dictionary<string, string> RequestQueryParams() => new Dictionary<string, string>()
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
            await requestFunc(RequestQueryParams());
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

        // Assertions, Even though request has query params, they should not be in the URL property of ApiEventData
        void CheckLog(ApiEventData data)
        {
            logCount++;
            Assert.AreEqual(expectedUrl, data.URL, $"Expected URL to be {expectedUrl}, but was {data.URL}");
            Assert.IsTrue(RequestQueryParams().SequenceEqual(data.QueryParams),
                "query params of request and log didn't match.");
        }
    }
}