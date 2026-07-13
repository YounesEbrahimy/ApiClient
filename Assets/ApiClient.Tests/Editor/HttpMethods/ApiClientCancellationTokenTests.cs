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
            await TestRequestCancellation(
                MockServer,
                x => Client.GetAsync("api/v1/hang", ct: x)
            );
        });

    [UnityTest]
    public IEnumerator GetAsyncWithBody_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestCancellation(
                MockServer,
                x => Client.GetAsync<TestPayload>("api/v1/hang", ct: x)
            );
        });

    [UnityTest]
    public IEnumerator PostAsyncNoBody_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestCancellation(
                MockServer,
                x => Client.PostAsync("api/v1/hang", string.Empty, ct: x)
            );
        });

    [UnityTest]
    public IEnumerator PostAsyncWithBody_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestCancellation(
                MockServer,
                x => Client.PostAsync<TestPayload>("api/v1/hang", string.Empty, ct: x)
            );
        });

    [UnityTest]
    public IEnumerator PutAsyncNoBody_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestCancellation(
                MockServer,
                x => Client.PutAsync("api/v1/hang", string.Empty, ct: x)
            );
        });

    [UnityTest]
    public IEnumerator PutAsyncWithBody_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestCancellation(
                MockServer,
                x => Client.PutAsync<TestPayload>("api/v1/hang", string.Empty, ct: x)
            );
        });

    [UnityTest]
    public IEnumerator PatchAsyncNoBody_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestCancellation(
                MockServer,
                x => Client.PatchAsync("api/v1/hang", string.Empty, ct: x)
            );
        });

    [UnityTest]
    public IEnumerator PatchAsyncWithBody_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestCancellation(
                MockServer,
                x => Client.PatchAsync<TestPayload>("api/v1/hang", string.Empty, ct: x)
            );
        });

    [UnityTest]
    public IEnumerator DeleteAsyncNoBody_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestCancellation(
                MockServer,
                x => Client.DeleteAsync("api/v1/hang", ct: x)
            );
        });

    [UnityTest]
    public IEnumerator DeleteAsyncWithBody_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestCancellation(
                MockServer,
                x => Client.DeleteAsync<TestPayload>("api/v1/hang", ct: x)
            );
        });

    [UnityTest]
    public IEnumerator GetSpriteAsync_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestCancellation(
                MockServer,
                x => Client.GetSpriteAsync("test.png", ct: x)
            );
        });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestCancellation(
                MockServer,
                x => Client.GetCachedSpriteAsync("test.png", ct: x)
            );
        });

    [UnityTest]
    public IEnumerator GetAudioClipAsync_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestCancellation(
                MockServer,
                x => Client.GetAudioClipAsync("test.mp3", ct: x)
            );
        });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_WhenCancelledViaToken_ThrowsOperationCanceledException() =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestCancellation(
                MockServer,
                x => Client.GetCachedAudioClipAsync("test.mp3", ct: x)
            );
        });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static async UniTask TestRequestCancellation(MockHttpServer server,
        Func<CancellationToken, UniTask> requestFunc)
    {
        // Arrange
        server.DelayMilliseconds = 2000; // Force server to hang for 2 seconds
        var cts = new CancellationTokenSource();

        // Cancel the token after 50ms
        cts.CancelAfter(50);

        // Act & Assert
        try
        {
            await requestFunc(cts.Token);
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