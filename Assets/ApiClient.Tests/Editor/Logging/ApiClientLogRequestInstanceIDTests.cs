using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using ApiClientLib;
using System.Text;
using System;

public class ApiClientLogRequestInstanceIDTests : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetAsyncNoBody_CheckInstanceIDLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            x => x.GetAsync("api/test")
        );
    });

    [UnityTest]
    public IEnumerator GetAsyncWithBody_CheckInstanceIDLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            x => x.GetAsync<string>("api/test")
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncNoBody_CheckInstanceIDLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            x => x.PostAsync("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PostAsyncWithBody_CheckInstanceIDLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            x => x.PostAsync<string>("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncNoBody_CheckInstanceIDLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            x => x.PutAsync("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PutAsyncWithBody_CheckInstanceIDLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            x => x.PutAsync<string>("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncNoBody_CheckInstanceIDLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            x => x.PatchAsync("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator PatchAsyncWithBody_CheckInstanceIDLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            x => x.PatchAsync<string>("api/test", string.Empty)
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncNoBody_CheckInstanceIDLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            x => x.DeleteAsync("api/test")
        );
    });

    [UnityTest]
    public IEnumerator DeleteAsyncWithBody_CheckInstanceIDLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            Encoding.UTF8.GetBytes("Hello"),
            x => x.DeleteAsync<string>("api/test")
        );
    });

    [UnityTest]
    public IEnumerator GetSpriteAsync_CheckInstanceIDLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            RealPngBytes,
            x => x.GetSpriteAsync("test.png")
        );
    });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_CheckInstanceIDLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            RealPngBytes,
            x => x.GetCachedSpriteAsync("test.png")
        );
    });

    [UnityTest]
    public IEnumerator GetAudioClipAsync_CheckInstanceIDLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            RealMp3Bytes,
            x => x.GetAudioClipAsync("test.mp3")
        );
    });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_CheckInstanceIDLog() => UniTask.ToCoroutine(async () =>
    {
        await TestHttpRequestSuccessLog(
            MockServer,
            RealMp3Bytes,
            x => x.GetCachedAudioClipAsync("test.mp3")
        );
    });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static async UniTask TestHttpRequestSuccessLog(MockHttpServer server, byte[] responseBytes,
        Func<ApiClient, UniTask> requestFunc)
    {
        // Arrange
        var client1 = new ApiClient(server.ServerUrl);
        var client2 = new ApiClient(server.ServerUrl);
        var logCount1 = 0;
        var logCount2 = 0;
        client1.OnRequestCompleted += CheckLog1;
        client2.OnRequestCompleted += CheckLog2;
        server.ResponseStatusCode = 200;
        server.ResponseBytes = responseBytes;

        // Act
        try
        {
            await requestFunc(client1);
            await requestFunc(client2);
        }
        catch (Exception e)
        {
            // Assert: No Exception should have been thrown.
            Assert.Fail();
        }
        finally
        {
            client1.OnRequestCompleted -= CheckLog1;
            client2.OnRequestCompleted -= CheckLog2;
        }

        // Assert: Make sure different clients have unique ids.
        Assert.AreNotEqual(client1.InstanceID, client2.InstanceID, "Clients should have unique IDs");

        // Assert: Exactly one log should have been registered for each client.
        Assert.AreEqual(1, logCount1, $"Expected exactly one log, but got {logCount1}");
        Assert.AreEqual(1, logCount2, $"Expected exactly one log, but got {logCount2}");

        return;

        // Assertions
        void CheckLog1(ApiEventData data)
        {
            logCount1++;
            Assert.AreEqual(client1.InstanceID, data.InstanceID, $"Expected instance ID to be {client1.InstanceID}");
        }

        // Assertions
        void CheckLog2(ApiEventData data)
        {
            logCount2++;
            Assert.AreEqual(client2.InstanceID, data.InstanceID, $"Expected instance ID to be {client2.InstanceID}");
        }
    }
}