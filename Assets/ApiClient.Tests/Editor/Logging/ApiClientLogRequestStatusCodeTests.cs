using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using ApiClientLib;
using System.Text;
using System;

public class ApiClientLogRequestStatusCodeTests : ApiClientTestBase
{
    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(204, ExpectedResult = null)]
    [TestCase(256, ExpectedResult = null)]
    [TestCase(401, ExpectedResult = null)]
    [TestCase(403, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator GetAsyncNoBody_CheckRequestStatusCodeLog(int statusCode) => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestStatusCodeLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.GetAsync("api/test"),
            statusCode
        );
    });

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(204, ExpectedResult = null)]
    [TestCase(256, ExpectedResult = null)]
    [TestCase(401, ExpectedResult = null)]
    [TestCase(403, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator GetAsyncWithBody_CheckRequestStatusCodeLog(int statusCode) => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestStatusCodeLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.GetAsync<string>("api/test"),
            statusCode
        );
    });

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(204, ExpectedResult = null)]
    [TestCase(256, ExpectedResult = null)]
    [TestCase(401, ExpectedResult = null)]
    [TestCase(403, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator PostAsyncNoBody_CheckRequestStatusCodeLog(int statusCode) => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestStatusCodeLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PostAsync("api/test", string.Empty),
            statusCode
        );
    });

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(204, ExpectedResult = null)]
    [TestCase(256, ExpectedResult = null)]
    [TestCase(401, ExpectedResult = null)]
    [TestCase(403, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator PostAsyncWithBody_CheckRequestStatusCodeLog(int statusCode) => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestStatusCodeLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PostAsync<string>("api/test", string.Empty),
            statusCode
        );
    });

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(204, ExpectedResult = null)]
    [TestCase(256, ExpectedResult = null)]
    [TestCase(401, ExpectedResult = null)]
    [TestCase(403, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator PutAsyncNoBody_CheckRequestStatusCodeLog(int statusCode) => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestStatusCodeLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PutAsync("api/test", string.Empty),
            statusCode
        );
    });

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(204, ExpectedResult = null)]
    [TestCase(256, ExpectedResult = null)]
    [TestCase(401, ExpectedResult = null)]
    [TestCase(403, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator PutAsyncWithBody_CheckRequestStatusCodeLog(int statusCode) => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestStatusCodeLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PutAsync<string>("api/test", string.Empty),
            statusCode
        );
    });

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(204, ExpectedResult = null)]
    [TestCase(256, ExpectedResult = null)]
    [TestCase(401, ExpectedResult = null)]
    [TestCase(403, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator PatchAsyncNoBody_CheckRequestStatusCodeLog(int statusCode) => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestStatusCodeLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PatchAsync("api/test", string.Empty),
            statusCode
        );
    });

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(204, ExpectedResult = null)]
    [TestCase(256, ExpectedResult = null)]
    [TestCase(401, ExpectedResult = null)]
    [TestCase(403, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator PatchAsyncWithBody_CheckRequestStatusCodeLog(int statusCode) => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestStatusCodeLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.PatchAsync<string>("api/test", string.Empty),
            statusCode
        );
    });

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(204, ExpectedResult = null)]
    [TestCase(256, ExpectedResult = null)]
    [TestCase(401, ExpectedResult = null)]
    [TestCase(403, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator DeleteAsyncNoBody_CheckRequestStatusCodeLog(int statusCode) => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestStatusCodeLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.DeleteAsync("api/test"),
            statusCode
        );
    });

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(204, ExpectedResult = null)]
    [TestCase(256, ExpectedResult = null)]
    [TestCase(401, ExpectedResult = null)]
    [TestCase(403, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator DeleteAsyncWithBody_CheckRequestStatusCodeLog(int statusCode) => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestStatusCodeLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            Client,
            () => Client.DeleteAsync<string>("api/test"),
            statusCode
        );
    });

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(204, ExpectedResult = null)]
    [TestCase(256, ExpectedResult = null)]
    [TestCase(401, ExpectedResult = null)]
    [TestCase(403, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator GetSpriteAsync_CheckRequestStatusCodeLog(int statusCode) => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestStatusCodeLog(
            MockServer,
            RealPngBytes,
            Client,
            () => Client.GetSpriteAsync("test.png"),
            statusCode
        );
    });

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(204, ExpectedResult = null)]
    [TestCase(256, ExpectedResult = null)]
    [TestCase(401, ExpectedResult = null)]
    [TestCase(403, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator GetCachedSpriteAsync_CheckRequestStatusCodeLog(int statusCode) => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestStatusCodeLog(
            MockServer,
            RealPngBytes,
            Client,
            () => Client.GetCachedSpriteAsync("test.png"),
            statusCode
        );
    });

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(204, ExpectedResult = null)]
    [TestCase(256, ExpectedResult = null)]
    [TestCase(401, ExpectedResult = null)]
    [TestCase(403, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator GetAudioClipAsync_CheckRequestStatusCodeLog(int statusCode) => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestStatusCodeLog(
            MockServer,
            RealMp3Bytes,
            Client,
            () => Client.GetAudioClipAsync("test.mp3"),
            statusCode
        );
    });

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(204, ExpectedResult = null)]
    [TestCase(256, ExpectedResult = null)]
    [TestCase(401, ExpectedResult = null)]
    [TestCase(403, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator GetCachedAudioClipAsync_CheckRequestStatusCodeLog(int statusCode) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestHttpRequestStatusCodeLog(
                MockServer,
                RealMp3Bytes,
                Client,
                () => Client.GetCachedAudioClipAsync("test.mp3"),
                statusCode
            );
        });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static async UniTask TestHttpRequestStatusCodeLog(MockHttpServer server, byte[] responseBytes,
        ApiClient client, Func<UniTask> requestFunc, int statusCode)
    {
        // Arrange
        var logCount = 0;
        server.ResponseStatusCode = statusCode;
        server.ResponseBytes = responseBytes;
        client.OnRequestCompleted += CheckLog;

        // Act
        try
        {
            await requestFunc();
        }
        catch (ApiException apiException)
        {
            // Ignored, Since non 2.x.x status codes will throw
        }
        catch (Exception e)
        {
            // Assert: Only ApiException is allowed.
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
            Assert.AreEqual(statusCode, data.StatusCode, $"Expected status code to be {statusCode}");
        }
    }
}