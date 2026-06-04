using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using System;

public class ApiClientTimeoutTests : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetAsyncNoBody_WhenTimeout_ThrowsTimeoutException() =>
        UniTask.ToCoroutine(async () =>
        {
            var timeout = 1;
            await TestRequestTimeout(
                MockServer,
                Client.GetAsync("api/v1/hang", timeout: timeout),
                timeout
            );
        });

    [UnityTest]
    public IEnumerator GetAsyncWithBody_WhenTimeout_ThrowsTimeoutException() =>
        UniTask.ToCoroutine(async () =>
        {
            var timeout = 1;
            await TestRequestTimeout(
                MockServer,
                Client.GetAsync<TestPayload>("api/v1/hang", timeout: timeout),
                timeout
            );
        });

    [UnityTest]
    public IEnumerator PostAsyncNoBody_WhenTimeout_ThrowsTimeoutException() =>
        UniTask.ToCoroutine(async () =>
        {
            var timeout = 1;
            await TestRequestTimeout(
                MockServer,
                Client.PostAsync("api/v1/hang", string.Empty, timeout: timeout),
                timeout
            );
        });

    [UnityTest]
    public IEnumerator PostAsyncWithBody_WhenTimeout_ThrowsTimeoutException() =>
        UniTask.ToCoroutine(async () =>
        {
            var timeout = 1;
            await TestRequestTimeout(
                MockServer,
                Client.PostAsync<TestPayload>("api/v1/hang", string.Empty, timeout: timeout),
                timeout
            );
        });

    [UnityTest]
    public IEnumerator PutAsyncNoBody_WhenTimeout_ThrowsTimeoutException() =>
        UniTask.ToCoroutine(async () =>
        {
            var timeout = 1;
            await TestRequestTimeout(
                MockServer,
                Client.PutAsync("api/v1/hang", string.Empty, timeout: timeout),
                timeout
            );
        });

    [UnityTest]
    public IEnumerator PutAsyncWithBody_WhenTimeout_ThrowsTimeoutException() =>
        UniTask.ToCoroutine(async () =>
        {
            var timeout = 1;
            await TestRequestTimeout(
                MockServer,
                Client.PutAsync<TestPayload>("api/v1/hang", string.Empty, timeout: timeout),
                timeout
            );
        });

    [UnityTest]
    public IEnumerator PatchAsyncNoBody_WhenTimeout_ThrowsTimeoutException() =>
        UniTask.ToCoroutine(async () =>
        {
            var timeout = 1;
            await TestRequestTimeout(
                MockServer,
                Client.PatchAsync("api/v1/hang", string.Empty, timeout: timeout),
                timeout
            );
        });

    [UnityTest]
    public IEnumerator PatchAsyncWithBody_WhenTimeout_ThrowsTimeoutException() =>
        UniTask.ToCoroutine(async () =>
        {
            var timeout = 1;
            await TestRequestTimeout(
                MockServer,
                Client.PatchAsync<TestPayload>("api/v1/hang", string.Empty, timeout: timeout),
                timeout
            );
        });

    [UnityTest]
    public IEnumerator DeleteAsyncNoBody_WhenTimeout_ThrowsTimeoutException() =>
        UniTask.ToCoroutine(async () =>
        {
            var timeout = 1;
            await TestRequestTimeout(
                MockServer,
                Client.DeleteAsync("api/v1/hang", timeout: timeout),
                timeout
            );
        });

    [UnityTest]
    public IEnumerator DeleteAsyncWithBody_WhenTimeout_ThrowsTimeoutException() =>
        UniTask.ToCoroutine(async () =>
        {
            var timeout = 1;
            await TestRequestTimeout(
                MockServer,
                Client.DeleteAsync<TestPayload>("api/v1/hang", timeout: timeout),
                timeout
            );
        });

    [UnityTest]
    public IEnumerator GetSpriteAsync_WhenTimeout_ThrowsTimeoutException() =>
        UniTask.ToCoroutine(async () =>
        {
            var timeout = 1;
            await TestRequestTimeout(
                MockServer,
                Client.GetSpriteAsync(MockServer.ServerUrl + "test.png", timeout: timeout),
                timeout
            );
        });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_WhenTimeout_ThrowsTimeoutException() =>
        UniTask.ToCoroutine(async () =>
        {
            var timeout = 1;
            await TestRequestTimeout(
                MockServer,
                Client.GetCachedSpriteAsync(MockServer.ServerUrl + "test.png", timeout: timeout),
                timeout
            );
        });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static async UniTask TestRequestTimeout(MockHttpServer server, UniTask requestTask, int timeout)
    {
        // Arrange
        server.DelayMilliseconds = (timeout + 1) * 1000; // Force server to wait more than the request's timeout

        // Act & Assert
        try
        {
            await requestTask;
            Assert.Fail("Expected TimeoutException, but the request completed.");
        }
        catch (TimeoutException)
        {
            Assert.Pass(); // This is what we want
        }
        catch (Exception e)
        {
            // Assert, Only TimeoutException must be thrown
            Assert.Fail();
        }
    }
}