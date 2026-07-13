using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using ApiClientLib;
using System;

public class ApiClientLogRequestExceptionTests : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetAsyncNoBody_CheckExceptionLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestExceptionLog(
            MockServer,
            Client,
            () => Client.GetAsync("api/test")
        );
    });

    [UnityTest]
    public IEnumerator GetAsyncWithBody_CheckExceptionLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestExceptionLog(
            MockServer,
            Client,
            () => Client.GetAsync<string>("api/test")
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncNoBody_CheckExceptionLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestExceptionLog(
            MockServer,
            Client,
            () => Client.PostAsync("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncWithBody_CheckExceptionLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestExceptionLog(
            MockServer,
            Client,
            () => Client.PostAsync<string>("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncNoBody_CheckExceptionLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestExceptionLog(
            MockServer,
            Client,
            () => Client.PutAsync("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncWithBody_CheckExceptionLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestExceptionLog(
            MockServer,
            Client,
            () => Client.PutAsync<string>("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncNoBody_CheckExceptionLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestExceptionLog(
            MockServer,
            Client,
            () => Client.PatchAsync("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncWithBody_CheckExceptionLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestExceptionLog(
            MockServer,
            Client,
            () => Client.PatchAsync<string>("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncNoBody_CheckExceptionLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestExceptionLog(
            MockServer,
            Client,
            () => Client.DeleteAsync("api/test")
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncWithBody_CheckExceptionLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestExceptionLog(
            MockServer,
            Client,
            () => Client.DeleteAsync<string>("api/test")
        );
    });

    [UnityTest]
    public IEnumerator GetSpriteAsync_CheckExceptionLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestExceptionLog(
            MockServer,
            Client,
            () => Client.GetSpriteAsync("test.png")
        );
    });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_CheckExceptionLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestExceptionLog(
            MockServer,
            Client,
            () => Client.GetCachedSpriteAsync("test.png")
        );
    });

    [UnityTest]
    public IEnumerator GetAudioClipAsync_CheckExceptionLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestExceptionLog(
            MockServer,
            Client,
            () => Client.GetAudioClipAsync("test.mp3")
        );
    });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_CheckExceptionLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestExceptionLog(
            MockServer,
            Client,
            () => Client.GetCachedAudioClipAsync("test.mp3")
        );
    });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static async UniTask TestHttpRequestExceptionLog(MockHttpServer server, ApiClient client,
        Func<UniTask> requestFunc)
    {
        // Arrange
        var logCount = 0;
        server.ResponseStatusCode = 403;
        server.ResponseObject = string.Empty;
        client.OnRequestCompleted += CheckLog;

        // Act
        try
        {
            await requestFunc();
            Assert.Fail(); // Assert: Request should have thrown ApiException.
        }
        catch (ApiException ex)
        {
            // Ignored, Since it is supposed to throw.
        }
        catch (Exception e)
        {
            // Assert: Only ApiException should have been thrown.
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
            Assert.IsFalse(data.Success, "Expected a failed request.");
            Assert.IsNotNull(data.Exception, "Exception should not be null.");
            Assert.AreEqual(typeof(ApiException), data.Exception.GetType(),
                "Expected Exception to be an ApiException.");
            Assert.AreEqual(data.ErrorMessage, data.Exception.Message, "Messages of Error and Exception didn't match.");
        }
    }
}