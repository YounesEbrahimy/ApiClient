using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using System.IO;
using System;

public class ApiClientInvalidateCacheTests : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator InvalidateCacheAsync_DeletesExistingCacheFiles() => UniTask.ToCoroutine(async () =>
    {
        // Arrange
        var requestCount = 5;
        MockServer.ResponseStatusCode = 200;
        MockServer.ResponseBytes = RealPngBytes;

        // Act: Cache 5 different Sprites with different URLs
        var tasks = new List<UniTask<Sprite>>();
        for (var i = 0; i < requestCount; i++)
        {
            tasks.Add(Client.GetCachedSpriteAsync(MockServer.ServerUrl + $"test_[{i}].png"));
        }

        await UniTask.WhenAll(tasks);

        // Assert: Ensure all files exist on disk
        Assert.AreEqual(requestCount, Directory.GetFiles(Client._cacheDir, "*.png").Length,
            $"{requestCount} files should have been cached.");

        // Act: Invalidate cache
        await Client.InvalidateCacheAsync();

        // Assert: Check if cache directory is recreated after deletion
        Assert.IsTrue(Directory.Exists(Client._cacheDir), "Cache directory should have been recreated.");

        // Assert: Check if all cache files have been deleted successfully
        Assert.AreEqual(0, Directory.GetFiles(Client._cacheDir, "*.png").Length,
            "files should have been deleted.");
    });

    [UnityTest]
    public IEnumerator InvalidateCacheAsync_RedownloadsSameUrlAfterInvalidation() => UniTask.ToCoroutine(async () =>
    {
        // Arrange
        var callCounter = 0;
        var sameUrl = MockServer.ServerUrl + "test.png";
        MockServer.ResponseStatusCode = 200;
        MockServer.ResponseBytes = RealPngBytes;
        MockServer.OnRequestReceived = _ => callCounter++;

        // Act: Download and cache a Sprite
        await Client.GetCachedSpriteAsync(sameUrl);

        // Act: Invalidate cache
        await Client.InvalidateCacheAsync();

        // Act: Redownload and cache the same Sprite using the same URL
        await Client.GetCachedSpriteAsync(sameUrl);

        // Assert: Ensure the server received both requests
        Assert.AreEqual(2, callCounter, "The server should have been hit twice due to cache invalidation.");
    });

    [UnityTest]
    public IEnumerator InvalidateCacheAsync_WorksWhenTheCacheDirectoryDoesntExistYet() =>
        UniTask.ToCoroutine(async () =>
        {
            // Ensure the cache directory doesn't exist.
            if (Directory.Exists(Client._cacheDir))
            {
                Directory.Delete(Client._cacheDir, true);
            }

            try
            {
                // Act: Invalidate cache
                await Client.InvalidateCacheAsync();
            }
            catch (Exception e)
            {
                // Assert: No exceptions should have been thrown!
                Assert.Fail();
            }

            // Assert: Check if cache directory is created successfully
            Assert.IsTrue(Directory.Exists(Client._cacheDir), "Cache directory should have been created.");
        });

    [UnityTest]
    public IEnumerator InvalidateCacheAsync_WorksWhenTheCacheIsAlreadyEmpty() => UniTask.ToCoroutine(async () =>
    {
        // Arrange
        var callCounter = 0;
        MockServer.ResponseStatusCode = 200;
        MockServer.ResponseBytes = RealPngBytes;
        MockServer.OnRequestReceived = _ => callCounter++;

        // Act: Invalidate cache at the beginning (eg: Application startup)
        await Client.InvalidateCacheAsync();

        // Act: Download and cache a Sprite
        await Client.GetCachedSpriteAsync(MockServer.ServerUrl + "test.png");

        // Assert: Ensure the server received the request
        Assert.AreEqual(1, callCounter, "The server should have been hit once.");
    });

    [UnityTest]
    public IEnumerator InvalidateCacheAsync_RegeneratesIndexFileCorrectly() => UniTask.ToCoroutine(async () =>
    {
        // Arrange
        MockServer.ResponseStatusCode = 200;
        MockServer.ResponseBytes = RealPngBytes;

        // Act: Download and cache a Sprite
        await Client.GetCachedSpriteAsync(MockServer.ServerUrl + "test_first.png");

        // Act: Invalidate cache
        await Client.InvalidateCacheAsync();

        // Assert: Ensure the index file is deleted
        Assert.IsFalse(File.Exists(Client._cacheIndexPath), "The index file should have been deleted.");

        // Act: Download and cache another Sprite
        await Client.GetCachedSpriteAsync(MockServer.ServerUrl + "test_second.png");

        // Assert: Ensure the index file is recreated
        Assert.IsTrue(File.Exists(Client._cacheIndexPath), "The index file should have been recreated.");
    });

    [UnityTest]
    public IEnumerator InvalidateCacheAsync_WaitsForInFlightWriteBeforeDeleting() => UniTask.ToCoroutine(async () =>
    {
        // Arrange: a small server-side delay guarantees the download is still in-flight
        // (and therefore still holding its per-file lock) when we call InvalidateCacheAsync.
        var requestReceived = new UniTaskCompletionSource();
        MockServer.ResponseStatusCode = 200;
        MockServer.ResponseBytes = RealPngBytes;
        MockServer.DelayMilliseconds = 500;
        MockServer.OnRequestReceived = _ => requestReceived.TrySetResult();

        // Act: start the download (don't await yet), wait until the server has received it
        // (lock is now held), then fire an invalidation while the write is still pending.
        var downloadTask = Client.GetCachedSpriteAsync(MockServer.ServerUrl + "in_flight_write.png");
        await requestReceived.Task;
        var invalidateTask = Client.InvalidateCacheAsync();

        var sprite = await downloadTask;
        await invalidateTask;

        // Assert: the write was allowed to complete cleanly...
        Assert.IsNotNull(sprite, "The in-flight download/cache-write should have completed without error.");

        // ...but since the invalidation could only run after the write released its lock,
        // the resulting cache should end up empty rather than containing a half-deleted file.
        Assert.AreEqual(0, Directory.GetFiles(Client._cacheDir, "*.png").Length,
            "The file written during the race should have been removed by the invalidation that followed it.");
    });

    [UnityTest]
    public IEnumerator InvalidateCacheAsync_WaitsForInFlightCacheHitReadBeforeDeleting() =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange: prime the cache so the next request for the same URL is a cache hit
            // that only reads from disk, we rely on repeated attempts to surface a race).
            var testCount = 25;
            var url = MockServer.ServerUrl + "test.png";
            MockServer.ResponseStatusCode = 200;
            MockServer.ResponseBytes = RealPngBytes;

            // Act / Assert: race a cache-hit read against an invalidation repeatedly. The
            // per-file lock should serialize them so the read never throws or returns
            // corrupt data, regardless of which call wins the race to start first.
            for (var i = 0; i < testCount; i++)
            {
                // Re-seed the cache for each iteration since invalidation just cleared it.
                await Client.GetCachedSpriteAsync(url);

                var readTask = Client.GetCachedSpriteAsync(url);
                var invalidateTask = Client.InvalidateCacheAsync();

                Sprite sprite = null;
                try
                {
                    sprite = await readTask;
                }
                catch (Exception e)
                {
                    Assert.Fail($"Iteration {i}: cache-hit read threw while racing invalidation: {e}");
                }

                await invalidateTask;
                Assert.IsNotNull(sprite, $"Iteration {i}: cache-hit read should return a valid sprite.");
            }
        });

    [UnityTest]
    public IEnumerator InvalidateCacheAsync_RequestStartedDuringInvalidationRepopulatesCacheAfterward() =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var callCounter = 0;
            MockServer.ResponseStatusCode = 200;
            MockServer.ResponseBytes = RealPngBytes;
            MockServer.OnRequestReceived = _ => callCounter++;
            var sameUrl = MockServer.ServerUrl + "test.png";

            // Act: fire the invalidation and a brand-new cache request for an unseen URL
            // at the same time, with no synchronization between them.
            var invalidateTask = Client.InvalidateCacheAsync();
            var requestTask = Client.GetCachedSpriteAsync(sameUrl);

            await invalidateTask;
            await requestTask;

            // Assert: the request completed successfully and is now cached on disk.
            Assert.AreEqual(1, callCounter, "The server should have been hit exactly once.");
            Assert.AreEqual(1, Directory.GetFiles(Client._cacheDir, "*.png").Length,
                "The request started mid-invalidation should have re-populated the cache.");

            // Act: request the same URL again.
            await Client.GetCachedSpriteAsync(sameUrl);

            // Assert: it should now be served from cache, not hit the server again.
            Assert.AreEqual(1, callCounter, "The second request should have been served from cache.");
        });

    [UnityTest]
    public IEnumerator InvalidateCacheAsync_ConcurrentCallsDoNotDeadlockOrCorruptState() =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange
            MockServer.ResponseStatusCode = 200;
            MockServer.ResponseBytes = RealPngBytes;

            // Act: Download and cache a Sprite
            await Client.GetCachedSpriteAsync(MockServer.ServerUrl + "test_before.png");

            // Act: fire several invalidations at the same time.
            var invalidateTasks = new List<UniTask>();
            for (var i = 0; i < 5; i++)
            {
                invalidateTasks.Add(Client.InvalidateCacheAsync());
            }

            await UniTask.WhenAll(invalidateTasks);

            // Assert: cache directory ends up in a valid, empty state.
            Assert.IsTrue(Directory.Exists(Client._cacheDir), "Cache directory should still exist.");
            Assert.AreEqual(0, Directory.GetFiles(Client._cacheDir, "*.png").Length,
                "Cache should be empty after concurrent invalidations.");

            // Assert: the client is still usable afterward.
            await Client.GetCachedSpriteAsync(MockServer.ServerUrl + "test_after.png");
            Assert.AreEqual(1, Directory.GetFiles(Client._cacheDir, "*.png").Length,
                "Client should still be able to cache files after concurrent invalidations.");
        });

    [UnityTest]
    public IEnumerator InvalidateCacheAsync_CompletesPromptlyAfterPriorRequestsHaveFinished() =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange: cache several files and let every request fully complete first, so
            // every per-file lock that was created has already been released.
            MockServer.ResponseStatusCode = 200;
            MockServer.ResponseBytes = RealPngBytes;
            for (var i = 0; i < 10; i++)
            {
                await Client.GetCachedSpriteAsync(MockServer.ServerUrl + $"test_[{i}].png");
            }

            // Act
            var stopwatch = Stopwatch.StartNew();
            await Client.InvalidateCacheAsync();
            stopwatch.Stop();

            // Assert: the per-file lock acquisition loop should terminate immediately since
            // none of the previously-used locks are currently held by anyone.
            Assert.Less(stopwatch.ElapsedMilliseconds, 2000,
                "InvalidateCacheAsync should not hang waiting on locks nobody holds anymore.");
        });

    [UnityTest]
    public IEnumerator InvalidateCacheAsync_ThrowsAndReleasesLocksOnCancellation() => UniTask.ToCoroutine(async () =>
    {
        // Arrange
        MockServer.ResponseStatusCode = 200;
        MockServer.ResponseBytes = RealPngBytes;
        await Client.GetCachedSpriteAsync(MockServer.ServerUrl + "test.png");

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act / Assert: a pre-cancelled token should make the call throw rather than run.
        try
        {
            await Client.InvalidateCacheAsync(cts.Token);
            Assert.Fail("Expected an OperationCanceledException.");
        }
        catch (OperationCanceledException)
        {
            // expected
        }

        // Assert: any locks taken were released cleanly, so a normal, non-cancelled call
        // right afterward still succeeds instead of hanging.
        await Client.InvalidateCacheAsync(CancellationToken.None);
        Assert.IsTrue(Directory.Exists(Client._cacheDir), "Cache directory should exist after recovery.");
    });

    [UnityTest]
    public IEnumerator InvalidateCacheAsync_SubsequentRequestsCacheAndServeNormallyAfterInvalidation() =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var callCounter = 0;
            MockServer.ResponseStatusCode = 200;
            MockServer.ResponseBytes = RealPngBytes;
            MockServer.OnRequestReceived = _ => callCounter++;
            var url = MockServer.ServerUrl + "test.png";

            // Act: Invalidate cache.
            await Client.InvalidateCacheAsync();

            // Act: first request after invalidation should hit the server and cache the result.
            await Client.GetCachedSpriteAsync(url);
            Assert.AreEqual(1, callCounter, "First request after invalidation should hit the server.");

            // Act: second request for the same URL should be served from cache.
            await Client.GetCachedSpriteAsync(url);
            Assert.AreEqual(1, callCounter, "Second request should be served from the cache, not the server.");

            // Assert: the file and index entry both exist on disk.
            Assert.AreEqual(1, Directory.GetFiles(Client._cacheDir, "*.png").Length);
            Assert.IsTrue(File.Exists(Client._cacheIndexPath));
        });
}