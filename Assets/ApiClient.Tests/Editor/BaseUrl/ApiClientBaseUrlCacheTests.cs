using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using ApiClientLib;
using System.IO;

[TestFixture]
public class ApiClientBaseUrlCacheTests : ApiClientTestBase
{
    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_RelativeAndAbsoluteUrl_DownloadsOnlyOnce() =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var requestCalls = 0;
            MockServer.ResponseStatusCode = 200;
            MockServer.ResponseBytes = RealPngBytes;
            MockServer.OnRequestReceived = _ => requestCalls++;
            var relativeUrl = "test.png";

            // Act: Download a sprite from the same URL, once using absolute URL and then using relative URL
            var spr1 = await Client.GetCachedSpriteAsync(MockServer.ServerUrl + relativeUrl, urlType: UrlType.Absolute);
            Client.SetBaseUrl(MockServer.ServerUrl);
            var spr2 = await Client.GetCachedSpriteAsync(relativeUrl, urlType: UrlType.Relative);

            // Assertions: Must have downloaded the Sprite only once, and Only one image must exist in cache folder
            Assert.AreEqual(1, requestCalls, "Server must have been hit exactly once.");
            Assert.IsTrue(Directory.Exists(Client._cacheDir), "Cache folder should have been created.");
            Assert.AreEqual(1, Directory.GetFiles(Client._cacheDir, "*.png").Length,
                "Client should have cached exactly one .png file.");
        });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_RelativeAndAbsoluteUrl_DownloadsOnlyOnce() =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var requestCalls = 0;
            MockServer.ResponseStatusCode = 200;
            MockServer.ResponseBytes = RealMp3Bytes;
            MockServer.OnRequestReceived = _ => requestCalls++;
            var relativeUrl = "test.mp3";

            // Act: Download an AudioCLip from the same URL, once using absolute URL and then using relative URL
            var clip1 = await Client.GetCachedAudioClipAsync(MockServer.ServerUrl + relativeUrl,
                urlType: UrlType.Absolute);
            Client.SetBaseUrl(MockServer.ServerUrl);
            var clip2 = await Client.GetCachedAudioClipAsync(relativeUrl, urlType: UrlType.Relative);

            // Assertions: Must have downloaded the AudioClip only once, and Only one AudioCLip must exist in cache folder
            Assert.AreEqual(1, requestCalls, "Server must have been hit exactly once.");
            Assert.IsTrue(Directory.Exists(Client._cacheDir), "Cache folder should have been created.");
            Assert.AreEqual(1, Directory.GetFiles(Client._cacheDir, "*.mp3").Length,
                "Client should have cached exactly one .mp3 file.");
        });
}