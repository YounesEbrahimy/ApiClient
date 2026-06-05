using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using ApiClientLib;
using System.IO;

[TestFixture]
public class ApiClientFileSystemAndUtilityTests : ApiClientTestBase
{
    [TestCase("https://example.com/images/hero.png")]
    [TestCase("http://example.com/images/hero.png")]
    [TestCase("https://cdn.example.com/photos/banner.jpg")]
    [TestCase("http://cdn.example.com/photos/banner.jpg")]
    [TestCase("https://assets.example.org/img/logo.jpeg")]
    [TestCase("http://assets.example.org/img/logo.jpeg")]
    [TestCase("https://media.example.net/gallery/image.webp")]
    [TestCase("http://media.example.net/gallery/image.webp")]
    [TestCase("https://static.example.com/icons/app-icon.svg")]
    [TestCase("http://static.example.com/icons/app-icon.svg")]
    [TestCase("https://images.example.org/uploads/photo.gif")]
    [TestCase("http://images.example.org/uploads/photo.gif")]
    [TestCase("https://files.example.net/pictures/background.bmp")]
    [TestCase("http://files.example.net/pictures/background.bmp")]
    [TestCase("https://content.example.com/2026/06/landscape.png")]
    [TestCase("http://content.example.com/2026/06/landscape.png")]
    [TestCase("https://subdomain.example.com/pictures/picture%20gallery/image01.jpg")]
    [TestCase("https://img.example.org/products/track_12345.mp3")]
    [TestCase("http://img.example.org/products/track_12345.mp3")]
    [TestCase("https://storage.example.net/media/audio.ogg")]
    [TestCase("http://storage.example.net/media/audio.ogg")]
    [TestCase("https://example.com/sound.wav?size=large")]
    [TestCase("http://example.com/sound.wav?size=large")]
    [TestCase("https://example.org/audio/song.mp3?v=2")]
    [TestCase("http://example.org/audio/song.mp3?v=2")]
    [TestCase("https://cdn.example.net/assets/track.flac#section")]
    [TestCase("http://cdn.example.net/assets/track.flac#section")]
    [TestCase("https://subdomain.example.com/audio/audio%20gallery/track01.m4a")]
    [TestCase("http://subdomain.example.com/audio/audio%20gallery/track01.m4a")]
    public void ComputeHash_AlwaysReturnsConsistentHashForSameInput(string url)
    {
        // Act
        var hash1 = ApiClient.ComputeHash(url);
        var hash2 = ApiClient.ComputeHash(url);

        // Assert
        Assert.AreEqual(hash1, hash2, "Hashes for the exact same URL should be identical.");
        Assert.IsNotEmpty(hash1, "Hash should not be null or empty.");
        // SHA256 produces 32 bytes, hex string is 64 characters
        Assert.AreEqual(64, hash1.Length, "Hash should be a 64-character hex string.");
    }

    [UnityTest]
    public IEnumerator GetCachedSpriteAsync_WhenCacheDirectoryIsDeleted_RecreatesDirectoryGracefully() =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange
            MockServer.ResponseStatusCode = 200;
            MockServer.ResponseBytes = RealPngBytes;

            // Simulate the OS or user deleting the directory after the client was initialized
            if (Directory.Exists(Client._cacheDir))
            {
                Directory.Delete(Client._cacheDir, true);
            }

            // Act
            await Client.GetCachedSpriteAsync(MockServer.ServerUrl + "recreate-dir.png");

            // Assert
            Assert.IsTrue(Directory.Exists(Client._cacheDir),
                "The client should recreate the missing cache directory.");
            Assert.IsTrue(Directory.GetFiles(Client._cacheDir, "*.png").Length == 1,
                "The sprite should be saved inside the newly recreated directory.");
        });

    [UnityTest]
    public IEnumerator GetCachedAudioClipAsync_WhenCacheDirectoryIsDeleted_RecreatesDirectoryGracefully() =>
        UniTask.ToCoroutine(async () =>
        {
            // Arrange
            MockServer.ResponseStatusCode = 200;
            MockServer.ResponseBytes = RealMp3Bytes;

            // Simulate the OS or user deleting the directory after the client was initialized
            if (Directory.Exists(Client._cacheDir))
            {
                Directory.Delete(Client._cacheDir, true);
            }

            // Act
            await Client.GetCachedAudioClipAsync(MockServer.ServerUrl + "recreate-dir.mp3");

            // Assert
            Assert.IsTrue(Directory.Exists(Client._cacheDir),
                "The client should recreate the missing cache directory.");
            Assert.IsTrue(Directory.GetFiles(Client._cacheDir, "*.mp3").Length == 1,
                "The audio clip should be saved inside the newly recreated directory.");
        });
}