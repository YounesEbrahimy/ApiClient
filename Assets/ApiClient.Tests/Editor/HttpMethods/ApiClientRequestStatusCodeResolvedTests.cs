using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using ApiClientLib;
using System.Text;
using System;

[TestFixture]
public class ApiClientRequestStatusCodeResolvedTests : ApiClientTestBase
{
    // ── GET Tests ─────────────────────────────────────────────────────────────

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(204, ExpectedResult = null)]
    [TestCase(400, ExpectedResult = null)]
    [TestCase(401, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator GetAsyncNoBody_TriggersEventWithCorrectStatusCode(int statusCode) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestStatusCodeEvent(
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
    [TestCase(400, ExpectedResult = null)]
    [TestCase(401, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator GetAsyncWithBody_TriggersEventWithCorrectStatusCode(int statusCode) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestStatusCodeEvent(
                MockServer,
                Encoding.UTF8.GetBytes("Hello"),
                Client,
                () => Client.GetAsync<string>("api/test"),
                statusCode
            );
        });

    // ── POST Tests ────────────────────────────────────────────────────────────

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(204, ExpectedResult = null)]
    [TestCase(400, ExpectedResult = null)]
    [TestCase(401, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator PostAsyncNoBody_TriggersEventWithCorrectStatusCode(int statusCode) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestStatusCodeEvent(
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
    [TestCase(400, ExpectedResult = null)]
    [TestCase(401, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator PostAsyncWithBody_TriggersEventWithCorrectStatusCode(int statusCode) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestStatusCodeEvent(
                MockServer,
                Encoding.UTF8.GetBytes("Hello"),
                Client,
                () => Client.PostAsync<string>("api/test", string.Empty),
                statusCode
            );
        });

    // ── PUT Tests ─────────────────────────────────────────────────────────────

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(204, ExpectedResult = null)]
    [TestCase(400, ExpectedResult = null)]
    [TestCase(401, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator PutAsyncNoBody_TriggersEventWithCorrectStatusCode(int statusCode) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestStatusCodeEvent(
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
    [TestCase(400, ExpectedResult = null)]
    [TestCase(401, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator PutAsyncWithBody_TriggersEventWithCorrectStatusCode(int statusCode) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestStatusCodeEvent(
                MockServer,
                Encoding.UTF8.GetBytes("Hello"),
                Client,
                () => Client.PutAsync<string>("api/test", string.Empty),
                statusCode
            );
        });

    // ── PATCH Tests ───────────────────────────────────────────────────────────

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(204, ExpectedResult = null)]
    [TestCase(400, ExpectedResult = null)]
    [TestCase(401, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator PatchAsyncNoBody_TriggersEventWithCorrectStatusCode(int statusCode) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestStatusCodeEvent(
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
    [TestCase(400, ExpectedResult = null)]
    [TestCase(401, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator PatchAsyncWithBody_TriggersEventWithCorrectStatusCode(int statusCode) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestStatusCodeEvent(
                MockServer,
                Encoding.UTF8.GetBytes("Hello"),
                Client,
                () => Client.PatchAsync<string>("api/test", string.Empty),
                statusCode
            );
        });

    // ── DELETE Tests ──────────────────────────────────────────────────────────

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(201, ExpectedResult = null)]
    [TestCase(204, ExpectedResult = null)]
    [TestCase(400, ExpectedResult = null)]
    [TestCase(401, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator DeleteAsyncNoBody_TriggersEventWithCorrectStatusCode(int statusCode) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestStatusCodeEvent(
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
    [TestCase(400, ExpectedResult = null)]
    [TestCase(401, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator DeleteAsyncWithBody_TriggersEventWithCorrectStatusCode(int statusCode) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestStatusCodeEvent(
                MockServer,
                Encoding.UTF8.GetBytes("Hello"),
                Client,
                () => Client.DeleteAsync<string>("api/test"),
                statusCode
            );
        });

    // ── Sprite Tests ──────────────────────────────────────────────────────────

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator GetSpriteAsync_TriggersEventWithCorrectStatusCode(int statusCode) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestStatusCodeEvent(
                MockServer,
                RealPngBytes,
                Client,
                () => Client.GetSpriteAsync("test.png"),
                statusCode
            );
        });

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator GetCachedSpriteAsync_TriggersEventWithCorrectStatusCode(int statusCode) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestStatusCodeEvent(
                MockServer,
                RealPngBytes,
                Client,
                () => Client.GetCachedSpriteAsync("test.png"),
                statusCode
            );
        });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_CacheHit_TriggersEventWithMinusOne() =>
        UniTask.ToCoroutine(async () =>
        {
            const int ServerStatusCode = 200;
            const string RelativeUrl = "test_cache.png";
            MockServer.ResponseStatusCode = ServerStatusCode;
            MockServer.ResponseBytes = RealPngBytes;

            var callCount = 0;
            var lastStatusCode = 0;

            void Handler(int code)
            {
                callCount++;
                lastStatusCode = code;
            }

            Client.OnRequestStatusCodeResolved += Handler;

            try
            {
                // 1. First request: Cache Miss -> download from server (200)
                callCount = 0;
                var res1 = await Client.GetCachedSpriteAsync(RelativeUrl);
                Assert.IsNotNull(res1.Sprite);
                Assert.AreEqual(1, callCount, "Expected exactly 1 event call on cache miss");
                Assert.AreEqual(ServerStatusCode, lastStatusCode);

                // 2. Second request: Cache Hit -> served from disk (-1)
                callCount = 0;
                var res2 = await Client.GetCachedSpriteAsync(RelativeUrl);
                Assert.IsNotNull(res2.Sprite);
                Assert.AreEqual(1, callCount, "Expected exactly 1 event call on cache hit");
                Assert.AreEqual(-1, lastStatusCode, "Expected status code -1 for cache hit");

                // 3. Invalidate cache and request again -> Cache Miss (200)
                await Client.InvalidateCacheAsync();
                callCount = 0;
                var res3 = await Client.GetCachedSpriteAsync(RelativeUrl);
                Assert.IsNotNull(res3.Sprite);
                Assert.AreEqual(1, callCount, "Expected exactly 1 event call after cache invalidation");
                Assert.AreEqual(ServerStatusCode, lastStatusCode);
            }
            finally
            {
                Client.OnRequestStatusCodeResolved -= Handler;
            }
        });

    // ── AudioClip Tests ───────────────────────────────────────────────────────

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator GetAudioClipAsync_TriggersEventWithCorrectStatusCode(int statusCode) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestStatusCodeEvent(
                MockServer,
                RealMp3Bytes,
                Client,
                () => Client.GetAudioClipAsync("test.mp3"),
                statusCode
            );
        });

    [UnityTest]
    [TestCase(200, ExpectedResult = null)]
    [TestCase(404, ExpectedResult = null)]
    [TestCase(500, ExpectedResult = null)]
    public IEnumerator GetCachedAudioClipAsync_TriggersEventWithCorrectStatusCode(int statusCode) =>
        UniTask.ToCoroutine(async () =>
        {
            await TestRequestStatusCodeEvent(
                MockServer,
                RealMp3Bytes,
                Client,
                () => Client.GetCachedAudioClipAsync("test.mp3"),
                statusCode
            );
        });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_CacheHit_TriggersEventWithMinusOne() =>
        UniTask.ToCoroutine(async () =>
        {
            const int ServerStatusCode = 200;
            const string RelativeUrl = "test_cache.mp3";
            MockServer.ResponseStatusCode = ServerStatusCode;
            MockServer.ResponseBytes = RealMp3Bytes;

            var callCount = 0;
            var lastStatusCode = 0;

            void Handler(int code)
            {
                callCount++;
                lastStatusCode = code;
            }

            Client.OnRequestStatusCodeResolved += Handler;

            try
            {
                // 1. First request: Cache Miss -> download from server (200)
                callCount = 0;
                var res1 = await Client.GetCachedAudioClipAsync(RelativeUrl);
                Assert.IsNotNull(res1.AudioClip);
                Assert.AreEqual(1, callCount, "Expected exactly 1 event call on cache miss");
                Assert.AreEqual(ServerStatusCode, lastStatusCode);

                // 2. Second request: Cache Hit -> served from disk (-1)
                callCount = 0;
                var res2 = await Client.GetCachedAudioClipAsync(RelativeUrl);
                Assert.IsNotNull(res2.AudioClip);
                Assert.AreEqual(1, callCount, "Expected exactly 1 event call on cache hit");
                Assert.AreEqual(-1, lastStatusCode, "Expected status code -1 for cache hit");

                // 3. Invalidate cache and request again -> Cache Miss (200)
                await Client.InvalidateCacheAsync();
                callCount = 0;
                var res3 = await Client.GetCachedAudioClipAsync(RelativeUrl);
                Assert.IsNotNull(res3.AudioClip);
                Assert.AreEqual(1, callCount, "Expected exactly 1 event call after cache invalidation");
                Assert.AreEqual(ServerStatusCode, lastStatusCode);
            }
            finally
            {
                Client.OnRequestStatusCodeResolved -= Handler;
            }
        });

    // ── Exception & Failure Tests ─────────────────────────────────────────────

    [UnityTest]
    public IEnumerator WhenTimeout_TriggersEventWithZero() =>
        UniTask.ToCoroutine(async () =>
        {
            const int TimeoutSeconds = 1;
            MockServer.DelayMilliseconds = 3000;

            var callCount = 0;
            var lastStatusCode = -999;

            void Handler(int code)
            {
                callCount++;
                lastStatusCode = code;
            }

            Client.OnRequestStatusCodeResolved += Handler;

            try
            {
                await Client.GetAsync("api/timeout", timeout: TimeoutSeconds);
                Assert.Fail("Expected TimeoutException to be thrown.");
            }
            catch (TimeoutException)
            {
                // Expected
            }
            finally
            {
                Client.OnRequestStatusCodeResolved -= Handler;
                MockServer.DelayMilliseconds = 0;
            }

            Assert.AreEqual(1, callCount, "Expected exactly 1 event call on timeout");
            Assert.AreEqual(0, lastStatusCode, "Expected status code 0 on timeout");
        });

    [UnityTest]
    public IEnumerator WhenInvalidUrl_TriggersEventWithZero() =>
        UniTask.ToCoroutine(async () =>
        {
            var callCount = 0;
            var lastStatusCode = -999;

            void Handler(int code)
            {
                callCount++;
                lastStatusCode = code;
            }

            Client.OnRequestStatusCodeResolved += Handler;

            try
            {
                // Pass an invalid relative URL that is an absolute URL with relative UrlType
                await Client.GetAsync("http://invalid-sub-path.com");
                Assert.Fail("Expected InvalidUrlException to be thrown.");
            }
            catch (InvalidUrlException)
            {
                // Expected
            }
            finally
            {
                Client.OnRequestStatusCodeResolved -= Handler;
            }

            Assert.AreEqual(1, callCount, "Expected exactly 1 event call on invalid URL");
            Assert.AreEqual(0, lastStatusCode, "Expected status code 0 on invalid URL");
        });

    // ── Instance Isolation Test ───────────────────────────────────────────────

    [UnityTest]
    public IEnumerator InstanceIsolation_EventOnlyFiresForOwningClientInstance() =>
        UniTask.ToCoroutine(async () =>
        {
            var clientA = new ApiClient(MockServer.ServerUrl);
            var clientB = new ApiClient(MockServer.ServerUrl);

            var countA = 0;
            var countB = 0;

            void HandlerA(int _) => countA++;
            void HandlerB(int _) => countB++;

            clientA.OnRequestStatusCodeResolved += HandlerA;
            clientB.OnRequestStatusCodeResolved += HandlerB;

            MockServer.ResponseStatusCode = 200;
            MockServer.ResponseBytes = Encoding.UTF8.GetBytes("OK");

            try
            {
                // Request from clientA only
                await clientA.GetAsync("api/testA");

                Assert.AreEqual(1, countA, "ClientA event should have fired once.");
                Assert.AreEqual(0, countB, "ClientB event should NOT have fired.");

                // Request from clientB only
                await clientB.GetAsync("api/testB");

                Assert.AreEqual(1, countA, "ClientA event count should still be 1.");
                Assert.AreEqual(1, countB, "ClientB event should now have fired once.");
            }
            finally
            {
                clientA.OnRequestStatusCodeResolved -= HandlerA;
                clientB.OnRequestStatusCodeResolved -= HandlerB;
            }
        });

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static async UniTask TestRequestStatusCodeEvent(MockHttpServer server, byte[] responseBytes,
        ApiClient client, Func<UniTask> requestFunc, int expectedStatusCode)
    {
        // Arrange
        var callCount = 0;
        var receivedStatusCode = int.MinValue;
        server.ResponseStatusCode = expectedStatusCode;
        server.ResponseBytes = responseBytes;

        void Handler(int code)
        {
            callCount++;
            receivedStatusCode = code;
        }

        client.OnRequestStatusCodeResolved += Handler;

        // Act
        try
        {
            await requestFunc();
        }
        catch (ApiException)
        {
            // Expected for non-2xx status codes (or 204 for generic typed requests)
        }
        catch (Exception)
        {
            Assert.Fail("Unexpected exception thrown during request.");
        }
        finally
        {
            client.OnRequestStatusCodeResolved -= Handler;
        }

        // Assert
        Assert.AreEqual(1, callCount, $"Expected exactly one event invocation, but got {callCount}");
        Assert.AreEqual(expectedStatusCode, receivedStatusCode,
            $"Expected status code {expectedStatusCode}, but received {receivedStatusCode}");
    }
}
