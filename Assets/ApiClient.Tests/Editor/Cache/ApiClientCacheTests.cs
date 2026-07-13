using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using ApiClientLib.Helpers;
using System.Collections;
using Newtonsoft.Json;
using NUnit.Framework;
using ApiClientLib;
using System.IO;
using System;

public class ApiClientCacheTests : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_ConcurrentRequests_OnlyFetchesOnce() => UniTask.ToCoroutine(async () =>
    {
        // Arrange
        var requestCount = 5;
        var callCounter = 0;
        MockServer.ResponseStatusCode = 200;
        MockServer.ResponseBytes = RealPngBytes;
        MockServer.DelayMilliseconds = 2000;
        MockServer.OnRequestReceived = _ => callCounter++;

        // Act: Fire 5 simultaneous requests for the same URL
        var tasks = new List<UniTask<SpriteResponse>>();
        for (var i = 0; i < requestCount; i++)
        {
            tasks.Add(Client.GetCachedSpriteAsync("test.png"));
        }

        await UniTask.WhenAll(tasks);

        // Assert: Ensure the server only received 1 request despite 5 calls
        Assert.AreEqual(1, callCounter, "The server should only have been hit once due to caching/locking.");

        // Ensure files exist on disk
        var cacheDir = Client.CacheDirectoryPath();
        Assert.IsTrue(Directory.GetFiles(cacheDir, "*.png").Length == 1, "Cache file was not saved to disk.");
    });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_WhenFileDeleted_ReFetches() => UniTask.ToCoroutine(async () =>
    {
        // 1. First fetch to populate cache
        MockServer.ResponseStatusCode = 200;
        MockServer.ResponseBytes = RealPngBytes;
        await Client.GetCachedSpriteAsync("test.png");

        // 2. Manually corrupt/delete the cache folder
        var cacheDir = Client.CacheDirectoryPath();
        var files = Directory.GetFiles(cacheDir, "*.png");
        File.Delete(files[0]);

        // 3. Second fetch
        var callCounter = 0;
        MockServer.OnRequestReceived = _ => callCounter++;
        await Client.GetCachedSpriteAsync("test.png");

        // Assert: It should have noticed the missing file and re-fetched
        Assert.AreEqual(1, callCounter, "The client should have re-fetched the file because the cache was missing.");
    });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_WhenCacheIsExpired_ReFetchesFromServer() => UniTask.ToCoroutine(async () =>
    {
        // Arrange
        var url = MockServer.ServerUrl + "expired.png";
        var key = CachedRequestHandling.ComputeHash(url);

        // Manually inject an expired cache index entry (15 days old, with a 14-day lifespan)
        Directory.CreateDirectory(Client.CacheDirectoryPath());
        var expiredEntry = new CacheEntry { Url = url, CachedAt = DateTime.UtcNow.AddDays(-15) };
        var index = new Dictionary<string, CacheEntry> { { key, expiredEntry } };

        await File.WriteAllTextAsync(Client.CacheIndexPath(), JsonConvert.SerializeObject(index));
        await File.WriteAllBytesAsync(Path.Combine(Client.CacheDirectoryPath(), $"{key}.png"), RealPngBytes);

        var callCounter = 0;
        MockServer.ResponseStatusCode = 200;
        MockServer.ResponseBytes = RealPngBytes;
        MockServer.OnRequestReceived = _ => callCounter++;

        // Act
        await Client.GetCachedSpriteAsync(url, cacheDays: 14, urlType: UrlType.Absolute);

        // Assert
        Assert.AreEqual(1, callCounter, "Should hit the server because the local cache entry has expired.");
    });

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_WithZeroCacheDays_AlwaysReFetches() => UniTask.ToCoroutine(async () =>
    {
        // Arrange
        const string url = "always-fresh.png";
        MockServer.ResponseStatusCode = 200;
        MockServer.ResponseBytes = RealPngBytes;

        var callCounter = 0;
        MockServer.OnRequestReceived = _ => callCounter++;

        // Act: Fetch twice consecutively with cacheDays set to 0
        await Client.GetCachedSpriteAsync(url, cacheDays: 0);
        await Client.GetCachedSpriteAsync(url, cacheDays: 0);

        // Assert
        Assert.AreEqual(2, callCounter, "Should bypass cache validity check and fetch twice because cacheDays is 0.");
    });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_ConcurrentRequests_OnlyFetchesOnce() => UniTask.ToCoroutine(async () =>
    {
        // Arrange
        var requestCount = 5;
        var callCounter = 0;
        MockServer.ResponseStatusCode = 200;
        MockServer.ResponseBytes = RealMp3Bytes;
        MockServer.DelayMilliseconds = 2000;
        MockServer.OnRequestReceived = _ => callCounter++;

        // Act: Fire 5 simultaneous requests for the same URL
        var tasks = new List<UniTask<AudioClipResponse>>();
        for (var i = 0; i < requestCount; i++)
        {
            tasks.Add(Client.GetCachedAudioClipAsync("test.mp3"));
        }

        await UniTask.WhenAll(tasks);

        // Assert: Ensure the server only received 1 request despite 5 calls
        Assert.AreEqual(1, callCounter, "The server should only have been hit once due to caching/locking.");

        // Ensure files exist on disk
        var cacheDir = Client.CacheDirectoryPath();
        Assert.IsTrue(Directory.GetFiles(cacheDir, "*.mp3").Length == 1, "Cache file was not saved to disk.");
    });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_WhenFileDeleted_ReFetches() => UniTask.ToCoroutine(async () =>
    {
        // 1. First fetch to populate cache
        MockServer.ResponseStatusCode = 200;
        MockServer.ResponseBytes = RealMp3Bytes;
        await Client.GetCachedAudioClipAsync("test.mp3");

        // 2. Manually corrupt/delete the cache folder
        var cacheDir = Client.CacheDirectoryPath();
        var files = Directory.GetFiles(cacheDir, "*.mp3");
        File.Delete(files[0]);

        // 3. Second fetch
        var callCounter = 0;
        MockServer.OnRequestReceived = _ => callCounter++;
        await Client.GetCachedAudioClipAsync("test.mp3");

        // Assert: It should have noticed the missing file and re-fetched
        Assert.AreEqual(1, callCounter, "The client should have re-fetched the file because the cache was missing.");
    });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_WhenCacheIsExpired_ReFetchesFromServer() =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var url = MockServer.ServerUrl + "expired.mp3";
            var key = CachedRequestHandling.ComputeHash(url);

            // Manually inject an expired cache index entry (15 days old, with a 14-day lifespan)
            Directory.CreateDirectory(Client.CacheDirectoryPath());
            var expiredEntry = new CacheEntry { Url = url, CachedAt = DateTime.UtcNow.AddDays(-15) };
            var index = new Dictionary<string, CacheEntry> { { key, expiredEntry } };

            await File.WriteAllTextAsync(Client.CacheIndexPath(), JsonConvert.SerializeObject(index));
            await File.WriteAllBytesAsync(Path.Combine(Client.CacheDirectoryPath(), $"{key}.mp3"), RealMp3Bytes);

            var callCounter = 0;
            MockServer.ResponseStatusCode = 200;
            MockServer.ResponseBytes = RealMp3Bytes;
            MockServer.OnRequestReceived = _ => callCounter++;

            // Act
            await Client.GetCachedAudioClipAsync(url, cacheDays: 14, urlType: UrlType.Absolute);

            // Assert
            Assert.AreEqual(1, callCounter, "Should hit the server because the local cache entry has expired.");
        });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_WithZeroCacheDays_AlwaysReFetches() => UniTask.ToCoroutine(async () =>
    {
        // Arrange
        const string url = "always-fresh.mp3";
        MockServer.ResponseStatusCode = 200;
        MockServer.ResponseBytes = RealMp3Bytes;

        var callCounter = 0;
        MockServer.OnRequestReceived = _ => callCounter++;

        // Act: Fetch twice consecutively with cacheDays set to 0
        await Client.GetCachedAudioClipAsync(url, cacheDays: 0);
        await Client.GetCachedAudioClipAsync(url, cacheDays: 0);

        // Assert
        Assert.AreEqual(2, callCounter, "Should bypass cache validity check and fetch twice because cacheDays is 0.");
    });
}