using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using System.Threading;
using NUnit.Framework;
using System;

public class ApiClientCancellationTokenTests : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetAsyncNoBody_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            var cts = new CancellationTokenSource();
            await TestRequestCancellation(
                MockServer,
                Client.GetAsync("api/v1/hang", ct: cts.Token),
                cts
            );
        });

    [UnityTest]
    public IEnumerator GetAsyncWithBody_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            var cts = new CancellationTokenSource();
            await TestRequestCancellation(
                MockServer,
                Client.GetAsync<TestPayload>("api/v1/hang", ct: cts.Token),
                cts
            );
        });

    [UnityTest]
    public IEnumerator PostAsyncNoBody_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            var cts = new CancellationTokenSource();
            await TestRequestCancellation(
                MockServer,
                Client.PostAsync("api/v1/hang", string.Empty, ct: cts.Token),
                cts
            );
        });

    [UnityTest]
    public IEnumerator PostAsyncWithBody_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            var cts = new CancellationTokenSource();
            await TestRequestCancellation(
                MockServer,
                Client.PostAsync<TestPayload>("api/v1/hang", string.Empty, ct: cts.Token),
                cts
            );
        });

    [UnityTest]
    public IEnumerator PutAsyncNoBody_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            var cts = new CancellationTokenSource();
            await TestRequestCancellation(
                MockServer,
                Client.PutAsync("api/v1/hang", string.Empty, ct: cts.Token),
                cts
            );
        });

    [UnityTest]
    public IEnumerator PutAsyncWithBody_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            var cts = new CancellationTokenSource();
            await TestRequestCancellation(
                MockServer,
                Client.PutAsync<TestPayload>("api/v1/hang", string.Empty, ct: cts.Token),
                cts
            );
        });

    [UnityTest]
    public IEnumerator PatchAsyncNoBody_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            var cts = new CancellationTokenSource();
            await TestRequestCancellation(
                MockServer,
                Client.PatchAsync("api/v1/hang", string.Empty, ct: cts.Token),
                cts
            );
        });

    [UnityTest]
    public IEnumerator PatchAsyncWithBody_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            var cts = new CancellationTokenSource();
            await TestRequestCancellation(
                MockServer,
                Client.PatchAsync<TestPayload>("api/v1/hang", string.Empty, ct: cts.Token),
                cts
            );
        });

    [UnityTest]
    public IEnumerator DeleteAsyncNoBody_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            var cts = new CancellationTokenSource();
            await TestRequestCancellation(
                MockServer,
                Client.DeleteAsync("api/v1/hang", ct: cts.Token),
                cts
            );
        });

    [UnityTest]
    public IEnumerator DeleteAsyncWithBody_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            var cts = new CancellationTokenSource();
            await TestRequestCancellation(
                MockServer,
                Client.DeleteAsync<TestPayload>("api/v1/hang", ct: cts.Token),
                cts
            );
        });

    [UnityTest]
    public IEnumerator GetSpriteAsync_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            var cts = new CancellationTokenSource();
            await TestRequestCancellation(
                MockServer,
                Client.GetSpriteAsync(MockServer.ServerUrl + "test.png", ct: cts.Token),
                cts
            );
        });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            var cts = new CancellationTokenSource();
            await TestRequestCancellation(
                MockServer,
                Client.GetCachedSpriteAsync(MockServer.ServerUrl + "test.png", ct: cts.Token),
                cts
            );
        });

    [UnityTest]
    public IEnumerator GetAudioClipAsync_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            var cts = new CancellationTokenSource();
            await TestRequestCancellation(
                MockServer,
                Client.GetAudioClipAsync(MockServer.ServerUrl + "test.mp3", ct: cts.Token),
                cts
            );
        });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            var cts = new CancellationTokenSource();
            await TestRequestCancellation(
                MockServer,
                Client.GetCachedAudioClipAsync(MockServer.ServerUrl + "test.mp3", ct: cts.Token),
                cts
            );
        });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static async UniTask TestRequestCancellation(MockHttpServer server, UniTask requestTask,
        CancellationTokenSource cts)
    {
        // Arrange
        server.DelayMilliseconds = 2000; // Force server to hang for 2 seconds

        // Cancel the token after 50ms
        cts.CancelAfter(50);

        // Act & Assert
        try
        {
            await requestTask;
            Assert.Fail("Expected OperationCanceledException, but the request completed.");
        }
        catch (OperationCanceledException)
        {
            Assert.Pass(); // This is what we want
        }
        catch (Exception e)
        {
            // Assert, Only OperationCanceledException must be thrown
            Assert.Fail();
        }
    }
}